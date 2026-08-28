namespace FinansalAsistanApi.Models;

public class LoanTypeOffer
{
    public LoanType LoanType { get; set; }
    public decimal MonthlyInterestRate { get; set; }
    public decimal MaxLoanAmount { get; set; }
    public int MaxTermMonths { get; set; }
}