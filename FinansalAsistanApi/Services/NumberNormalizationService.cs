using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace FinansalAsistanApi.Services;

public class NumberNormalizationService : INumberNormalizationService
{
    private static bool IsDigitsOnly(string word) => Regex.IsMatch(word, @"^\d+$");

    // "15bin", "200bin", "15k" gibi rakam harf bitişik kalıpları yakalamak
    private static readonly Regex DigitWordCompoundRegex =
        new(@"^(\d+)([a-zA-ZçÇğĞıİöÖşŞüÜ]+)$", RegexOptions.Compiled);

    // "binlik", "milyonluk" gibi sayı kelimesine eklenen sıfat ekleri -- kendi başlarına
    // sayısal bir değer taşımazlar, kelimeyi parçalarken yok sayılmalılar
    // (örn. "binlik" = "bin" kökü + "-lik" eki -> sadece "bin" işlenir)
    private static readonly HashSet<string> TrailingSuffixes = new() { "lik", "luk" };

    private static readonly Dictionary<string, int> FoldedBaseNumbers =
        NumberWordDictionary.BaseNumbers.ToDictionary(kv => TurkishTextNormalizer.Fold(kv.Key), kv => kv.Value);

    private static readonly Dictionary<string, decimal> FoldedMultipliers = BuildFoldedMultipliers();

    private static Dictionary<string, decimal> BuildFoldedMultipliers()
    {
        var dict = NumberWordDictionary.Multipliers.ToDictionary(kv => TurkishTextNormalizer.Fold(kv.Key), kv => kv.Value);
        dict["k"] = 1000m; // "15k" gibi kısaltmalar için ek giriş
        return dict;
    }

    private static void ProcessFoldedToken(string folded, ref decimal current, ref decimal total, ref bool foundAny)
    {
        if (FoldedBaseNumbers.TryGetValue(folded, out int baseValue))
        {
            current += baseValue;
            foundAny = true;
        }
        else if (FoldedMultipliers.TryGetValue(folded, out decimal multiplier))
        {
            if (current == 0)
            {
                current = 1;
            }

            current *= multiplier;
            foundAny = true;
            if (multiplier >= 1000)
            {
                total += current;
                current = 0;
            }
        }
    }

    // Bir kelimenin doğrudan ya da -lik/-lık/-luk/-lük eki çıkarılmış haliyle
    // çarpan sözlüğünde (bin/milyon/k vb.) karşılığını bulması için
    private static bool TryResolveMultiplier(string wordPart, out decimal multiplier)
    {
        if (FoldedMultipliers.TryGetValue(wordPart, out multiplier))
        {
            return true;
        }

        foreach (var suffix in TrailingSuffixes)
        {
            if (wordPart.Length > suffix.Length && wordPart.EndsWith(suffix, StringComparison.Ordinal))
            {
                var stripped = wordPart[..^suffix.Length];
                if (FoldedMultipliers.TryGetValue(stripped, out multiplier))
                {
                    return true;
                }
            }
        }

        multiplier = 0m;
        return false;
    }

    // "15bin" -> 15000, "200bin" -> 200000, "15k" -> 15000, "600klık"/"200binlik"  ek varsa da çözülür
    private static decimal? TryParseDigitWordCompound(string word)
    {
        var match = DigitWordCompoundRegex.Match(word);
        if (!match.Success)
            return null;

        var digitPart = decimal.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
        var wordPart = TurkishTextNormalizer.Fold(match.Groups[2].Value.ToLower(new CultureInfo("tr-TR")));

        if (TryResolveMultiplier(wordPart, out var multiplier))
        {
            return digitPart * multiplier;
        }

        return null;
    }

