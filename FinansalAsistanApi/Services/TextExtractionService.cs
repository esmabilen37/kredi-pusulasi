using FinansalAsistanApi.Models;
using Microsoft.ML;

namespace FinansalAsistanApi.Services;

public class TextExtractionService 
{
    private readonly PredictionEngine<TokenData, TokenPrediction> _predictionEngine;
    private readonly INumberNormalizationService _numberNormalizationService;

    public TextExtractionService(INumberNormalizationService numberNormalizationService)
    {
        var mlContext = new MLContext();
        var modelPath = Path.Combine(AppContext.BaseDirectory, "model.zip");
        var model = mlContext.Model.Load(modelPath, out _);
        _predictionEngine = mlContext.Model.CreatePredictionEngine<TokenData, TokenPrediction>(model);
        _numberNormalizationService = numberNormalizationService;
    }

    public Task<UserFinancialProfile> ExtractFromTextAsync(string sentence)
    {
        
        var normalizedSentence = _numberNormalizationService.NormalizeSentence(sentence);
        var tokens = Tokenizer.TokenizeForPrediction(normalizedSentence);

        var detectedLoanType = LoanTypeDetector.Detect(sentence);

        var profile = new UserFinancialProfile
        {
            LoanType = detectedLoanType ?? LoanType.Ihtiyac
        };

        foreach (var token in tokens)
        {
            var prediction = _predictionEngine.Predict(token);

            if (!decimal.TryParse(token.Token, out var numericValue))
            {
                continue;
            }

            switch (prediction.PredictedLabel)
            {
                case "GELIR":
                    profile.MonthlyIncome = numericValue;
                    break;
                case "GIDER":
                    profile.MonthlyExpenses = numericValue;
                    break;
                case "TUTAR":
                    profile.RequestedLoanAmount = numericValue;
                    break;
                case "VADE":
                    profile.RequestedTermMonths = (int)numericValue;
                    break;
            }
        }

        return Task.FromResult(profile);
    }
}