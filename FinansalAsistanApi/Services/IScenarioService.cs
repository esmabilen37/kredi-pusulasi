using FinansalAsistanApi.Models;

namespace FinansalAsistanApi.Services;

public interface IScenarioService
{
    Task<List<ScenarioResult>> RunScenariosAsync(ScenarioRequestDto request);
}