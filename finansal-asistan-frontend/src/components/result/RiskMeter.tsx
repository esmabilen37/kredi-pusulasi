import { RiskLevel } from "@/types/finansal";

const LEVELS = [
    { level: RiskLevel.Guvenli, label: "Güvenli", color: "var(--success)" },
    { level: RiskLevel.AzRiskli, label: "Az Riskli", color: "var(--success)" },
    { level: RiskLevel.Sinirda, label: "Sınırda", color: "var(--warning)" },
    { level: RiskLevel.Riskli, label: "Riskli", color: "var(--danger)" },
    { level: RiskLevel.CokRiskli, label: "Çok Riskli", color: "var(--danger)" },
];

export default function RiskMeter({ level }: { level: RiskLevel }) {
    const current = LEVELS[level];
    return (
        <div>
            <div className="flex gap-1 mb-2">
                {LEVELS.map((l, i) => (
                    <div
                        key={l.level}
                        className="h-1.5 flex-1 rounded-full transition-colors"
                        style={{ backgroundColor: i <= level ? current.color : "var(--border)" }}
                    />
                ))}
            </div>
            <span className="text-sm font-semibold" style={{ color: current.color }}>
        {current.label}
      </span>
        </div>
    );
}