"use client";

import { useState, FormEvent } from "react";
import { useRouter } from "next/navigation";
import Card from "@/components/ui/Card";
import Button from "@/components/ui/Button";
import { useAuth } from "@/context/AuthContext";
import { loginApi, registerApi } from "@/lib/api";

type Tab = "login" | "register";

export default function LoginPage() {
    const [tab, setTab] = useState<Tab>("login");
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const { login } = useAuth();
    const router = useRouter();

    async function handleSubmit(e: FormEvent) {
        e.preventDefault();
        setError(null);
        setLoading(true);

        try {
            const result =
                tab === "login" ? await loginApi(email, password) : await registerApi(email, password);
            login(result.email);
            router.push("/");
        } catch (err) {
            setError(err instanceof Error ? err.message : "Bir şeyler ters gitti");
        } finally {
            setLoading(false);
        }
    }

    function switchTab(next: Tab) {
        setTab(next);
        setError(null);
    }

    return (
        <div className="min-h-screen flex items-center justify-center px-6">
            <div className="w-full max-w-sm">
                <div className="text-center mb-6">
                    <div className="w-10 h-10 rounded-lg bg-[var(--accent)] flex items-center justify-center text-white font-bold text-sm mx-auto mb-3">
                        KP
                    </div>
                    <h1 className="text-lg font-semibold text-[var(--text)]">Kredi Pusulası</h1>
                </div>

                <Card>
                    <div className="flex mb-5 border border-[var(--border)] rounded-lg p-1">
                        <button
                            type="button"
                            onClick={() => switchTab("login")}
                            className={`flex-1 text-sm font-medium py-1.5 rounded-md transition-colors ${
                                tab === "login"
                                    ? "bg-[var(--accent)] text-white"
                                    : "text-[var(--text-muted)]"
                            }`}
                        >
                            Giriş Yap
                        </button>
                        <button
                            type="button"
                            onClick={() => switchTab("register")}
                            className={`flex-1 text-sm font-medium py-1.5 rounded-md transition-colors ${
                                tab === "register"
                                    ? "bg-[var(--accent)] text-white"
                                    : "text-[var(--text-muted)]"
                            }`}
                        >
                            Kayıt Ol
                        </button>
                    </div>

                    <form onSubmit={handleSubmit} className="space-y-3.5">
                        <label className="block text-xs text-[var(--text-muted)]">
                            E-posta
                            <input
                                type="email"
                                required
                                autoComplete="email"
                                className="w-full mt-1 border border-[var(--border)] rounded-md px-3 py-2 text-sm"
                                value={email}
                                onChange={(e) => setEmail(e.target.value)}
                                placeholder="ornek@mail.com"
                            />
                        </label>

                        <label className="block text-xs text-[var(--text-muted)]">
                            Şifre
                            <input
                                type="password"
                                required
                                minLength={6}
                                autoComplete={tab === "login" ? "current-password" : "new-password"}
                                className="w-full mt-1 border border-[var(--border)] rounded-md px-3 py-2 text-sm"
                                value={password}
                                onChange={(e) => setPassword(e.target.value)}
                                placeholder="••••••••"
                            />
                        </label>

                        {error && (
                            <p className="text-xs text-[var(--danger)] bg-[var(--danger)]/10 rounded-md px-3 py-2">
                                {error}
                            </p>
                        )}

                        <Button type="submit" disabled={loading} className="w-full">
                            {loading
                                ? "Bekleyin..."
                                : tab === "login"
                                    ? "Giriş Yap"
                                    : "Hesap Oluştur"}
                        </Button>
                    </form>
                </Card>
            </div>
        </div>
    );
}