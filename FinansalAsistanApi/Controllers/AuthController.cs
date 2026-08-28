using FinansalAsistanApi.Models;
using FinansalAsistanApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinansalAsistanApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;
    private readonly IConfiguration _configuration;

    public AuthController(IAuthService authService, ILogger<AuthController> logger, IConfiguration configuration)
    {
        _authService = authService;
        _logger = logger;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        try
        {
            var (response, refreshToken) = await _authService.RegisterAsync(request);
            SetTokenCookies(response.Token, refreshToken);
            return Ok(new { email = response.Email });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Register endpoint'inde beklenmeyen hata oluştu.");
            return StatusCode(500, new { error = "Kayıt sırasında bir hata oluştu." });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            var (response, refreshToken) = await _authService.LoginAsync(request);
            SetTokenCookies(response.Token, refreshToken);
            return Ok(new { email = response.Email });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login endpoint'inde beklenmeyen hata oluştu.");
            return StatusCode(500, new { error = "Giriş sırasında bir hata oluştu." });
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var incomingRefreshToken = Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(incomingRefreshToken))
        {
            return Unauthorized(new { error = "Oturum bulunamadı, lütfen giriş yapın." });
        }

        try
        {
            var (response, newRefreshToken) = await _authService.RefreshAsync(incomingRefreshToken);
            SetTokenCookies(response.Token, newRefreshToken);
            return Ok(new { email = response.Email });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Refresh endpoint'inde beklenmeyen hata oluştu.");
            return StatusCode(500, new { error = "Oturum yenilenirken bir hata oluştu." });
        }
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("accessToken");
        Response.Cookies.Delete("refreshToken");
        return Ok(new { message = "Çıkış yapıldı." });
    }

    private void SetTokenCookies(string accessToken, string refreshToken)
    {
        var accessExpireMinutes = int.Parse(_configuration["Jwt:ExpireMinutes"] ?? "15");
        var refreshExpireDays = int.Parse(_configuration["Jwt:RefreshExpireDays"] ?? "30");

        Response.Cookies.Append("accessToken", accessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddMinutes(accessExpireMinutes)
        });

        Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddDays(refreshExpireDays)
        });
    }
}