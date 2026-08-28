namespace FinansalAsistanApi.Models;

public class BankOffer
{
    public BankProfile Bank { get; set; } = null!;
    public decimal MonthlyInterestRate { get; set; }
    public decimal MaxLoanAmount { get; set; }
    public int MaxTermMonths { get; set; }
    public decimal MonthlyInstallment { get; set; }
    public decimal TotalRepayment { get; set; }
    public decimal TotalInterestPaid { get; set; }
}