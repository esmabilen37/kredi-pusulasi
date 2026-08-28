export enum LoanType {
    Ihtiyac = 0,
    Konut = 1,
    Tasit = 2,
}

export enum RiskLevel {
    Guvenli = 0,
    AzRiskli = 1,
    Sinirda = 2,
    Riskli = 3,
    CokRiskli = 4,
}

export interface UserFinancialProfile {
    loanType: LoanType;
    monthlyIncome: number;
    monthlyExpenses: number;
    requestedLoanAmount: number;
    existingMonthlyDebtPayment: number;
    requestedTermMonths: number;
}

export interface BankProfile {
    id?: string;
    name: string;
    maxDebtToIncomeRatio: number;
    minMonthlyIncome: number;
    maxLoanAmount: number;
    maxTermMonths: number;
    monthlyInterestRate: number;
    description: string;
}

export interface BankOffer {
    bank: BankProfile;
    monthlyInterestRate: number;
    monthlyInstallment: number;
    maxLoanAmount: number;
    maxTermMonths: number;
    totalRepayment: number;
    totalInterestPaid: number;
}

export interface TermOption {
    termMonths: number;
    estimatedMonthlyInstallment: number;
    totalMonthlyDebtLoad: number;
    debtToIncomeRatio: number;
    riskLevel: RiskLevel;
    matchingBanks: BankOffer[];
}

export interface AffordabilityResult {
    estimatedMonthlyInstallment: number;
    totalMonthlyDebtLoad: number;
    debtToIncomeRatio: number;
    disposableIncome: number;
    riskLevel: RiskLevel | null;
    explanation: string;
    isAllowed: boolean;
    rejectionReason: string | null;
    matchingBanks: BankOffer[];
    termSuggestions: TermOption[];
}

export interface ScenarioAdjustment {
    label: string;
    newRequestedLoanAmount?: number;
    newRequestedTermMonths?: number;
    incomeChangePercentage?: number;
    expenseChangePercentage?: number;
}

export interface ScenarioRequestDto {
    baseProfile: UserFinancialProfile;
    adjustments: ScenarioAdjustment[];
    includeStressTest: boolean;
}

export interface ScenarioResult {
    label: string;
    adjustedProfile: UserFinancialProfile;
    affordability: AffordabilityResult;
    recommendations: string[];
}

// Sohbet / birleşik asistan tipleri
export interface ChatMessage {
    role: "user" | "assistant";
    content: string;
}

export interface ChatRequestDto {
    messages: ChatMessage[];
}



export interface AssistantResponseDto {
    monthlyIncome: number;
    monthlyExpenses: number;
    requestedLoanAmount: number;
    requestedTermMonths: number;
    loanType: LoanType;
    reply: string;
}