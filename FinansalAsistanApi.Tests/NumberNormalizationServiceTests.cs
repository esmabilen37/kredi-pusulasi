using FinansalAsistanApi.Services;
using Xunit;

namespace FinansalAsistanApi.Tests;

public class NumberNormalizationServiceTests
{
    [Fact]
    public void ExtractAmount_IkiYuzBin_DogruTutariDondurur()
    {
        // Arrange
        var service = new NumberNormalizationService();
        var text = "iki yüz bin TL istiyorum";

        // Act
        var result = service.ExtractAmount(text);

        // Assert
        Assert.Equal(200000, result);
    }
    [Fact]
    public void ExtractAmount_OnBesBin_DogruTutariDondurur()
    {
        // Arrange
        var service = new NumberNormalizationService();
        var text = "on beş bin TL istiyorum";

        // Act
        var result = service.ExtractAmount(text);

        // Assert
        Assert.Equal(15000, result);
    }
    [Fact]
    public void ExtractAmount_SayiIcermeyenMetin_NullDondurur()
    {
        // Arrange
        var service = new NumberNormalizationService();
        var text = "kredi istiyorum";

        // Act
        var result = service.ExtractAmount(text);

        // Assert
        Assert.Null(result);
    }
    [Fact]
    public void ExtractAmount_IkiYuzBinBesYuz_DogruTutariDondurur()
    {
        // Arrange
        var service = new NumberNormalizationService();
        var text = "iki yüz bin beş yüz TL istiyorum";

        // Act
        var result = service.ExtractAmount(text);

        // Assert
        Assert.Equal(200500, result);
    }
    [Fact]
    public void ExtractAmount_DuzRakam_DogruTutariDondurur()
    {
        // Not: Artık düz rakamlar da destekleniyor (rakam+kelime karışık yazımları desteklemek için eklendi)
        var service = new NumberNormalizationService();
        var text = "200000 TL istiyorum";

        // Act
        var result = service.ExtractAmount(text);

        // Assert
        Assert.Equal(200000, result);
    }
    [Fact]
    public void NormalizeSentence_CumleIcindeKelimeBazliSayi_DuzRakamaCevirir()
    {
        // Arrange
        var service = new NumberNormalizationService();
        var text = "aylık iki yüz bin TL kazanıyorum";

        // Act
        var result = service.NormalizeSentence(text);

        // Assert
        Assert.Equal("aylık 200000 TL kazanıyorum", result);
    }

    [Fact]
    public void NormalizeSentence_DuzRakamIcerenCumle_DegismedenKalir()
    {
        // Arrange
        var service = new NumberNormalizationService();
        var text = "45000 TL kazanıyorum";

        // Act
        var result = service.NormalizeSentence(text);

        // Assert
        Assert.Equal("45000 TL kazanıyorum", result);
    }

    [Fact]
    public void NormalizeSentence_BirdenFazlaSayiGrubu_HepsiniCevirir()
    {
        // Arrange
        var service = new NumberNormalizationService();
        var text = "elli bin TL gelirim var yüz bin TL kredi istiyorum";

        // Act
        var result = service.NormalizeSentence(text);

        // Assert
        Assert.Equal("50000 TL gelirim var 100000 TL kredi istiyorum", result);
    }
    [Fact]
    public void ExtractAmount_BitisikYaziliBilesikSayi_DogruTutariDondurur()
    {
        // Arrange
        var service = new NumberNormalizationService();
        var text = "yuzelli bin TL istiyorum";

        // Act
        var result = service.ExtractAmount(text);

        // Assert
        Assert.Equal(150000, result);
    }

    [Fact]
    public void ExtractAmount_BitisikVadeKelimesi_DogruSayiyiDondurur()
    {
        // Arrange
        var service = new NumberNormalizationService();
        var text = "oniki ay vadeli";

        // Act
        var result = service.ExtractAmount(text);

        // Assert
        Assert.Equal(12, result);
    }

    [Fact]
    public void NormalizeSentence_BitisikYaziliBilesikSayi_CumleIcindeDeCevirir()
    {
        // Arrange
        var service = new NumberNormalizationService();
        var text = "oniki ay vadeyle yuzelli bin TL kredi istiyorum";

        // Act
        var result = service.NormalizeSentence(text);

        // Assert
        Assert.Equal("12 ay vadeyle 150000 TL kredi istiyorum", result);
    }
    [Fact]
    public void ExtractAmount_RakamKelimeBitisik_DogruTutariDondurur()
    {
        // Arrange
        var service = new NumberNormalizationService();
        var text = "15bin TL kredi istiyorum";

        // Act
        var result = service.ExtractAmount(text);

        // Assert
        Assert.Equal(15000, result);
    }

    [Fact]
    public void ExtractAmount_BuyukRakamKelimeBitisik_DogruTutariDondurur()
    {
        // Arrange
        var service = new NumberNormalizationService();
        var text = "200bin TL geliri var";

        // Act
        var result = service.ExtractAmount(text);

        // Assert
        Assert.Equal(200000, result);
    }

    [Fact]
    public void ExtractAmount_KKisaltmasi_DogruTutariDondurur()
    {
        // Arrange
        var service = new NumberNormalizationService();
        var text = "15k TL istiyorum";

        // Act
        var result = service.ExtractAmount(text);

        // Assert
        Assert.Equal(15000, result);
    }

    [Fact]
    public void NormalizeSentence_RakamKelimeBitisik_CumleIcindeDeCevirir()
    {
        // Arrange
        var service = new NumberNormalizationService();
        var text = "aylık gelirim 15bin TL, 200bin TL kredi istiyorum";

        // Act
        var result = service.NormalizeSentence(text);

        // Assert
        Assert.Equal("aylık gelirim 15000 TL, 200000 TL kredi istiyorum", result);
    }
    
}

