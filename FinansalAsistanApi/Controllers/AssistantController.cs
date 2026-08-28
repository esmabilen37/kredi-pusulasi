using FinansalAsistanApi.Models;
using FinansalAsistanApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
namespace FinansalAsistanApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AssistantController : ControllerBase
{
    private readonly IAssistantService _assistantService;
    private readonly ILogger<AssistantController> _logger;

    public AssistantController(IAssistantService assistantService, ILogger<AssistantController> logger)
    {
        _assistantService = assistantService;
        _logger = logger;
    }

    [HttpPost("message")]
    public async Task<IActionResult> SendMessage([FromBody] ChatRequestDto? request)
    {
        if (request?.Messages == null || request.Messages.Count == 0)
        {
            return BadRequest("En az bir mesaj gönderilmeli.");
        }

        try
        {
            var result = await _assistantService.GetResponseAsync(request.Messages);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Assistant endpoint'inde beklenmeyen hata oluştu.");
            return StatusCode(503, new AssistantResponseDto
            {
                Reply = "Şu an bir sorun yaşıyorum, lütfen birazdan tekrar dener misin?"
            });
        }
    }
}