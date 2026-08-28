import { TermOption } from "@/types/finansal";
import Card from "@/components/ui/Card";
import Badge from "@/components/ui/Badge";
import BankCard from "./BankCard";
import { useState } from "react";

function tl(n: number) {
    return n.toLocaleString("tr-TR") + " TL";
}

const RISK_TONE = ["success", "success", "warning", "danger", "danger"] as const;
const RISK_LABEL = ["Güvenli", "Az Riskli", "Sınırda", "Riskli", "Çok Riskli"];

export default function TermOptionCard({ option }: { option: TermOption }) {
    const [expanded, setExpanded] = useState(false);
    const visibleBanks = expanded
        ? option.matchingBanks
        : option.matchingBanks.slice(0, 2);

    return (
        <Card>
            <div className="flex items-center justify-between mb-3">
                <div>
                    <p className="text-sm font-semibold text-[var(--text)]">
                        {option.termMonths} ay vade
                    </p>
                    <p className="font-num text-lg font-semibold text-[var(--text)]">
                        {tl(option.estimatedMonthlyInstallment)}
                        <span className="text-xs font-normal text-[var(--text-muted)]"> /ay</span>
                    </p>
                </div>
                <Badge tone={RISK_TONE[option.riskLevel]}>
                    {RISK_LABEL[option.riskLevel]}
                </Badge>
            </div>

            <p className="text-xs text-[var(--text-muted)] mb-3">
                Gelirin %{(option.debtToIncomeRatio * 100).toFixed(0)}'i borç ödemesine gidiyor
            </p>

            {option.matchingBanks.length === 0 ? (
                <p className="text-xs text-[var(--text-muted)] italic">
                    Bu vadede uygun banka bulunamadı.
                </p>
            ) : (
                <>
                    <div className="divide-y divide-[var(--border)]">
                        {visibleBanks.map((offer) => (
                            <BankCard key={offer.bank.id ?? offer.bank.name} offer={offer} />
                        ))}
                    </div>
                    {option.matchingBanks.length > 2 && (
                        <button
                            onClick={() => setExpanded((v) => !v)}
                            className="mt-2 text-xs font-medium text-[var(--accent)] hover:underline"
                        >
                            {expanded
                                ? "Daha az göster"
                                : `+${option.matchingBanks.length - 2} banka daha`}
                        </button>
                    )}
                </>
            )}
        </Card>
    );
}