    public decimal? ExtractAmount(string text)
    {
        var words = text.ToLower(new CultureInfo("tr-TR"))
            .Split(' ', StringSplitOptions.RemoveEmptyEntries);
        decimal total = 0;
        decimal current = 0;
        bool foundAny = false;

        foreach (var word in words)
        {
            var folded = TurkishTextNormalizer.Fold(word);

            if (FoldedBaseNumbers.ContainsKey(folded) || FoldedMultipliers.ContainsKey(folded))
            {
                ProcessFoldedToken(folded, ref current, ref total, ref foundAny);
            }
            else if (IsDigitsOnly(word))
            {
                current += decimal.Parse(word, CultureInfo.InvariantCulture);
                foundAny = true;
            }
            else
            {
                var compoundValue = TryParseDigitWordCompound(word);
                if (compoundValue.HasValue)
                {
                    var wordPart = TurkishTextNormalizer.Fold(DigitWordCompoundRegex.Match(word).Groups[2].Value);
                    TryResolveMultiplier(wordPart, out var multiplier);

                    if (multiplier >= 1000)
                    {
                        total += compoundValue.Value;
                    }
                    else
                    {
                        current += compoundValue.Value;
                    }
                    foundAny = true;
                }
                else
                {
                    var parts = SplitCompoundNumberWord(folded);
                    if (parts != null)
                    {
                        foreach (var part in parts)
                        {
                            ProcessFoldedToken(part, ref current, ref total, ref foundAny);
                        }
                    }
                }
            }
        }
        total += current;
        return foundAny ? total : null;
    }

    private bool IsPartOfNumberGroup(string[] words, int index)
    {
        var folded = TurkishTextNormalizer.Fold(words[index].ToLower(new CultureInfo("tr-TR")));

        if (FoldedBaseNumbers.ContainsKey(folded) || FoldedMultipliers.ContainsKey(folded))
        {
            return true;
        }

        if (IsDigitsOnly(words[index]) && index + 1 < words.Length)
        {
            var nextFolded = TurkishTextNormalizer.Fold(words[index + 1].ToLower(new CultureInfo("tr-TR")));
            return FoldedMultipliers.ContainsKey(nextFolded);
        }

        if (TryParseDigitWordCompound(words[index]) != null)
        {
            return true;
        }

        if (SplitCompoundNumberWord(folded) != null)
        {
            return true;
        }

        return false;
    }

    public string NormalizeSentence(string text)
    {
        var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var resultWords = new List<string>();

        int i = 0;
        while (i < words.Length)
        {
            if (IsPartOfNumberGroup(words, i))
            {
                int start = i;

                // "35k", "200bin" gibi rakam+kelime bitişik kısaltmalar  kendi başına tamamlanmış bir sayıdır. Örn. "gelirim 35k beş yüz binlik taşıt kredisi" cümlesinde "35k" ile "beş yüz bin" iki ayrı miktarı ifade eder
                bool isCompound = TryParseDigitWordCompound(words[i]) != null;
                i++;

                if (!isCompound)
                {
                    while (i < words.Length
                           && IsPartOfNumberGroup(words, i)
                           && TryParseDigitWordCompound(words[i]) == null)
                    {
                        i++;
                    }
                }

                var group = string.Join(" ", words[start..i]);
                var amount = ExtractAmount(group);
                resultWords.Add(amount?.ToString() ?? group);
            }
            else
            {
                resultWords.Add(words[i]);
                i++;
            }
        }

        return string.Join(" ", resultWords);
    }

    private static List<string>? SplitCompoundNumberWord(string foldedWord)
    {
        if (foldedWord.Length == 0)
            return new List<string>();

        // "binlik", "milyonluk" gibi kalıplarda geriye kalan sıfat eki
        // (-lik/-lık/-luk/-lük) sayısal değer taşımaz, yok sayılabilir.
        if (TrailingSuffixes.Contains(foldedWord))
            return new List<string>();

        // len >= 1: "k" (bin kısaltması) tek karakter olduğu için "klık" gibi
        // rakam olmadan yazılmış bitişik kalıpları da (k + -lık eki) yakalayabilmek için
        // önek denemesi tek karaktere kadar iniyor. FoldedMultipliers'ta "k" dışında
        // tek karakterlik anahtar olmadığından yanlış eşleşme riski yok.
        for (int len = foldedWord.Length; len >= 1; len--)
        {
            var prefix = foldedWord.Substring(0, len);

            if (FoldedBaseNumbers.ContainsKey(prefix) || FoldedMultipliers.ContainsKey(prefix))
            {
                var rest = foldedWord.Substring(len);
                var restSplit = SplitCompoundNumberWord(rest);

                if (restSplit != null)
                {
                    var result = new List<string> { prefix };
                    result.AddRange(restSplit);
                    return result;
                }
            }
        }

        return null;
    }
}