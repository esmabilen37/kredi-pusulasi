using FinansalAsistanApi.Models;
using MongoDB.Driver; 
namespace FinansalAsistanApi.Services;


public class ScenarioService : IScenarioService
{
    private readonly IAffordabilityService _affordabilityService;
    private readonly MongoDbContext _context;

    public ScenarioService(IAffordabilityService affordabilityService, MongoDbContext context)
    {
        _affordabilityService = affordabilityService;
        _context = context;
    }

    public async Task<List<ScenarioResult>> RunScenariosAsync(ScenarioRequestDto request)
    {
        var results = new List<ScenarioResult>();

        foreach (var adjustment in request.Adjustments)
        {
            var adjustedProfile = ApplyAdjustment(request.BaseProfile, adjustment);
            results.Add(await BuildScenarioResultAsync(adjustment.Label, adjustedProfile));
        }

        if (request.IncludeStressTest)
        {
            var stressScenarios = new List<ScenarioAdjustment>
            {
                new() { Label = "Gelir %10 azalırsa", IncomeChangePercentage = -10 },
                new() { Label = "Gelir %20 azalırsa", IncomeChangePercentage = -20 },
                new() { Label = "Gider %10 artarsa", ExpenseChangePercentage = 10 },
                new() { Label = "Gider %20 artarsa", ExpenseChangePercentage = 20 },
                new() { Label = "Gelir %15 azalır + gider %15 artarsa", IncomeChangePercentage = -15, ExpenseChangePercentage = 15 },
            };

            foreach (var stress in stressScenarios)
            {
                var adjustedProfile = ApplyAdjustment(request.BaseProfile, stress);
                results.Add(await BuildScenarioResultAsync(stress.Label, adjustedProfile));
            }
        }

        return results;
    }

    private static UserFinancialProfile ApplyAdjustment(UserFinancialProfile baseProfile, ScenarioAdjustment adjustment)
    {
        var adjusted = new UserFinancialProfile
        {
            MonthlyIncome = baseProfile.MonthlyIncome,
            MonthlyExpenses = baseProfile.MonthlyExpenses,
            ExistingMonthlyDebtPayment = baseProfile.ExistingMonthlyDebtPayment,
            RequestedLoanAmount = adjustment.NewRequestedLoanAmount ?? baseProfile.RequestedLoanAmount,
            RequestedTermMonths = adjustment.NewRequestedTermMonths ?? baseProfile.RequestedTermMonths,
            LoanType = baseProfile.LoanType
        };

        if (adjustment.IncomeChangePercentage.HasValue)
        {
            adjusted.MonthlyIncome += baseProfile.MonthlyIncome * (adjustment.IncomeChangePercentage.Value / 100m);
        }

        if (adjustment.ExpenseChangePercentage.HasValue)
        {
            adjusted.MonthlyExpenses += baseProfile.MonthlyExpenses * (adjustment.ExpenseChangePercentage.Value / 100m);
        }

        return adjusted;
    }

    private async Task<ScenarioResult> BuildScenarioResultAsync(string label, UserFinancialProfile adjustedProfile)
    {
        // AffordabilityService zaten banka eşleştirmesini + taksit hesaplamasını kendi içinde yapıyor
        var affordability = await _affordabilityService.EvaluateAsync(adjustedProfile);

        var recommendations = new List<string>();

        // BDDK reddi varsa veya vade belirtilmediyse (TermSuggestions dönüyorsa) öneri motoru çalışmaz
        if (affordability.IsAllowed != false && affordability.MatchingBanks != null)
        {
            recommendations = await BuildRecommendationsAsync(adjustedProfile, affordability);
        }

        return new ScenarioResult
        {
            Label = label,
            AdjustedProfile = adjustedProfile,
            Affordability = affordability,
            Recommendations = recommendations
        };
    }

    private async Task<List<string>> BuildRecommendationsAsync(UserFinancialProfile profile, AffordabilityResult affordability)
    {
        var recommendations = new List<string>();
        if (profile.MonthlyIncome <= 0)
        {
            recommendations.Add("Gelirin 0 TL olduğu sürece hiçbir bankadan teklif alamazsın — kredi değerlendirmesi için en azından asgari bir aylık gelir gerekiyor.");
            return recommendations;
        }
        if (affordability.DisposableIncome < 0)   
        {
            recommendations.Add($"Giderlerin, gelirinden mevcut borç ve bu kredinin taksitini çıkardıktan sonra kalan tutarı aşıyor (aylık {affordability.DisposableIncome:N0} TL). Tutarı ya da vadeyi değiştirmek tek başına yeterli olmayabilir — önce giderlerini gelirinin altına indirmen gerekiyor.");
            return recommendations;
        }

        var allBanks = await _context.BankProfiles.Find(_ => true).ToListAsync();
        var matchedNames = affordability.MatchingBanks!.Select(o => o.Bank.Name).ToHashSet();

        foreach (var bank in allBanks.Where(b => !matchedNames.Contains(b.Name)))
        {
            var offer = bank.GetOfferFor(profile.LoanType);
            if (offer == null) continue;

            if (profile.RequestedLoanAmount > offer.MaxLoanAmount)
            {
                var diff = profile.RequestedLoanAmount - offer.MaxLoanAmount;
                if (diff <= profile.RequestedLoanAmount * 0.25m)
                {
                    recommendations.Add($"Talep ettiğin tutarı {diff:N0} TL azaltırsan {bank.Name}'ndan da teklif alabilirsin.");
                }
            }

            if (affordability.DebtToIncomeRatio > bank.MaxDebtToIncomeRatio)
            {
                var ratioDiff = affordability.DebtToIncomeRatio - bank.MaxDebtToIncomeRatio;
                if (ratioDiff <= 0.10m && ratioDiff > 0.005m)
                {
                    recommendations.Add($"Borç/gelir oranını yaklaşık %{ratioDiff * 100:N1} puan düşürürsen {bank.Name}'ndan uygun hale gelebilirsin.");
                }
            }

            if (profile.RequestedTermMonths > offer.MaxTermMonths)
            {
                recommendations.Add($"Vadeyi {offer.MaxTermMonths} aya indirirsen {bank.Name}'ndan teklif alabilirsin.");
            }

            if (profile.MonthlyIncome < bank.MinMonthlyIncome)
            {
                var gap = bank.MinMonthlyIncome - profile.MonthlyIncome;
                if (gap <= profile.MonthlyIncome * 0.20m)
                {
                    recommendations.Add($"Gelirin {gap:N0} TL daha yüksek olsaydı {bank.Name} için de uygun olurdun.");
                }
            }
        }

        return recommendations;
    }

       
}