using FinansalAsistanApi.Models;

namespace FinansalAsistanApi.Services;

public class AffordabilityService : IAffordabilityService
{
    private static readonly int[] CandidateTerms = { 12, 24, 36, 48, 60 };

    private readonly IBankProfileMatcher _bankMatcher;

    public AffordabilityService(IBankProfileMatcher bankMatcher)
    {
        _bankMatcher = bankMatcher;
    }

    public async Task<AffordabilityResult> EvaluateAsync(UserFinancialProfile profile)
    {
        if (profile.RequestedTermMonths <= 0)
        {
            return await BuildTermSuggestionsAsync(profile);
        }

        if (profile.LoanType == LoanType.Ihtiyac && !IhtiyacKrediRules.IsAllowed(profile.RequestedLoanAmount, profile.RequestedTermMonths))
        {
            return new AffordabilityResult
            {
                IsAllowed = false,
                RejectionReason = "BDDK kapsamı gereği, talep ettiğiniz tutar ve vade kombinasyonuyla ihtiyaç kredisi kullanamazsınız."
            };
        }

        if (profile.LoanType == LoanType.Tasit && !TasitKrediRules.IsAllowed(profile.RequestedLoanAmount, profile.RequestedTermMonths))
        {
            return new AffordabilityResult
            {
                IsAllowed = false,
                RejectionReason = "BDDK kapsamı gereği, talep ettiğiniz tutar ve vade kombinasyonuyla taşıt kredisi kullanamazsınız."
            };
        }

        // Eligibility aşamasında seçilen kredi türündeki piyasa ortalama faiziyle bir tahmini taksiti 
        
        var averageMonthlyRate = await _bankMatcher.GetAverageMonthlyInterestRateAsync(profile.LoanType);
        var estimatedInstallment = CalculateInstallment(profile.RequestedLoanAmount, averageMonthlyRate, profile.RequestedTermMonths);

        var totalMonthlyDebtLoad = profile.ExistingMonthlyDebtPayment + estimatedInstallment;

        var debtToIncomeRatio = CalculateDebtToIncomeRatio(totalMonthlyDebtLoad, profile.MonthlyIncome);
        var disposableIncome = profile.MonthlyIncome - profile.MonthlyExpenses - totalMonthlyDebtLoad;
        var riskLevel = DetermineRiskLevel(debtToIncomeRatio);

        var explanation =
            $"Aylık gelirin {profile.MonthlyIncome:N0} TL. Mevcut borcun ile bu kredinin " +
            $"piyasa ortalama faiziyle (%{averageMonthlyRate:N2} aylık) hesaplanan tahmini taksiti " +
            $"({estimatedInstallment:N0} TL) toplandığında aylık {totalMonthlyDebtLoad:N0} TL borç " +
            $"ödemesi yapman gerekir. Bu, gelirinin yaklaşık %{debtToIncomeRatio * 100:N0}'i demek. " +
            $"Giderlerini de düştüğünde elinde aylık {disposableIncome:N0} TL kalıyor.";

        var matchingBankProfiles = await _bankMatcher.GetMatchingBanksAsync(profile, debtToIncomeRatio);

        // Harcanabilir gelir negatifse DTI eşiğini geçse bile hiçbir banka gerçekten uygun sayılmamalı.
        
        if (disposableIncome < 0)
        {
            matchingBankProfiles = new List<BankProfile>();
        }

        var matchingBanks = BuildBankOffers(matchingBankProfiles, profile.LoanType, profile.RequestedLoanAmount, profile.RequestedTermMonths);

        return new AffordabilityResult
        {
            EstimatedMonthlyInstallment = estimatedInstallment,
            TotalMonthlyDebtLoad = totalMonthlyDebtLoad,
            DebtToIncomeRatio = debtToIncomeRatio,
            DisposableIncome = disposableIncome,
            RiskLevel = riskLevel,
            Explanation = explanation,
            MatchingBanks = matchingBanks
        };
    }

