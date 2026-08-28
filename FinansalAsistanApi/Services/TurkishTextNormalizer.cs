namespace FinansalAsistanApi.Services;

public static class TurkishTextNormalizer
{
    public static string Fold(string word) => word
        .Replace("ü", "u").Replace("Ü", "u")
        .Replace("ş", "s").Replace("Ş", "s")
        .Replace("ç", "c").Replace("Ç", "c")
        .Replace("ö", "o").Replace("Ö", "o")
        .Replace("ı", "i").Replace("İ", "i")
        .Replace("ğ", "g").Replace("Ğ", "g");
}