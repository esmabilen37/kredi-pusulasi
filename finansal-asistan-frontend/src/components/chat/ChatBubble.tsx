export type ChatRole = "user" | "assistant";

export function ChatBubble({ role, children }: { role: ChatRole; children: React.ReactNode }) {
    const isUser = role === "user";
    return (
        <div className={`flex ${isUser ? "justify-end" : "justify-start"} animate-in`}>
            <div
                className={`max-w-[85%] rounded-2xl px-4 py-3 text-sm leading-relaxed ${
                    isUser
                        ? "bg-[var(--accent)] text-white rounded-br-md"
                        : "bg-[var(--surface)] border border-[var(--border)] text-[var(--text)] rounded-bl-md"
                }`}
            >
                {children}
            </div>
        </div>
    );
}