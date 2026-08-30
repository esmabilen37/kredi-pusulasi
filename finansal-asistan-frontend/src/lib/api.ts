import {
    UserFinancialProfile,
    AffordabilityResult,
    ScenarioRequestDto,
    ScenarioResult,
    ChatMessage,
    AssistantResponseDto,
} from "@/types/finansal";

const API_BASE = process.env.NEXT_PUBLIC_API_BASE_URL ?? "http://localhost:5142";


const defaultOptions: RequestInit = { credentials: "include" };


async function fetchWithAuthRetry(url: string, options: RequestInit): Promise<Response> {
    const res = await fetch(url, options);

    if (res.status !== 401) {
        return res;
    }

    
    const refreshRes = await fetch(`${API_BASE}/api/Auth/refresh`, {
        ...defaultOptions,
        method: "POST",
    });

    if (!refreshRes.ok) {
       
        return res;
    }

    
    return fetch(url, options);
}

export async function evaluateAffordability(
    profile: UserFinancialProfile
): Promise<AffordabilityResult> {
    const res = await fetchWithAuthRetry(`${API_BASE}/api/Affordability/evaluate`, {
        ...defaultOptions,
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(profile),
    });
    if (!res.ok) throw new Error(`Değerlendirme başarısız (${res.status}): ${await res.text()}`);
    return res.json();
}

export async function runScenarios(request: ScenarioRequestDto): Promise<ScenarioResult[]> {
    const res = await fetchWithAuthRetry(`${API_BASE}/api/Affordability/scenarios`, {
        ...defaultOptions,
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(request),
    });
    if (!res.ok) throw new Error(`Senaryo hesaplama başarısız (${res.status}): ${await res.text()}`);
    return res.json();
}

export async function sendAssistantMessage(messages: ChatMessage[]): Promise<AssistantResponseDto> {
    const res = await fetchWithAuthRetry(`${API_BASE}/api/Assistant/message`, {
        ...defaultOptions,
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ messages }),
    });
    if (!res.ok) throw new Error(`Asistan isteği başarısız (${res.status}): ${await res.text()}`);
    return res.json();
}



interface AuthResponse {
    email: string;
}

export async function registerApi(email: string, password: string): Promise<AuthResponse> {
    const res = await fetch(`${API_BASE}/api/Auth/register`, {
        ...defaultOptions,
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email, password }),
    });
    if (!res.ok) {
        const err = await res.json().catch(() => ({ error: "Kayıt başarısız" }));
        throw new Error(err.error || "Kayıt başarısız");
    }
    return res.json();
}

export async function loginApi(email: string, password: string): Promise<AuthResponse> {
    const res = await fetch(`${API_BASE}/api/Auth/login`, {
        ...defaultOptions,
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email, password }),
    });
    if (!res.ok) {
        const err = await res.json().catch(() => ({ error: "Giriş başarısız" }));
        throw new Error(err.error || "Giriş başarısız");
    }
    return res.json();
}

export async function refreshApi(): Promise<AuthResponse> {
    const res = await fetch(`${API_BASE}/api/Auth/refresh`, {
        ...defaultOptions,
        method: "POST",
    });
    if (!res.ok) throw new Error("Oturum bulunamadı");
    return res.json();
}

export async function logoutApi(): Promise<void> {
    await fetch(`${API_BASE}/api/Auth/logout`, { ...defaultOptions, method: "POST" });
}