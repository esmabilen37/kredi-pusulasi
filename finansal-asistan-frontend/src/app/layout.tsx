import type { Metadata } from "next";
import "./globals.css";
import { AuthProvider } from "@/context/AuthContext";
import { ProfileProvider } from "@/context/ProfileContext";

export const metadata: Metadata = {
    title: "Kredi Pusulası",
    description: "Bütçeni anlat, uygunluk değerlendirmesini gör.",
};

export default function RootLayout({ children }: { children: React.ReactNode }) {
    return (
        <html lang="tr">
        <body>
        <AuthProvider>
            <ProfileProvider>{children}</ProfileProvider>
        </AuthProvider>
        </body>
        </html>
    );
}