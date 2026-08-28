using System.Text.RegularExpressions;
using FinansalAsistanApi.Models;

namespace FinansalAsistanApi.Services;

public static class Tokenizer
{
    private static bool IsNumeric(string token) => Regex.IsMatch(token, @"^\d+$");

    public static List<TokenData> TokenizeForPrediction(string sentence)
    {
        var words = sentence
            .Replace(",", " ")
            .Replace(".", " ")
            .Split(' ', StringSplitOptions.RemoveEmptyEntries);

        var result = new List<TokenData>();
        for (int i = 0; i < words.Length; i++)
        {
            var newToken = new TokenData
            {
                Token = words[i],
                PrevPrev = i > 1 ? words[i - 2] : "<YOK>",
                Prev = i > 0 ? words[i - 1] : "<YOK>",
                Next = i < words.Length - 1 ? words[i + 1] : "<YOK>",
                NextNext = i < words.Length - 2 ? words[i + 2] : "<YOK>",
                IsNumeric = IsNumeric(words[i]),
                Label = string.Empty
            };
            result.Add(newToken);
        }
        return result;
    }
}