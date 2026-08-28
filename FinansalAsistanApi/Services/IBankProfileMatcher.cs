using FinansalAsistanApi.Models;

namespace FinansalAsistanApi.Services;

public interface IBankProfileMatcher
{
    Task<List<BankProfile>> GetMatchingBanksAsync(UserFinancialProfile profile, decimal debtToIncomeRatio);

    // Eligibility (uygunluk) aşamasında kullanılacak tahmini taksiti gerçekçi
    // hesaplayabilmek için, belirli kredi türündeki tüm bankaların ortalama aylık faiz oranı (her banka artık kredi türüne göre farklı faiz sunduğu için
    // tek bir genel ortalama anlamsız hale geldi).
    Task<decimal> GetAverageMonthlyInterestRateAsync(LoanType loanType);
}