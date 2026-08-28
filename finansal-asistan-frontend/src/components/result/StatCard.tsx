import Card from "@/components/ui/Card";

const toneColors: Record<string, string> = {
    success: "var(--success)",
    danger: "var(--danger)",
    default: "var(--text)",
};

export default function StatCard({
                                     label,
                                     value,
                                     sublabel,
                                     tone = "default",
                                 }: {
    label: string;
    value: string;
    sublabel?: string;
    tone?: "success" | "danger" | "default";
}) {
    return (
        <Card hover>
            <p className="text-xs font-medium text-[var(--text-muted)] uppercase tracking-wide mb-2">
                {label}
            </p>
            <p className="font-num text-2xl font-semibold" style={{ color: toneColors[tone] }}>
                {value}
            </p>
            {sublabel && <p className="text-xs text-[var(--text-muted)] mt-1">{sublabel}</p>}
        </Card>
    );
}