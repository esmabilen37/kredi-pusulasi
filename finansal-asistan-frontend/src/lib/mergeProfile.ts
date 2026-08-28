import { UserFinancialProfile } from "@/types/finansal";

export const EMPTY_PROFILE: UserFinancialProfile = {
    loanType: 0,
    monthlyIncome: 0,
    monthlyExpenses: 0,
    requestedLoanAmount: 0,
    existingMonthlyDebtPayment: 0,
    requestedTermMonths: 0,
};

/**
 * Yeni ayrıştırılan profili, önceki (birikmiş) profille birleştirir.
 * Yeni mesajda "0" gelen alanlar, eski değerini korur — böylece kullanıcı
 * parça parça bilgi verse bile sistem "hatırlıyormuş" gibi davranır.
 */
export function mergeProfiles(
    previous: UserFinancialProfile,
    incoming: UserFinancialProfile
): UserFinancialProfile {
    return {
        loanType: incoming.loanType !== 0 ? incoming.loanType : previous.loanType,
        monthlyIncome: incoming.monthlyIncome || previous.monthlyIncome,
        monthlyExpenses: incoming.monthlyExpenses || previous.monthlyExpenses,
        requestedLoanAmount: incoming.requestedLoanAmount || previous.requestedLoanAmount,
        existingMonthlyDebtPayment:
            incoming.existingMonthlyDebtPayment || previous.existingMonthlyDebtPayment,
        requestedTermMonths: incoming.requestedTermMonths || previous.requestedTermMonths,
    };
}

/** Profilin "değerlendirmeye yetecek kadar" dolu olup olmadığını kontrol eder. */
export function isProfileReadyForEvaluation(profile: UserFinancialProfile): boolean {
    return profile.requestedLoanAmount > 0;
}