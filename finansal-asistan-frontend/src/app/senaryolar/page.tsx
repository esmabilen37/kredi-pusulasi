"use client";

import { useState, useEffect, useCallback } from "react";
import { useRouter } from "next/navigation";
import { runScenarios } from "@/lib/api";
import { UserFinancialProfile, ScenarioResult, RiskLevel } from "@/types/finansal";
import { useProfile } from "@/context/ProfileContext";
import { useAuth } from "@/context/AuthContext";
import { isProfileReadyForEvaluation } from "@/lib/mergeProfile";

import Card from "@/components/ui/Card";
import Badge from "@/components/ui/Badge";
import { LoadingState, ErrorState, EmptyState } from "@/components/ui/States";
import RiskMeter from "@/components/result/RiskMeter";
import StatCard from "@/components/result/StatCard";
import BankCard from "@/components/result/BankCard";
import ScenarioBuilder from "@/components/scenario/ScenarioBuilder";
import { BarChart3 } from "lucide-react";
import Button from "@/components/ui/Button";

function tl(n: number) {
    return Math.round(n).toLocaleString("tr-TR") + " TL";
}

export default function SenaryolarPage() {
    const router = useRouter();
    const { isAuthenticated, isCheckingSession } = useAuth();

    const { profile: sharedProfile } = useProfile();
    const [baseProfile, setBaseProfile] = useState<UserFinancialProfile>(sharedProfile);
    const [scenarios, setScenarios] = useState<ScenarioResult[]>([]);
    const [activeIndex, setActiveIndex] = useState(0);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [showAllRecs, setShowAllRecs] = useState(false);

    // Senaryolar en son hangi profille hesaplandıysa onu tutuyoruz — kullanıcı
    // baseProfile'ı (talep tutarı, vade vb.) değiştirdiğinde bunun farkına
    // varıp "sonuçlar artık güncel değil" uyarısı gösterebilmek için.
    const [lastLoadedProfile, setLastLoadedProfile] = useState<UserFinancialProfile | null>(null);

    useEffect(() => {
        if (!isCheckingSession && !isAuthenticated) {
            router.push("/login");
        }
    }, [isCheckingSession, isAuthenticated, router]);

    const load = useCallback(async (profile: UserFinancialProfile) => {
        setLoading(true);
        setError(null);
        try {
            const results = await runScenarios({
                baseProfile: profile,
                adjustments: [],
                includeStressTest: true,
            });
            setScenarios(results);
            setActiveIndex(0);
            setLastLoadedProfile(profile);
        } catch (e) {
            setError(e instanceof Error ? e.message : "Bilinmeyen hata");
        } finally {
            setLoading(false);
        }
    }, []);

    useEffect(() => {
        if (isProfileReadyForEvaluation(sharedProfile) && sharedProfile.requestedTermMonths > 0) {
            load(sharedProfile);
        }
    }, []); // eslint-disable-line react-hooks/exhaustive-deps

    async function addCustomScenario(incomePct: number, expensePct: number) {
        setLoading(true);
        setError(null);
        try {
            const results = await runScenarios({
                baseProfile,
                adjustments: [{
                    label: `Özel: Gelir ${incomePct >= 0 ? "+" : ""}${incomePct}%, Gider ${expensePct >= 0 ? "+" : ""}${expensePct}%`,
                    incomeChangePercentage: incomePct,
                    expenseChangePercentage: expensePct,
                }],
                includeStressTest: false,
            });
            setScenarios((prev) => [...prev, ...results]);
            setActiveIndex(scenarios.length);
        } catch (e) {
            setError(e instanceof Error ? e.message : "Bilinmeyen hata");
        } finally {
            setLoading(false);
        }
    }

    const active = scenarios[activeIndex];
    const ready = isProfileReadyForEvaluation(baseProfile) && baseProfile.requestedTermMonths > 0;

    // baseProfile, en son hesaplanan profilden farklıysa, kullanıcı ana
    // formu (talep tutarı, vade, gelir vb.) değiştirmiş ama henüz yeniden
    // hesaplamamış demektir.
    const isDirty =
        lastLoadedProfile !== null &&
        JSON.stringify(baseProfile) !== JSON.stringify(lastLoadedProfile);

    useEffect(() => {
        setShowAllRecs(false);
    }, [activeIndex]);

    if (isCheckingSession || !isAuthenticated) {
        return (
            <div className="min-h-screen flex items-center justify-center">
                <LoadingState label="Yükleniyor…" />
            </div>
        );
    }

    return (
        <div className="max-w-5xl mx-auto p-6 space-y-6">
            <div>
                <h1 className="text-2xl font-semibold">Senaryonu seç, sonucu gör</h1>
                <p className="text-sm text-[var(--text-muted)] mt-1">
                    Gelir/gider değişimlerini ve stres testi sonuçlarını karşılaştır.
                </p>
            </div>

            <ScenarioBuilder
                baseProfile={baseProfile}
                onProfileChange={(p) => { setBaseProfile(p); }}
                onAddCustomScenario={addCustomScenario}
            />

            {ready && scenarios.length > 0 && isDirty && (
                <div className="flex items-center justify-between gap-3 bg-[var(--warning-soft)] border border-[var(--warning)]/30 rounded-lg px-4 py-3">
                    <p className="text-sm text-[var(--text-muted)]">
                        Profil bilgilerini değiştirdin. Güncel sonuçları görmek için yeniden hesapla.
                    </p>
                    <Button onClick={() => load(baseProfile)} disabled={loading} className="whitespace-nowrap">
                        Yeniden Hesapla
                    </Button>
                </div>
            )}

            {ready && scenarios.length === 0 && !loading && (
                <Button
                    onClick={() => load(baseProfile)}
                    className="w-full py-3.5 text-base font-semibold flex items-center justify-center gap-2 shadow-md hover:shadow-lg transition-shadow"
                >
                    <BarChart3 size={18} />
                    Senaryoları Hesapla
                </Button>
            )}

            {!ready && scenarios.length === 0 && (
                <EmptyState
                    title="Profil eksik"
                    description={
                        !baseProfile.requestedLoanAmount && !baseProfile.requestedTermMonths
                            ? "Senaryoları hesaplamak için talep tutarı ve vade (ay) girmen gerekiyor."
                            : !baseProfile.requestedLoanAmount
                                ? "Senaryoları hesaplamak için talep tutarı girmen gerekiyor."
                                : "Senaryoları hesaplamak için vade (ay) girmen gerekiyor."
                    }
                />
            )}

            {error && <ErrorState message={error} />}
            {loading && <LoadingState label="Hesaplanıyor…" />}

            {scenarios.length > 0 && (
                <>
                    <div className="flex gap-2 overflow-x-auto pb-1">
                        {scenarios.map((s, i) => (
                            <button
                                key={i}
                                onClick={() => setActiveIndex(i)}
                                className={`flex-shrink-0 px-3 py-2 rounded-lg text-sm border transition-colors ${
                                    i === activeIndex
                                        ? "bg-[var(--accent)] text-white border-[var(--accent)]"
                                        : "border-[var(--border)] text-[var(--text-muted)] hover:border-[var(--accent)]"
                                }`}
                            >
                                {s.label}
                            </button>
                        ))}
                    </div>

                    {active && (
                        <div className="space-y-4">
                            {!active.affordability.isAllowed ? (
                                <Card>
                                    <Badge tone="danger">Reddedildi</Badge>
                                    <p className="text-sm text-[var(--text-muted)] mt-3">{active.affordability.rejectionReason}</p>
                                </Card>
                            ) : (
                                <>
                                    <Card>
                                        {active.affordability.riskLevel !== null ? (
                                            <RiskMeter level={active.affordability.riskLevel} />
                                        ) : (
                                            <p className="text-sm text-[var(--text-muted)]">
                                                Bu senaryo için risk seviyesi hesaplanamadı (vade eksik).
                                            </p>
                                        )}
                                        <p className="text-sm text-[var(--text-muted)] mt-4 leading-relaxed">
                                            {active.affordability.explanation}
                                        </p>
                                    </Card>

                                    <div className="grid grid-cols-2 sm:grid-cols-4 gap-4">
                                        <StatCard label="Aylık Gelir" value={tl(active.adjustedProfile.monthlyIncome)} />
                                        <StatCard label="Aylık Gider" value={tl(active.adjustedProfile.monthlyExpenses)} />
                                        <StatCard label="Tahmini Taksit" value={tl(active.affordability.estimatedMonthlyInstallment)} />
                                        <StatCard
                                            label="Harcanabilir Gelir"
                                            value={tl(active.affordability.disposableIncome)}
                                            tone={active.affordability.disposableIncome >= 0 ? "success" : "danger"}
                                        />
                                    </div>

                                    <Card>
                                        <p className="text-xs font-medium text-[var(--text-muted)] uppercase tracking-wide mb-2">
                                            Uygun bankalar ({active.affordability.matchingBanks.length})
                                        </p>
                                        {active.affordability.matchingBanks.length === 0 ? (
                                            <p className="text-xs text-[var(--text-muted)] italic">Uygun banka bulunamadı.</p>
                                        ) : (
                                            active.affordability.matchingBanks.map((offer, i) => <BankCard key={i} offer={offer} />)
                                        )}
                                    </Card>

                                    {active.recommendations.length > 0 && (
                                        <Card>
                                            <p className="text-xs font-medium text-[var(--text-muted)] uppercase tracking-wide mb-2">
                                                Öneriler ({active.recommendations.length})
                                            </p>
                                            <div className="space-y-2">
                                                {(showAllRecs ? active.recommendations : active.recommendations.slice(0, 5)).map((r, i) => (
                                                    <div key={i} className="text-sm bg-[var(--warning-soft)] border border-[var(--warning)]/30 rounded-lg px-3 py-2">
                                                        {r}
                                                    </div>
                                                ))}
                                            </div>
                                            {active.recommendations.length > 5 && (
                                                <button
                                                    onClick={() => setShowAllRecs((prev) => !prev)}
                                                    className="text-sm text-[var(--accent)] hover:underline mt-3"
                                                >
                                                    {showAllRecs
                                                        ? "Daha az göster"
                                                        : `+${active.recommendations.length - 5} öneri daha göster`}
                                                </button>
                                            )}
                                        </Card>
                                    )}
                                </>
                            )}
                        </div>
                    )}
                </>
            )}
        </div>
    );
}