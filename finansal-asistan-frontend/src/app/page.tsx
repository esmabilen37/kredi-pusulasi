"use client";

import { useState, useRef, useEffect } from "react";
import { useRouter } from "next/navigation";
import Link from "next/link";
import { sendAssistantMessage, evaluateAffordability } from "@/lib/api";
import { mergeProfiles, isProfileReadyForEvaluation, EMPTY_PROFILE } from "@/lib/mergeProfile";
import { AffordabilityResult, ChatMessage } from "@/types/finansal";
import { useProfile } from "@/context/ProfileContext";
import { useAuth } from "@/context/AuthContext";
import Card from "@/components/ui/Card";
import Button from "@/components/ui/Button";
import Badge from "@/components/ui/Badge";
import { LoadingState, EmptyState, ErrorState } from "@/components/ui/States";
import { ChatBubble } from "@/components/chat/ChatBubble";
import ChatInput from "@/components/chat/ChatInput";
import ProfileChips from "@/components/chat/ProfileChips";
import StatCard from "@/components/result/StatCard";
import RiskMeter from "@/components/result/RiskMeter";
import TermOptionCard from "@/components/result/TermOptionCard";
import BankCard from "@/components/result/BankCard";
import { BarChart3 } from "lucide-react";

interface Message {
  role: "user" | "assistant";
  text: string;
}

function tl(n: number) {
  return Math.round(n).toLocaleString("tr-TR") + " TL";
}

