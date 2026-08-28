namespace FinansalAsistanApi.Models;

public class UserFinancialProfile
{
    public required LoanType LoanType { get; set; }
    public decimal MonthlyIncome { get; set; }
    public decimal MonthlyExpenses { get; set; }
    public decimal RequestedLoanAmount { get; set; }
    public decimal ExistingMonthlyDebtPayment { get; set; }
    public int RequestedTermMonths { get; set; }
}

