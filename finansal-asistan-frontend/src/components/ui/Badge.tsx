type Tone = "success" | "warning" | "danger" | "neutral" | "accent";

const TONE_STYLES: Record<Tone, string> = {
    success: "bg-[var(--success-soft)] text-[var(--success)]",
    warning: "bg-[var(--warning-soft)] text-[var(--warning)]",
    danger: "bg-[var(--danger-soft)] text-[var(--danger)]",
    neutral: "bg-[var(--surface-hover)] text-[var(--text-muted)]",
    accent: "bg-[var(--accent-soft)] text-[var(--accent)]",
};

export default function Badge({ tone, children }: { tone: Tone; children: React.ReactNode }) {
    return (
        <span
            className={`inline-flex items-center gap-1.5 text-xs font-semibold px-2.5 py-1 rounded-full ${TONE_STYLES[tone]}`}
        >
      {children}
    </span>
    );
}