export default function Home() {
  const router = useRouter();
  const { isAuthenticated, isCheckingSession, email, logout } = useAuth();

  const [messages, setMessages] = useState<Message[]>([]);
  const [chatHistory, setChatHistory] = useState<ChatMessage[]>([]);
  const { profile, setProfile } = useProfile();
  const [result, setResult] = useState<AffordabilityResult | null>(null);

  const [extracting, setExtracting] = useState(false);
  const [evaluating, setEvaluating] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const chatEndRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    chatEndRef.current?.scrollIntoView({ behavior: "smooth" });
  }, [messages]);

  // Oturum kontrolü bittiğinde giriş yapılmamışsa login sayfasına yönlendir
  useEffect(() => {
    if (!isCheckingSession && !isAuthenticated) {
      router.push("/login");
    }
  }, [isCheckingSession, isAuthenticated, router]);

  async function handleSend(text: string) {
    setError(null);
    const updatedHistory: ChatMessage[] = [
      ...chatHistory,
      { role: "user", content: text },
    ];
    setMessages((prev) => [...prev, { role: "user", text }]);
    setChatHistory(updatedHistory);
    setExtracting(true);

    try {
      const result = await sendAssistantMessage(updatedHistory);

      const merged = mergeProfiles(profile, {
        monthlyIncome: result.monthlyIncome,
        monthlyExpenses: result.monthlyExpenses,
        requestedLoanAmount: result.requestedLoanAmount,
        requestedTermMonths: result.requestedTermMonths,
        loanType: result.loanType,
        existingMonthlyDebtPayment: profile.existingMonthlyDebtPayment,
      });
      setProfile(merged);

      const understood: string[] = [];
      if (result.monthlyIncome) understood.push(`gelir: ${tl(result.monthlyIncome)}`);
      if (result.monthlyExpenses) understood.push(`gider: ${tl(result.monthlyExpenses)}`);
      if (result.requestedLoanAmount) understood.push(`talep tutar: ${tl(result.requestedLoanAmount)}`);
      if (result.requestedTermMonths) understood.push(`vade: ${result.requestedTermMonths} ay`);

      const dataSummary = understood.length > 0 ? `Anladım — ${understood.join(", ")}.` : "";

      const readyForEval = isProfileReadyForEvaluation(merged);

      const missingFields: string[] = [];
      if (!merged.monthlyIncome) missingFields.push("gelir");
      if (!merged.monthlyExpenses) missingFields.push("gider");
      if (!merged.requestedTermMonths) missingFields.push("vade");

      const cta = readyForEval
          ? missingFields.length > 0
              ? `İstersen daha fazla bilgi (${missingFields.join(", ")} gibi) paylaşabilirsin ya da doğrudan "Değerlendir" butonuna basarak sana uygun bankaları görebilirsin.`
              : "İstersen doğrudan \"Değerlendir\" butonuna basarak sana uygun bankaları görebilirsin."
          : "";
      const finalReply = [dataSummary, result.reply, cta].filter(Boolean).join(" ");

      setMessages((prev) => [...prev, { role: "assistant", text: finalReply || "Devam edebilirsin." }]);
      setChatHistory((prev) => [...prev, { role: "assistant", content: finalReply }]);
    } catch (err) {
      setError("Mesaj gönderilemedi: " + (err instanceof Error ? err.message : String(err)));
    } finally {
      setExtracting(false);
    }
  }

  async function handleEvaluate() {
    setError(null);
    setEvaluating(true);
    try {
      const r = await evaluateAffordability(profile);
      setResult(r);
    } catch (err) {
      setError("Değerlendirme başarısız oldu: " + (err instanceof Error ? err.message : String(err)));
    } finally {
      setEvaluating(false);
    }
  }

  function handleReset() {
    setMessages([]);
    setProfile(EMPTY_PROFILE);
    setChatHistory([]);
    setResult(null);
    setError(null);
  }

  const ready = isProfileReadyForEvaluation(profile);

  if (isCheckingSession || !isAuthenticated) {
    return (
        <div className="min-h-screen flex items-center justify-center">
          <LoadingState label="Yükleniyor…" />
        </div>
    );
  }

  return (
      <div className="min-h-screen">
        <header className="border-b border-[var(--border)] bg-[var(--surface)] sticky top-0 z-10">
          <div className="max-w-6xl mx-auto px-6 py-4 flex items-center justify-between">
            <div className="flex items-center gap-2.5">
              <div className="w-8 h-8 rounded-lg bg-[var(--accent)] flex items-center justify-center text-white font-bold text-sm">
                KP
              </div>
              <span className="font-semibold text-[var(--text)]">Kredi Pusulası</span>
            </div>
            <div className="flex items-center gap-2">
              <span className="text-xs text-[var(--text-muted)] mr-1">{email}</span>
              <Link href="/senaryolar">
                <Button className="flex items-center gap-1.5 text-sm px-3.5 py-2">
                  <BarChart3 size={16} />
                  Senaryoları Gör
                </Button>
              </Link>
              <Button variant="ghost" onClick={handleReset} className="text-amber-600 hover:bg-amber-50">
                Yeni sorgu
              </Button>
              <Button
                  variant="ghost"
                  onClick={async () => {
                    await logout();
                    router.push("/login");
                  }}
              >
                Çıkış
              </Button>
            </div>
          </div>
        </header>

        <main className="max-w-6xl mx-auto px-6 py-8 grid grid-cols-1 lg:grid-cols-[1fr_1.1fr] gap-6">
          <Card className="flex flex-col h-[calc(100vh-140px)]">
            <div className="flex-1 overflow-y-auto space-y-3 pr-1">
              {messages.length === 0 ? (
                  <EmptyState
                      title="Durumunu anlat ya da soru sor"
                      description='Örn: "Aylık gelirim 45000 TL, 100000 TL kredi istiyorum, 24 ay vadeli." ya da "Sabit faiz mi değişken faiz mi daha iyi?" Parça parça da yazabilirsin, hatırlıyorum.'
                  />
              ) : (
                  messages.map((m, i) => (
                      <ChatBubble key={i} role={m.role}>
                        {m.text}
                      </ChatBubble>
                  ))
              )}
              {extracting && <LoadingState label="Yanıtlanıyor…" />}
              <div ref={chatEndRef} />
            </div>

            {messages.length > 0 && (
                <div className="border-t border-[var(--border)] pt-3 mt-2">
                  <ProfileChips profile={profile} />
                </div>
            )}

            {error && (
                <div className="pt-2">
                  <ErrorState message={error} />
                </div>
            )}

            <div className="pt-3">
              <ChatInput onSend={handleSend} disabled={extracting} />
              {ready && (
                  <Button
                      onClick={handleEvaluate}
                      disabled={evaluating}
                      className="w-full mt-2.5"
                  >
                    {evaluating ? "Değerlendiriliyor…" : "Değerlendir"}
                  </Button>
              )}
            </div>
          </Card>

          <div className="space-y-4">
            {!result ? (
                <Card className="h-[calc(100vh-140px)] flex items-center justify-center">
                  <EmptyState
                      title="Sonuçlar burada görünecek"
                      description="Soldan bilgilerini paylaşıp değerlendirmeyi başlattığında, uygunluk analizi ve banka önerileri burada listelenecek."
                  />
                </Card>
            ) : result.isAllowed === false ? (
                <Card className="animate-in">
                  <Badge tone="danger">Reddedildi</Badge>
                  <p className="text-sm text-[var(--text-muted)] mt-3 leading-relaxed">
                    {result.rejectionReason}
                  </p>
                </Card>
            ) : result.termSuggestions && result.termSuggestions.length > 0 ? (
                <>
                  <Card className="animate-in">
                    <p className="text-sm text-[var(--text-muted)] leading-relaxed">{result.explanation}</p>
                  </Card>
                  <div className="grid sm:grid-cols-2 gap-4">
                    {result.termSuggestions.map((opt) => (
                        <TermOptionCard key={opt.termMonths} option={opt} />
                    ))}
                  </div>
                </>
            ) : (
                <>
                  <Card className="animate-in">
                    <RiskMeter level={result.riskLevel!} />
                  </Card>
                  <div className="grid grid-cols-2 sm:grid-cols-4 gap-4">
                    <StatCard label="Aylık Taksit" value={tl(result.estimatedMonthlyInstallment)} />
                    <StatCard label="Toplam Borç Yükü" value={tl(result.totalMonthlyDebtLoad)} />
                    <StatCard label="Borç/Gelir" value={`${(result.debtToIncomeRatio * 100).toFixed(1)}%`} />
                    <StatCard
                        label="Harcanabilir Gelir"
                        value={tl(result.disposableIncome)}
                        tone={result.disposableIncome >= 0 ? "success" : "danger"}
                    />
                  </div>
                  <Card className="animate-in">
                    <p className="text-sm text-[var(--text-muted)] leading-relaxed mb-4">{result.explanation}</p>
                    <p className="text-xs font-medium text-[var(--text-muted)] uppercase tracking-wide mb-2">
                      Uygun bankalar
                    </p>
                    {result.matchingBanks.length === 0 ? (
                        <p className="text-xs text-[var(--text-muted)] italic">Uygun banka bulunamadı.</p>
                    ) : (
                        result.matchingBanks.map((offer, i) => <BankCard key={i} offer={offer} />)
                    )}
                  </Card>
                </>
            )}
          </div>
        </main>
      </div>
  );
}