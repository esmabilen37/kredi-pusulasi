using Microsoft.ML.Data;

namespace FinansalAsistanApi.Models;

public class TokenPrediction
{
    [ColumnName("PredictedLabel")]
    public string PredictedLabel { get; set; } = string.Empty;
}

