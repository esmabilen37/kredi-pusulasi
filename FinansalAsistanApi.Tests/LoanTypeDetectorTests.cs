using FinansalAsistanApi.Models;
using FinansalAsistanApi.Services;
using Xunit;
namespace FinansalAsistanApi.Tests;


public class LoanTypeDetectorTests
{
    [Fact]
    public void Detect_ArabaGeciyor_TasitDondurur()
    {
        // Arrange
        var text = "200 bin araba kredisi istiyorum";

        // Act
        var result = LoanTypeDetector.Detect(text);

        // Assert
        Assert.Equal(LoanType.Tasit, result);
    }

    [Fact]
    public void Detect_EvGeciyor_KonutDondurur()
    {
        // Arrange
        var text = "ev almak için kredi istiyorum";

        // Act
        var result = LoanTypeDetector.Detect(text);

        // Assert
        Assert.Equal(LoanType.Konut, result);
    }

    [Fact]
    public void Detect_HicbiriGecmiyor_IhtiyacDondurur()
    {
        // Arrange
        var text = "200 bin kredi istiyorum";

        // Act
        var result = LoanTypeDetector.Detect(text);

        // Assert
        Assert.Equal(LoanType.Ihtiyac, result);
    }

    [Fact]
    public void Detect_HemTasitHemKonutGeciyor_NullDondurur()
    {
        // Arrange
        var text = "ev ve araba için kredi istiyorum";

        // Act
        var result = LoanTypeDetector.Detect(text);

        // Assert
        Assert.Null(result);
    }
    
}