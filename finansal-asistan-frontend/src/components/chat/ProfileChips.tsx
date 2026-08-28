import { UserFinancialProfile } from "@/types/finansal";

function tl(n: number) {
    return n.toLocaleString("tr-TR") + " TL";
}

export default function ProfileChips({ profile }: { profile: UserFinancialProfile }) {
    const items: { label: string; value: string; filled: boolean }[] = [
        { label: "Gelir", value: profile.monthlyIncome ? tl(profile.monthlyIncome) : "—", filled: !!profile.monthlyIncome },
        { label: "Gider", value: profile.monthlyExpenses ? tl(profile.monthlyExpenses) : "—", filled: !!profile.monthlyExpenses },
        { label: "Talep tutar", value: profile.requestedLoanAmount ? tl(profile.requestedLoanAmount) : "—", filled: !!profile.requestedLoanAmount },
        { label: "Vade", value: profile.requestedTermMonths ? `${profile.requestedTermMonths} ay` : "Belirtilmedi", filled: !!profile.requestedTermMonths },
    ];

    return (
        <div className="flex flex-wrap gap-2 py-3">
            {items.map((item) => (
                <div
                    key={item.label}
                    className={`flex items-center gap-1.5 text-xs rounded-full px-3 py-1.5 border ${
                        item.filled
                            ? "border-[var(--accent)]/30 bg-[var(--accent-soft)] text-[var(--accent)]"
                            : "border-[var(--border)] bg-[var(--surface)] text-[var(--text-muted)]"
                    }`}
                >
                    <span className="font-medium">{item.label}:</span>
                    <span className="font-num">{item.value}</span>
                </div>
            ))}
        </div>
    );
}