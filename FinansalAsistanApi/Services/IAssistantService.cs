using FinansalAsistanApi.Models;

namespace FinansalAsistanApi.Services;

public interface IAssistantService
{
    Task<AssistantResponseDto> GetResponseAsync(List<ChatMessage> messages);
}