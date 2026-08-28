using FinansalAsistanApi.Models;

namespace FinansalAsistanApi.Services;

public interface IAffordabilityService
{
    Task<AffordabilityResult> EvaluateAsync(UserFinancialProfile profile);
}