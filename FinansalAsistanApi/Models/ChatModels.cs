namespace FinansalAsistanApi.Models;

public class ChatMessage
{
    public string Role { get; set; } = "user"; // "user" ya da "assistant"
    public string Content { get; set; } = "";
}

public class ChatRequestDto
{
    public List<ChatMessage> Messages { get; set; } = new();
}