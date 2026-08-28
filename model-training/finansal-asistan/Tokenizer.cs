using System.Text.RegularExpressions;

namespace FinansalAsistan;

public static class Tokenizer
{
    private static bool IsNumeric(string token)
    {
        return Regex.IsMatch(token, @"^\d+$");
    }

    public static List<TokenData> TokenizeForPrediction(string sentence)
    {
        var words = sentence
            .Replace(",", " ")
            .Replace(".", " ")
            .Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var result = new List<TokenData>();
        for (int i = 0; i < words.Length; i++)
        {
            TokenData newToken = new TokenData();
            newToken.Token = words[i];

            newToken.PrevPrev = i > 1 ? words[i - 2] : "<YOK>";
            newToken.Prev = i > 0 ? words[i - 1] : "<YOK>";
            newToken.Next = i < words.Length - 1 ? words[i + 1] : "<YOK>";
            newToken.NextNext = i < words.Length - 2 ? words[i + 2] : "<YOK>";

            newToken.IsNumeric = IsNumeric(words[i]);
            newToken.Label = string.Empty;
            result.Add(newToken);
        }
        return result;
    }
}