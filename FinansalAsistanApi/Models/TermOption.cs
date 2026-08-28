namespace FinansalAsistanApi.Models;

public class TermOption
{
    public int TermMonths { get; set; }
    public decimal EstimatedMonthlyInstallment { get; set; }
    public decimal TotalMonthlyDebtLoad { get; set; }
    public decimal DebtToIncomeRatio { get; set; }
    public RiskLevel RiskLevel { get; set; }
    public List<BankOffer> MatchingBanks { get; set; } = new();
}