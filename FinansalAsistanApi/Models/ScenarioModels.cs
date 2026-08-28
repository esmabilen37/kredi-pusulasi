namespace FinansalAsistanApi.Models;

public class ScenarioRequestDto
{
    public UserFinancialProfile BaseProfile { get; set; } = null!;
    public List<ScenarioAdjustment> Adjustments { get; set; } = new();
    public bool IncludeStressTest { get; set; } = false;
}

public class ScenarioAdjustment
{
    public string Label { get; set; } = "";
    public decimal? NewRequestedLoanAmount { get; set; }
    public int? NewRequestedTermMonths { get; set; }
    public decimal? IncomeChangePercentage { get; set; }   
    public decimal? ExpenseChangePercentage { get; set; }  
}

public class ScenarioResult
{
    public string Label { get; set; } = "";
    public UserFinancialProfile AdjustedProfile { get; set; } = null!;
    public AffordabilityResult Affordability { get; set; } = null!;
    public List<string> Recommendations { get; set; } = new();
}