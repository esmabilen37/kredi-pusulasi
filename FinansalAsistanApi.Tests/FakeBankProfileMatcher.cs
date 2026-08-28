using FinansalAsistanApi.Models;
using FinansalAsistanApi.Services;

namespace FinansalAsistanApi.Tests;

public class FakeBankProfileMatcher : IBankProfileMatcher
{
    public Task<List<BankProfile>> GetMatchingBanksAsync(UserFinancialProfile profile, decimal debtToIncomeRatio)
    {
        return Task.FromResult(new List<BankProfile>());
    }

    public Task<decimal> GetAverageMonthlyInterestRateAsync(LoanType loanType)
    {
        // 0 döndürüyoruz: mevcut risk-seviyesi sınır testleri (%30/%40/%50/%65) tam sınır
        // değerlerini hedefleyerek düz bölme varsayımıyla yazılmış.
        return Task.FromResult(0m);
    }
}