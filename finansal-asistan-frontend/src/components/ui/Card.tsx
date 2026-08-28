import { ReactNode } from "react";

export default function Card({
                                 children,
                                 className = "",
                                 hover = false,
                             }: {
    children: ReactNode;
    className?: string;
    hover?: boolean;
}) {
    return (
        <div
            className={`bg-[var(--surface)] border border-[var(--border)] rounded-2xl p-5 ${
                hover ? "transition-shadow hover:shadow-md" : ""
            } ${className}`}
        >
            {children}
        </div>
    );
}