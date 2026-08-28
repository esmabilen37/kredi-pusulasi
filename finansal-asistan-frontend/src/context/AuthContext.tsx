"use client";

import { createContext, useContext, useState, useEffect, ReactNode } from "react";
import { refreshApi, logoutApi } from "@/lib/api";

interface AuthContextType {
    email: string | null;
    isAuthenticated: boolean;
    isCheckingSession: boolean;
    login: (email: string) => void;
    logout: () => Promise<void>;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
    const [email, setEmail] = useState<string | null>(null);
    const [isCheckingSession, setIsCheckingSession] = useState(true);

    // Sayfa ilk yüklendiğinde, refreshToken cookie'si hâlâ geçerliyse
    // otomatik olarak oturumu geri yükle (kullanıcı tekrar login olmak zorunda kalmasın)
    useEffect(() => {
        refreshApi()
            .then((res) => setEmail(res.email))
            .catch(() => setEmail(null))
            .finally(() => setIsCheckingSession(false));
    }, []);

    function login(newEmail: string) {
        setEmail(newEmail);
    }

    async function logout() {
        await logoutApi();
        setEmail(null);
    }

    return (
        <AuthContext.Provider
            value={{ email, isAuthenticated: !!email, isCheckingSession, login, logout }}
        >
            {children}
        </AuthContext.Provider>
    );
}

export function useAuth() {
    const ctx = useContext(AuthContext);
    if (!ctx) throw new Error("useAuth, AuthProvider içinde kullanılmalı");
    return ctx;
}