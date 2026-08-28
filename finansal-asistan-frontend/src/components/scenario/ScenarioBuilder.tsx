"use client";

import { useState, useRef } from "react";
import Card from "@/components/ui/Card";
import Button from "@/components/ui/Button";
import { UserFinancialProfile, LoanType } from "@/types/finansal";

interface Props {
    baseProfile: UserFinancialProfile;
    onProfileChange: (profile: UserFinancialProfile) => void;
    onAddCustomScenario: (incomePct: number, expensePct: number) => void;
}

export default function ScenarioBuilder({ baseProfile, onProfileChange, onAddCustomScenario }: Props) {
    const [incomePct, setIncomePct] = useState(0);
    const [expensePct, setExpensePct] = useState(0);

    // Bileşen ilk yüklendiğindeki orijinal değerleri sakla (kıyaslama referansı)
    const originalProfile = useRef(baseProfile);

    function field<K extends keyof UserFinancialProfile>(key: K, value: string) {
        const parsed = value === "" ? 0 : Number(value);
        onProfileChange({ ...baseProfile, [key]: parsed });
    }

    const newIncome = Math.round(baseProfile.monthlyIncome * (1 + incomePct / 100));
    const newExpense = Math.round(baseProfile.monthlyExpenses * (1 + expensePct / 100));

    // Üstteki kutulardan elle yapılan değişikliği, orijinale göre yüzdeye çevir
    function manualPct(current: number, original: number): number {
        if (original === 0) return current === 0 ? 0 : 100;
        return Math.round(((current - original) / original) * 100);
    }

    const manualIncomePct = manualPct(baseProfile.monthlyIncome, originalProfile.current.monthlyIncome);
    const manualExpensePct = manualPct(baseProfile.monthlyExpenses, originalProfile.current.monthlyExpenses);

    // Slider'dan mı, üstteki kutudan mı değişiklik var — hangisi varsa onu kullan
    const effectiveIncomePct = incomePct !== 0 ? incomePct : manualIncomePct;
    const effectiveExpensePct = expensePct !== 0 ? expensePct : manualExpensePct;

    const canAddScenario = effectiveIncomePct !== 0 || effectiveExpensePct !== 0;

    function handleAddScenario() {
        onAddCustomScenario(effectiveIncomePct, effectiveExpensePct);
        setIncomePct(0);
        setExpensePct(0);
        // Yeni eklenen senaryo sonrası referansı güncelle ki bir sonraki kıyaslama sıfırdan başlasın
        originalProfile.current = baseProfile;
    }

    return (
        <div className="space-y-4">
            <Card>
                <p className="text-sm font-semibold mb-3">Mevcut profilin</p>
                <div className="grid grid-cols-2 sm:grid-cols-3 gap-3">
                    <label className="text-xs text-[var(--text-muted)]">
                        Aylık Gelir
                        <input
                            type="number"
                            className="w-full mt-1 border border-[var(--border)] rounded-md px-2 py-1.5 font-num text-sm"
                            value={baseProfile.monthlyIncome === 0 ? "" : baseProfile.monthlyIncome}
                            placeholder="0"
                            onFocus={(e) => e.target.select()}
                            onChange={(e) => field("monthlyIncome", e.target.value)}
                        />
                    </label>
                    <label className="text-xs text-[var(--text-muted)]">
                        Aylık Gider
                        <input
                            type="number"
                            className="w-full mt-1 border border-[var(--border)] rounded-md px-2 py-1.5 font-num text-sm"
                            value={baseProfile.monthlyExpenses === 0 ? "" : baseProfile.monthlyExpenses}
                            placeholder="0"
                            onFocus={(e) => e.target.select()}
                            onChange={(e) => field("monthlyExpenses", e.target.value)}
                        />
                    </label>
                    <label className="text-xs text-[var(--text-muted)]">
                        Mevcut Borç
                        <input
                            type="number"
                            className="w-full mt-1 border border-[var(--border)] rounded-md px-2 py-1.5 font-num text-sm"
                            value={baseProfile.existingMonthlyDebtPayment === 0 ? "" : baseProfile.existingMonthlyDebtPayment}
                            placeholder="0"
                            onFocus={(e) => e.target.select()}
                            onChange={(e) => field("existingMonthlyDebtPayment", e.target.value)}
                        />
                    </label>
                    <label className="text-xs text-[var(--text-muted)]">
                        Talep Tutarı
                        <input
                            type="number"
                            className="w-full mt-1 border border-[var(--border)] rounded-md px-2 py-1.5 font-num text-sm"
                            value={baseProfile.requestedLoanAmount === 0 ? "" : baseProfile.requestedLoanAmount}
                            placeholder="0"
                            onFocus={(e) => e.target.select()}
                            onChange={(e) => field("requestedLoanAmount", e.target.value)}
                        />
                    </label>
                    <label className="text-xs text-[var(--text-muted)]">
                        Vade (ay)
                        <input
                            type="number"
                            className="w-full mt-1 border border-[var(--border)] rounded-md px-2 py-1.5 font-num text-sm"
                            value={baseProfile.requestedTermMonths === 0 ? "" : baseProfile.requestedTermMonths}
                            placeholder="0"
                            onFocus={(e) => e.target.select()}
                            onChange={(e) => field("requestedTermMonths", e.target.value)}
                        />
                    </label>
                    <label className="text-xs text-[var(--text-muted)]">
                        Kredi Türü
                        <select
                            className="w-full mt-1 border border-[var(--border)] rounded-md px-2 py-1.5 text-sm"
                            value={baseProfile.loanType}
                            onChange={(e) => field("loanType", e.target.value)}
                        >
                            <option value={LoanType.Ihtiyac}>İhtiyaç</option>
                            <option value={LoanType.Konut}>Konut</option>
                            <option value={LoanType.Tasit}>Taşıt</option>
                        </select>
                    </label>
                </div>
            </Card>

            <Card>
                <p className="text-sm font-semibold mb-3">Kendi senaryonu oluştur</p>

                <div className="mb-4">
                    <div className="flex justify-between text-xs mb-1">
                        <span>Gelir değişimi</span>
                        <span className="font-num font-semibold text-[var(--success)]">{incomePct >= 0 ? "+" : ""}{incomePct}%</span>
                    </div>
                    <input type="range" min={-100} max={100} value={incomePct}
                           onChange={(e) => setIncomePct(Number(e.target.value))} className="w-full" />
                    <p className="text-xs font-num text-[var(--text-muted)] mt-1">
                        {baseProfile.monthlyIncome.toLocaleString("tr-TR")} TL → {newIncome.toLocaleString("tr-TR")} TL
                    </p>
                </div>

                <div className="mb-4">
                    <div className="flex justify-between text-xs mb-1">
                        <span>Gider değişimi</span>
                        <span className="font-num font-semibold text-[var(--danger)]">{expensePct >= 0 ? "+" : ""}{expensePct}%</span>
                    </div>
                    <input type="range" min={-100} max={100} value={expensePct}
                           onChange={(e) => setExpensePct(Number(e.target.value))} className="w-full" />
                    <p className="text-xs font-num text-[var(--text-muted)] mt-1">
                        {baseProfile.monthlyExpenses.toLocaleString("tr-TR")} TL → {newExpense.toLocaleString("tr-TR")} TL
                    </p>
                </div>

                <Button
                    onClick={handleAddScenario}
                    disabled={!canAddScenario}
                    className="w-full"
                >
                    + Senaryo olarak ekle
                </Button>
                {!canAddScenario && (
                    <p className="text-xs text-[var(--text-muted)] mt-2 text-center">
                        Senaryo eklemek için gelir/gider kaydırıcılarını ya da yukarıdaki gelir/gider alanlarını değiştir.
                    </p>
                )}
            </Card>
        </div>
    );
}