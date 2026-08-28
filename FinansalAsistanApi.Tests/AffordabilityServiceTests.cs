using FinansalAsistanApi.Models;
using FinansalAsistanApi.Services;
using Xunit;

namespace FinansalAsistanApi.Tests;

public class AffordabilityServiceTests
{
    [Fact]
    public async Task Evaluate_OrtaGelirliProfil_SinirdaRiskDondurur()
    {
        // Arrange
        var service = new AffordabilityService(new FakeBankProfileMatcher());
        var profile = new UserFinancialProfile
        {
            LoanType = LoanType.Ihtiyac,
            MonthlyIncome = 45000,
            MonthlyExpenses = 22000,
            ExistingMonthlyDebtPayment = 6000,
            RequestedLoanAmount = 150000,
            RequestedTermMonths = 12
        };

        // Act
        var result = await service.EvaluateAsync(profile);

        // Assert
        Assert.Equal(RiskLevel.Sinirda, result.RiskLevel);
    }

    [Fact]
    public async Task Evaluate_GelirSifir_CokRiskliDondurur()
    {
        // Arrange
        var service = new AffordabilityService(new FakeBankProfileMatcher());
        var profile = new UserFinancialProfile
        {
            LoanType = LoanType.Konut,
            MonthlyIncome = 0,
            MonthlyExpenses = 5000,
            ExistingMonthlyDebtPayment = 0,
            RequestedLoanAmount = 100000,
            RequestedTermMonths = 12
        };

        // Act
        var result = await service.EvaluateAsync(profile);

        // Assert
        Assert.Equal(RiskLevel.CokRiskli, result.RiskLevel);
    }

    [Fact]
    public async Task Evaluate_TamYuzde30Oran_GuvenliDondurur()
    {
        // Arrange: 30000 / 100000 = tam %30 -> sınırda "Guvenli" tarafında kalmalı (<=)
        var service = new AffordabilityService(new FakeBankProfileMatcher());
        var profile = new UserFinancialProfile
        {
            LoanType = LoanType.Tasit,
            MonthlyIncome = 100000,
            MonthlyExpenses = 0,
            ExistingMonthlyDebtPayment = 0,
            RequestedLoanAmount = 30000,
            RequestedTermMonths = 1
        };

        // Act
        var result = await service.EvaluateAsync(profile);

        // Assert
        Assert.Equal(RiskLevel.Guvenli, result.RiskLevel);
    }

    [Fact]
    public async Task Evaluate_Yuzde30UstuBirBirim_AzRiskliDondurur()
    {
        // Arrange: 300001 / 1000000 = %30,0001 -> %30'u aştığı için "AzRiskli"ye geçmeli
        var service = new AffordabilityService(new FakeBankProfileMatcher());
        var profile = new UserFinancialProfile
        {
            LoanType = LoanType.Ihtiyac,
            MonthlyIncome = 1000000,
            MonthlyExpenses = 0,
            ExistingMonthlyDebtPayment = 0,
            RequestedLoanAmount = 300001,
            RequestedTermMonths = 1
        };

        // Act
        var result = await service.EvaluateAsync(profile);

        // Assert
        Assert.Equal(RiskLevel.AzRiskli, result.RiskLevel);
    }

    [Fact]
    public async Task Evaluate_TamYuzde40Oran_AzRiskliDondurur()
    {
        // Arrange: 40000 / 100000 = tam %40 -> "AzRiskli" sınırında kalmalı
        var service = new AffordabilityService(new FakeBankProfileMatcher());
        var profile = new UserFinancialProfile
        {
            LoanType = LoanType.Ihtiyac,
            MonthlyIncome = 100000,
            MonthlyExpenses = 0,
            ExistingMonthlyDebtPayment = 0,
            RequestedLoanAmount = 40000,
            RequestedTermMonths = 1
        };

        // Act
        var result = await service.EvaluateAsync(profile);

        // Assert
        Assert.Equal(RiskLevel.AzRiskli, result.RiskLevel);
    }

    [Fact]
    public async Task Evaluate_TamYuzde50Oran_SinirdaDondurur()
    {
        // Arrange: 50000 / 100000 = tam %50 -> "Sinirda" sınırında kalmalı
        var service = new AffordabilityService(new FakeBankProfileMatcher());
        var profile = new UserFinancialProfile
        {
            LoanType = LoanType.Ihtiyac,
            MonthlyIncome = 100000,
            MonthlyExpenses = 0,
            ExistingMonthlyDebtPayment = 0,
            RequestedLoanAmount = 50000,
            RequestedTermMonths = 1
        };

        // Act
        var result = await service.EvaluateAsync(profile);

        // Assert
        Assert.Equal(RiskLevel.Sinirda, result.RiskLevel);
    }

