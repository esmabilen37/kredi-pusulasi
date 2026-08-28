using FinansalAsistanApi.Models;
using Microsoft.AspNetCore.Mvc;
using FinansalAsistanApi.Services;
using Microsoft.AspNetCore.Authorization;
namespace FinansalAsistanApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AffordabilityController : ControllerBase
{
    private readonly IAffordabilityService _affordabilityService;
    private readonly IScenarioService _scenarioService;    
    public AffordabilityController(IAffordabilityService affordabilityService, IScenarioService scenarioService)
    {
        _affordabilityService = affordabilityService;
        _scenarioService = scenarioService;
    }
    [HttpPost("evaluate")]
    public async Task<IActionResult> Evaluate([FromBody] UserFinancialProfile profile)
    {
        var result = await _affordabilityService.EvaluateAsync(profile);
        return Ok(result);
    }
    
    [HttpPost("scenarios")]
    public async Task<IActionResult> RunScenarios([FromBody] ScenarioRequestDto request)
    {
        var results = await _scenarioService.RunScenariosAsync(request);
        return Ok(results);
    }
}