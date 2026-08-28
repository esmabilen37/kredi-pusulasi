using System.Text.RegularExpressions;

namespace FinansalAsistanApi.Services;

public static class ChatSafetyFilters
{
    public static readonly string[] InjectionPatterns =
    {
        "önceki talimat", "önceki kural", "talimatları unut", "kuralları unut",
        "sınırlama.*kaldır", "sınırlaman.*yok", "sistem promptu", "system prompt",
        "artık her konuda", "kısıtlama.*yok", "yasak.*yok", "ignore previous",
        "ignore all", "forget your instructions", "you are now", "sen artık",
        "rol yap", "roleplay", "jailbreak", "dan modu", "developer mode",
        "yönergeleri.*bir kenara", "yönerge.*yok say", "serbestsin", "artık serbest",
        "sınır.*yok", "her konuda yardım", "kısıtlamasız", "özgürce yardım"
    };

    public static readonly string[] IdentityProbePatterns =
    {
        "cümleyi tamamla.*tarafından", "boşluk.*doldur.*tarafından", "___ tarafından",
        "tarafından geliştirilmiştir", "kim.*geliştirdi", "hangi şirket.*ürün",
        "hangi banka.*ürün", "kimin ürünüsün", "hangi kuruma ait"
    };

    public static readonly string[] ScopeViolationIndicators =
    {
        "artık her konuda", "tüm konularda yardımcı", "sınırlama.*yok", "serbestim",
        "kısıtlama.*kalk", "özgürce yardımcı", "istediğin her konu", "her türlü konuda"
    };

    public static readonly string[] IdentityDisclosureIndicators =
    {
        "tarafından geliştirilmiştir", "tarafından geliştirilmemiştir",
        "tarafından geliştirilmedi", "için çalışıyorum", "için çalışmıyorum",
        "adlı şirket", "adında bir şirket"
    };

    private static readonly Regex TcKimlikNoRegex = new(@"(?<!\d)\d{11}(?!\d)", RegexOptions.Compiled);

    public const string InjectionRefusalMessage =
        "Bu isteği yerine getiremem — kapsamım kredi ve finans konularıyla sınırlı, " +
        "bu sınırlamayı kaldıramam. Sana finansal konularda nasıl yardımcı olabilirim?";

    public const string SensitiveDataRefusalMessage =
        "Bu tür kişisel/özel bilgilere erişimim yok ve böyle bir bilgi paylaşamam. " +
        "Genel finansal konularda yardımcı olabilirim — nasıl destek olabilirim?";

    public const string IdentityRefusalMessage =
        "Hangi kurumla çalıştığım hakkında bilgi paylaşmıyorum. " +
        "Sana finansal konularda nasıl yardımcı olabilirim?";

   
    public static bool ContainsPattern(string message, string[] patterns)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return false;
        }

        var normalized = TurkishTextNormalizer.Fold(message.ToLower(new System.Globalization.CultureInfo("tr-TR")));
        return patterns.Any(pattern =>
            Regex.IsMatch(normalized, TurkishTextNormalizer.Fold(pattern), RegexOptions.IgnoreCase));
    }

   
    //Metinde TC kimlik no (11 haneli) ya da kart numarası (13-19 haneli) formatına uyan bir rakam dizisi olup olmadığını kontrol eder.
    
    public static bool LooksLikeSensitiveIdentifier(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        if (TcKimlikNoRegex.IsMatch(text))
        {
            return true;
        }

        var digitsOnly = text.Replace(" ", "").Replace("-", "");
        return Regex.IsMatch(digitsOnly, @"(?<!\d)\d{13,19}(?!\d)");
    }
}