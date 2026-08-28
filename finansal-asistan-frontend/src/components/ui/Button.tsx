import { ButtonHTMLAttributes, ReactNode } from "react";

interface ButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
    variant?: "primary" | "secondary" | "ghost";
    children: ReactNode;
}

export default function Button({
                                   variant = "primary",
                                   children,
                                   className = "",
                                   disabled,
                                   ...rest
                               }: ButtonProps) {
    const base =
        "inline-flex items-center justify-center gap-2 text-sm font-medium rounded-lg px-4 py-2.5 transition-all duration-150 disabled:opacity-50 disabled:cursor-not-allowed";

    const variants: Record<string, string> = {
        primary:
            "bg-[var(--accent)] text-white hover:brightness-110 active:scale-[0.98] shadow-sm",
        secondary:
            "bg-[var(--surface)] text-[var(--text)] border border-[var(--border)] hover:bg-[var(--surface-hover)] active:scale-[0.98]",
        ghost: "text-[var(--text-muted)] hover:text-[var(--text)] hover:bg-[var(--surface-hover)]",
    };

    return (
        <button
            className={`${base} ${variants[variant]} ${className}`}
            disabled={disabled}
            {...rest}
        >
            {children}
        </button>
    );
}