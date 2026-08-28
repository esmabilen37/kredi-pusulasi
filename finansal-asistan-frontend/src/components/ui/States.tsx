export function LoadingState({ label = "Yükleniyor…" }: { label?: string }) {
    return (
        <div className="flex items-center gap-3 py-6 px-1 animate-pulse-soft">
            <div className="w-2 h-2 rounded-full bg-[var(--accent)]" />
            <div className="w-2 h-2 rounded-full bg-[var(--accent)]" style={{ animationDelay: "0.15s" }} />
            <div className="w-2 h-2 rounded-full bg-[var(--accent)]" style={{ animationDelay: "0.3s" }} />
            <span className="text-sm text-[var(--text-muted)] ml-1">{label}</span>
        </div>
    );
}

export function EmptyState({ title, description }: { title: string; description: string }) {
    return (
        <div className="flex flex-col items-center text-center py-14 px-6">
            <div className="w-12 h-12 rounded-full bg-[var(--accent-soft)] flex items-center justify-center mb-4">
                <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="var(--accent)" strokeWidth="2">
                    <path d="M12 8v4M12 16h.01" strokeLinecap="round" />
                    <circle cx="12" cy="12" r="9" />
                </svg>
            </div>
            <p className="font-medium text-[var(--text)] mb-1">{title}</p>
            <p className="text-sm text-[var(--text-muted)] max-w-xs">{description}</p>
        </div>
    );
}

export function ErrorState({ message }: { message: string }) {
    return (
        <div className="flex items-start gap-3 rounded-xl border border-[var(--danger)]/20 bg-[var(--danger-soft)] p-4 text-sm text-[var(--danger)] animate-in">
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" className="mt-0.5 shrink-0">
                <circle cx="12" cy="12" r="9" />
                <path d="M12 8v5M12 16h.01" strokeLinecap="round" />
            </svg>
            <span>{message}</span>
        </div>
    );
}