namespace FinansalAsistanApi.Models;

public class TokenData
{
    public string Token { get; set; } = string.Empty;
    public string PrevPrev { get; set; } = string.Empty;
    public string Prev { get; set; } = string.Empty;
    public string Next { get; set; } = string.Empty;
    public string NextNext { get; set; } = string.Empty;
    public bool IsNumeric { get; set; }
    public string Label { get; set; } = string.Empty;
}