    private async Task<AffordabilityResult> BuildTermSuggestionsAsync(UserFinancialProfile profile)
    {
        var averageMonthlyRate = await _bankMatcher.GetAverageMonthlyInterestRateAsync(profile.LoanType);
        var suggestions = new List<TermOption>();

        foreach (var term in CandidateTerms)
        {
            if (!IsTermAllowedForLoanType(profile.LoanType, profile.RequestedLoanAmount, term))
            {
                continue;
            }

            var estimatedInstallment = CalculateInstallment(profile.RequestedLoanAmount, averageMonthlyRate, term);
            var totalMonthlyDebtLoad = profile.ExistingMonthlyDebtPayment + estimatedInstallment;
            var debtToIncomeRatio = CalculateDebtToIncomeRatio(totalMonthlyDebtLoad, profile.MonthlyIncome);
            var disposableIncome = profile.MonthlyIncome - profile.MonthlyExpenses - totalMonthlyDebtLoad;
            var riskLevel = DetermineRiskLevel(debtToIncomeRatio);

            var candidateProfile = new UserFinancialProfile
            {
                LoanType = profile.LoanType,
                MonthlyIncome = profile.MonthlyIncome,
                MonthlyExpenses = profile.MonthlyExpenses,
                RequestedLoanAmount = profile.RequestedLoanAmount,
                ExistingMonthlyDebtPayment = profile.ExistingMonthlyDebtPayment,
                RequestedTermMonths = term
            };

            var matchingBankProfiles = await _bankMatcher.GetMatchingBanksAsync(candidateProfile, debtToIncomeRatio);

            // Harcanabilir gelir negatifse, bu vade seçeneği için de hiçbir banka uygun sayılmasın
            if (disposableIncome < 0)
            {
                matchingBankProfiles = new List<BankProfile>();
            }

            var matchingBanks = BuildBankOffers(matchingBankProfiles, profile.LoanType, profile.RequestedLoanAmount, term);

            suggestions.Add(new TermOption
            {
                TermMonths = term,
                EstimatedMonthlyInstallment = estimatedInstallment,
                TotalMonthlyDebtLoad = totalMonthlyDebtLoad,
                DebtToIncomeRatio = debtToIncomeRatio,
                RiskLevel = riskLevel,
                MatchingBanks = matchingBanks
            });
        }

        if (suggestions.Count == 0)
        {
            var loanTypeLabel = profile.LoanType switch
            {
                LoanType.Ihtiyac => "ihtiyaç kredisi",
                LoanType.Tasit => "taşıt kredisi",
                LoanType.Konut => "konut kredisi",
                _ => "kredi"
            };

            return new AffordabilityResult
            {
                IsAllowed = false,
                RejectionReason = $"BDDK kapsamı gereği, {profile.RequestedLoanAmount:N0} TL tutarındaki " +
                                   $"{loanTypeLabel} için denenen hiçbir standart vadede (12-60 ay) uygunluk " +
                                   "sağlanamadı. Tutarı düşürmeyi deneyebilirsiniz."
            };
        }

        return new AffordabilityResult
        {
            IsAllowed = true,
            Explanation = "Vade belirtmediniz, bu yüzden farklı vade seçenekleri için uygunluk değerlendirmesi hazırlandı.",
            TermSuggestions = suggestions
        };
    }

    private static bool IsTermAllowedForLoanType(LoanType loanType, decimal amount, int term)
    {
        return loanType switch
        {
            LoanType.Ihtiyac => IhtiyacKrediRules.IsAllowed(amount, term),
            LoanType.Tasit => TasitKrediRules.IsAllowed(amount, term),
            _ => true
        };
    }

    private static decimal CalculateDebtToIncomeRatio(decimal totalMonthlyDebtLoad, decimal monthlyIncome)
    {
        return monthlyIncome > 0 ? totalMonthlyDebtLoad / monthlyIncome : 1m;
    }

    private static RiskLevel DetermineRiskLevel(decimal debtToIncomeRatio)
    {
        if (debtToIncomeRatio <= 0.30m) return RiskLevel.Guvenli;
        if (debtToIncomeRatio <= 0.40m) return RiskLevel.AzRiskli;
        if (debtToIncomeRatio <= 0.50m) return RiskLevel.Sinirda;
        if (debtToIncomeRatio <= 0.65m) return RiskLevel.Riskli;
        return RiskLevel.CokRiskli;
    }

    
    private static List<BankOffer> BuildBankOffers(List<BankProfile> banks, LoanType loanType, decimal principal, int termMonths)
    {
        var offers = new List<BankOffer>();

        foreach (var bank in banks)
        {
            var typeOffer = bank.GetOfferFor(loanType);
            if (typeOffer == null)
            {
                continue; // bu bankada bu kredi türü hiç yok
            }

            var installment = CalculateInstallment(principal, typeOffer.MonthlyInterestRate, termMonths);
            var totalRepayment = installment * termMonths;
            var totalInterestPaid = totalRepayment - principal;

            offers.Add(new BankOffer
            {
                Bank = bank,
                MonthlyInterestRate = typeOffer.MonthlyInterestRate,
                MaxLoanAmount = typeOffer.MaxLoanAmount,
                MaxTermMonths = typeOffer.MaxTermMonths,
                MonthlyInstallment = installment,
                TotalRepayment = totalRepayment,
                TotalInterestPaid = totalInterestPaid
            });
        }

        // Kullanıcıya en avantajlı banka en üstte görünsün
        return offers.OrderBy(o => o.MonthlyInstallment).ToList();
    }

    
    // Eşit taksitli kredi (annuite) formülü ile aylık taksit hesaplar:
    // Taksit = P * r * (1+r)^n / ((1+r)^n - 1)
    // P: anapara, r: aylık faiz oranı (ondalık, örn. %2.99 -> 0.0299), n: vade (ay)
    //Decimal hassasiyeti korumak için (1+r)^n döngüyle hesaplanır, double'a çevrilmez.
   
    private static decimal CalculateInstallment(decimal principal, decimal monthlyInterestRatePercent, int termMonths)
    {
        if (termMonths <= 0)
        {
            return 0m;
        }

        // Faiz oranı tanımsız/sıfırsa, faizsiz düz bölmeye düş (savunma amaçlı)
        if (monthlyInterestRatePercent <= 0)
        {
            return principal / termMonths;
        }

        var r = monthlyInterestRatePercent / 100m;

        var growthFactor = 1m;
        for (var i = 0; i < termMonths; i++)
        {
            growthFactor *= (1 + r);
        }

        return principal * r * growthFactor / (growthFactor - 1);
    }
}