    [Fact]
    public async Task Evaluate_TamYuzde65Oran_RiskliDondurur()
    {
        // Arrange: 65000 / 100000 = tam %65 -> "Riskli" sınırında kalmalı
        var service = new AffordabilityService(new FakeBankProfileMatcher());
        var profile = new UserFinancialProfile
        {
            LoanType = LoanType.Konut,
            MonthlyIncome = 100000,
            MonthlyExpenses = 0,
            ExistingMonthlyDebtPayment = 0,
            RequestedLoanAmount = 65000,
            RequestedTermMonths = 1
        };

        // Act
        var result = await service.EvaluateAsync(profile);

        // Assert
        Assert.Equal(RiskLevel.Riskli, result.RiskLevel);
    }

    [Fact]
    public async Task Evaluate_Yuzde65UstuOran_CokRiskliDondurur()
    {
        // Arrange: 70000 / 100000 = %70 -> "CokRiskli" olmalı
        var service = new AffordabilityService(new FakeBankProfileMatcher());
        var profile = new UserFinancialProfile
        {
            LoanType = LoanType.Tasit,
            MonthlyIncome = 100000,
            MonthlyExpenses = 0,
            ExistingMonthlyDebtPayment = 0,
            RequestedLoanAmount = 70000,
            RequestedTermMonths = 1
        };

        // Act
        var result = await service.EvaluateAsync(profile);

        // Assert
        Assert.Equal(RiskLevel.CokRiskli, result.RiskLevel);
    }

    [Fact]
    public async Task Evaluate_IhtiyacKredisiBddkSiniriniAsiyor_ReddedilirVeMesajDoner()
    {
        // Arrange: 250000 TL, 36 ay vade -> hiçbir BDDK kuralına uymuyor
        var service = new AffordabilityService(new FakeBankProfileMatcher());
        var profile = new UserFinancialProfile
        {
            LoanType = LoanType.Ihtiyac,
            MonthlyIncome = 100000,
            MonthlyExpenses = 0,
            ExistingMonthlyDebtPayment = 0,
            RequestedLoanAmount = 250000,
            RequestedTermMonths = 36
        };

        // Act
        var result = await service.EvaluateAsync(profile);

        // Assert
        Assert.False(result.IsAllowed);
        Assert.NotNull(result.RejectionReason);
        Assert.Null(result.RiskLevel);
    }

    [Fact]
    public async Task Evaluate_TasitKredisiBddkSiniriniAsiyor_ReddedilirVeMesajDoner()
    {
        // Arrange: 450000 TL, 36 ay vade -> TasitKrediRules'daki üst sınır 400000
        var service = new AffordabilityService(new FakeBankProfileMatcher());
        var profile = new UserFinancialProfile
        {
            LoanType = LoanType.Tasit,
            MonthlyIncome = 200000,
            MonthlyExpenses = 0,
            ExistingMonthlyDebtPayment = 0,
            RequestedLoanAmount = 450000,
            RequestedTermMonths = 36
        };

        // Act
        var result = await service.EvaluateAsync(profile);

        // Assert
        Assert.False(result.IsAllowed);
        Assert.NotNull(result.RejectionReason);
        Assert.Null(result.RiskLevel);
    }

    [Fact]
    public async Task Evaluate_KonutKredisiCokYuksekTutar_BddkKontroluOlmadanDevamEder()
    {
        // Not: talimata göre v1 kapsamında konut kredisi için BDDK kontrolü YOK.
        // İleride BDDK'nın konut/LTV kuralları eklenmek istenirse, bu test güncellenmeli.
        var service = new AffordabilityService(new FakeBankProfileMatcher());
        var profile = new UserFinancialProfile
        {
            LoanType = LoanType.Konut,
            MonthlyIncome = 100000,
            MonthlyExpenses = 0,
            ExistingMonthlyDebtPayment = 0,
            RequestedLoanAmount = 5000000,
            RequestedTermMonths = 36
        };

        var result = await service.EvaluateAsync(profile);

        Assert.True(result.IsAllowed);
        Assert.Null(result.RejectionReason);
        Assert.NotNull(result.RiskLevel);
    }
}