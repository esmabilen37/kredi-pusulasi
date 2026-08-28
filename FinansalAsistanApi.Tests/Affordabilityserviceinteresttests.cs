using FinansalAsistanApi.Models;
using FinansalAsistanApi.Services;
using Xunit;

namespace FinansalAsistanApi.Tests;

/// <summary>
/// Bilinen bir faiz oranına sahip tek bankalı sahte matcher.
/// Amaç: CalculateInstallment (annuite formülü) uçtan uca EvaluateAsync üzerinden test edilebilsin.
/// </summary>
public class FakeBankProfileMatcherWithBank : IBankProfileMatcher
{
    private readonly BankProfile _bank;
    private readonly LoanType _loanType;

    public FakeBankProfileMatcherWithBank(BankProfile bank, LoanType loanType)
    {
        _bank = bank;
        _loanType = loanType;
    }

    public Task<List<BankProfile>> GetMatchingBanksAsync(UserFinancialProfile profile, decimal debtToIncomeRatio)
    {
        return Task.FromResult(new List<BankProfile> { _bank });
    }

    public Task<decimal> GetAverageMonthlyInterestRateAsync(LoanType loanType)
    {
        var offer = _bank.GetOfferFor(_loanType);
        return Task.FromResult(offer?.MonthlyInterestRate ?? 0m);
    }
}

public class AffordabilityServiceInterestTests
{
    [Fact]
    public async Task Evaluate_BilinenFaizVeVade_DogruTaksitiHesaplar()
    {
        // Arrange
        // 100.000 TL anapara, aylık %2 faiz, 12 ay vade
        // Standart amortisman tablosu faktörü (A/P, n=12, i=%2) ~ 0.094560
        // Beklenen taksit ~ 100.000 * 0.094560 = 9.456 TL (referans/elle hesap)
        var bank = new BankProfile
        {
            Name = "Test Bankası",
            MaxDebtToIncomeRatio = 1m, // testte eleme yapmasın diye gevşek bıraktım
            MinMonthlyIncome = 0m,
            Description = "Test amaçlı",
            LoanTypeOffers = new List<LoanTypeOffer>
            {
                new() { LoanType = LoanType.Konut, MonthlyInterestRate = 2.00m, MaxLoanAmount = 1_000_000m, MaxTermMonths = 60 }
            }
        };

        var matcher = new FakeBankProfileMatcherWithBank(bank, LoanType.Konut);
        var service = new AffordabilityService(matcher);

        var profile = new UserFinancialProfile
        {
            LoanType = LoanType.Konut,
            MonthlyIncome = 100_000m,
            MonthlyExpenses = 0m,
            RequestedLoanAmount = 100_000m,
            ExistingMonthlyDebtPayment = 0m,
            RequestedTermMonths = 12
        };

        // Act
        var result = await service.EvaluateAsync(profile);

        // Assert
        Assert.True(result.IsAllowed);
        Assert.Single(result.MatchingBanks);

        var offer = result.MatchingBanks[0];
        var expectedInstallment = 9456.0m;

        Assert.True(
            Math.Abs(offer.MonthlyInstallment - expectedInstallment) < 1m,
            $"Beklenen ~{expectedInstallment} TL, gerçek {offer.MonthlyInstallment} TL"
        );

        Assert.Equal(offer.MonthlyInstallment * 12, offer.TotalRepayment);
        Assert.Equal(offer.TotalRepayment - profile.RequestedLoanAmount, offer.TotalInterestPaid);
        Assert.True(offer.TotalInterestPaid > 0m);
    }

    [Fact]
    public async Task Evaluate_SifirFaiz_DuzBolmeyeDuser()
    {
        var bank = new BankProfile
        {
            Name = "Faizsiz Test Bankası",
            MaxDebtToIncomeRatio = 1m,
            MinMonthlyIncome = 0m,
            Description = "Test amaçlı",
            LoanTypeOffers = new List<LoanTypeOffer>
            {
                new() { LoanType = LoanType.Konut, MonthlyInterestRate = 0m, MaxLoanAmount = 1_000_000m, MaxTermMonths = 60 }
            }
        };

        var matcher = new FakeBankProfileMatcherWithBank(bank, LoanType.Konut);
        var service = new AffordabilityService(matcher);

        var profile = new UserFinancialProfile
        {
            LoanType = LoanType.Konut,
            MonthlyIncome = 100_000m,
            MonthlyExpenses = 0m,
            RequestedLoanAmount = 120_000m,
            ExistingMonthlyDebtPayment = 0m,
            RequestedTermMonths = 12
        };

        var result = await service.EvaluateAsync(profile);

        var offer = Assert.Single(result.MatchingBanks);
        Assert.Equal(10_000m, offer.MonthlyInstallment);
    }
}