using Microsoft.ML.Data;

namespace  FinansalAsistan;

public class TokenPrediction
{
    [ColumnName("PredictedLabel")] public string Label { get; set; }= string.Empty;
}

