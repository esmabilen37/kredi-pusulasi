using Microsoft.ML.Data;

namespace FinansalAsistan;

public class TokenData
{
    [LoadColumn(0)] public string Token { get; set; } = string.Empty;
    [LoadColumn(1)] public string PrevPrev { get; set; } = string.Empty;
    [LoadColumn(2)] public string Prev { get; set; } = string.Empty;
    [LoadColumn(3)] public string Next { get; set; } = string.Empty;
    [LoadColumn(4)] public string NextNext { get; set; } = string.Empty;
    [LoadColumn(5)] public bool IsNumeric { get; set; }
    [LoadColumn(6)] public string Label { get; set; } = string.Empty;
}