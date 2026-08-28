namespace FinansalAsistanApi.Models;

public class AssistantResponseDto
{
    public decimal MonthlyIncome { get; set; }
    public decimal MonthlyExpenses { get; set; }
    public decimal RequestedLoanAmount { get; set; }
    public int RequestedTermMonths { get; set; }
    public LoanType LoanType { get; set; } = LoanType.Ihtiyac;

    //Kullanıcıya gösterilecek doğal dil cevabı (veri onayı ya da soru cevabı)
    public string Reply { get; set; } = "";
}