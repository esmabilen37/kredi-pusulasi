using FinansalAsistanApi.Models;
using MongoDB.Driver;

namespace FinansalAsistanApi.Services;

public class BankProfileMatcher: IBankProfileMatcher
{
    private readonly MongoDbContext _context;

    public BankProfileMatcher(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<List<BankProfile>> GetMatchingBanksAsync(UserFinancialProfile profile, decimal debtToIncomeRatio)
    {
        var allBanks = await _context.BankProfiles.Find(_ => true).ToListAsync();

        var matches = new List<BankProfile>();
        foreach (var bank in allBanks)
        {
            
            var offer = bank.GetOfferFor(profile.LoanType);
            if (offer == null)
            {
                continue;
            }

            bool fits = debtToIncomeRatio <= bank.MaxDebtToIncomeRatio
                        && profile.MonthlyIncome >= bank.MinMonthlyIncome
                        && profile.RequestedLoanAmount <= offer.MaxLoanAmount
                        && profile.RequestedTermMonths <= offer.MaxTermMonths;

            if (fits)
            {
                matches.Add(bank);
            }
        }

        return matches;
    }

    public async Task<decimal> GetAverageMonthlyInterestRateAsync(LoanType loanType)
    {
        var allBanks = await _context.BankProfiles.Find(_ => true).ToListAsync();

        var ratesForType = allBanks
            .Select(bank => bank.GetOfferFor(loanType))
            .Where(offer => offer != null)
            .Select(offer => offer!.MonthlyInterestRate)
            .ToList();

        if (ratesForType.Count == 0)
        {
            return 0m;
        }

        return ratesForType.Average();
    }
}