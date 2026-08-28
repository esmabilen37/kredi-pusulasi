"use client";

import { createContext, useContext, useState, ReactNode } from "react";
import { UserFinancialProfile } from "@/types/finansal";
import { EMPTY_PROFILE } from "@/lib/mergeProfile";

interface ProfileContextType {
    profile: UserFinancialProfile;
    setProfile: (p: UserFinancialProfile) => void;
}

const ProfileContext = createContext<ProfileContextType | undefined>(undefined);

export function ProfileProvider({ children }: { children: ReactNode }) {
    const [profile, setProfile] = useState<UserFinancialProfile>(EMPTY_PROFILE);
    return (
        <ProfileContext.Provider value={{ profile, setProfile }}>
            {children}
        </ProfileContext.Provider>
    );
}

export function useProfile() {
    const ctx = useContext(ProfileContext);
    if (!ctx) throw new Error("useProfile, ProfileProvider içinde kullanılmalı");
    return ctx;
}