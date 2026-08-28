using System.Linq;
using FinansalAsistanApi.Models;

namespace FinansalAsistanApi.Services;

public static class LoanTypeDetector
{
    private static readonly string[] TasitKeywords =
    {
        "tasit", "arac", "araba", "otomobil", "motor", "motosiklet", "kamyon", "kamyonet"
    };

    private static readonly string[] KonutKeywords =
    {
        "konut", "ev", "daire", "arsa", "dukkan", "villa", "apartman", "mesken", "yer"
    };

    public static LoanType? Detect(string text)
    {
        var normalized = TurkishTextNormalizer.Fold(
            text.ToLower(new System.Globalization.CultureInfo("tr-TR"))
        );

        bool isTasit = TasitKeywords.Any(k => normalized.Contains(k));
        bool isKonut = KonutKeywords.Any(k => normalized.Contains(k));

        if (isTasit && isKonut)
            return null; // çakışma — belirsiz

        if (isTasit)
            return LoanType.Tasit;

        if (isKonut)
            return LoanType.Konut;

        return LoanType.Ihtiyac; // varsayılan
    }
}