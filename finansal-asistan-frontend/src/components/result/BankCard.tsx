import { BankOffer } from "@/types/finansal";

function tl(n: number) {
    return Math.round(n).toLocaleString("tr-TR") + " TL";
}

export default function BankCard({ offer }: { offer: BankOffer }) {
    const { bank } = offer;

    return (
        <div className="flex items-center justify-between py-3 px-3.5 rounded-xl hover:bg-[var(--surface-hover)] transition-colors">
            <div className="flex items-center gap-3">
                <div className="w-8 h-8 rounded-lg bg-[var(--accent-soft)] flex items-center justify-center text-[var(--accent)] font-semibold text-xs shrink-0">
                    {bank.name.charAt(0)}
                </div>
                <div>
                    <p className="text-sm font-medium text-[var(--text)]">{bank.name}</p>
                    <p className="text-xs text-[var(--text-muted)]">
                        Aylık faiz %{offer.monthlyInterestRate.toFixed(2)} · Maks. {tl(offer.maxLoanAmount)}
                    </p>
                </div>
            </div>
            <div className="text-right">
                <p className="font-num text-sm font-semibold text-[var(--text)]">
                    {tl(offer.monthlyInstallment)}
                    <span className="text-xs font-normal text-[var(--text-muted)]"> /ay</span>
                </p>
                <p className="text-xs text-[var(--text-muted)]">
                    Toplam {tl(offer.totalRepayment)}
                </p>
            </div>
        </div>
    );
}