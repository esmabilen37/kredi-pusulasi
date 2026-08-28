namespace FinansalAsistanApi.Services;

public interface INumberNormalizationService
{
    decimal? ExtractAmount(string text);
    string NormalizeSentence(string text);
}

