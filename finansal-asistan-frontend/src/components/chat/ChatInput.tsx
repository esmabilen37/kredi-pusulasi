import { useState, KeyboardEvent } from "react";
import Button from "@/components/ui/Button";

export default function ChatInput({
                                      onSend,
                                      disabled,
                                  }: {
    onSend: (text: string) => void;
    disabled?: boolean;
}) {
    const [value, setValue] = useState("");

    function handleSend() {
        if (!value.trim()) return;
        onSend(value.trim());
        setValue("");
    }

    function handleKeyDown(e: KeyboardEvent<HTMLTextAreaElement>) {
        if (e.key === "Enter" && !e.shiftKey) {
            e.preventDefault();
            handleSend();
        }
    }

    return (
        <div className="flex items-end gap-2 border border-[var(--border)] bg-[var(--surface)] rounded-2xl p-2 shadow-sm focus-within:ring-2 focus-within:ring-[var(--accent)]/30">
      <textarea
          value={value}
          onChange={(e) => setValue(e.target.value)}
          onKeyDown={handleKeyDown}
          disabled={disabled}
          rows={1}
          placeholder="Örn: Aylık gelirim 45000 TL, 100000 TL kredi istiyorum…"
          className="flex-1 resize-none bg-transparent outline-none text-sm py-2 px-2 max-h-32"
      />
            <Button onClick={handleSend} disabled={disabled || !value.trim()} className="shrink-0">
                Gönder
            </Button>
        </div>
    );
}