namespace FinansalAsistanApi.Models;

public class AffordabilityResult
{
    public decimal EstimatedMonthlyInstallment { get; set; }
    public decimal TotalMonthlyDebtLoad { get; set; }
    public decimal DebtToIncomeRatio { get; set; }
    public decimal DisposableIncome { get; set; }   
    public  RiskLevel? RiskLevel { get; set; }
    public string Explanation { get; set; } = string.Empty;
    public bool IsAllowed { get; set; } = true; //bddk sağlanmazsa falsea cevircem
    public string? RejectionReason { get; set; } //kredi onaylanırsa null donuyor red icin bir sebep yok anlamında
    //red olursa da bddk kapsamı gereği.. yazdırcam o yuzden ekledim
    public List<BankOffer> MatchingBanks { get; set; } = new();
    public List<TermOption> TermSuggestions { get; set; } = new();
}