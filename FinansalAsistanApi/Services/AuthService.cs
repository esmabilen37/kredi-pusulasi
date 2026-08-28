using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FinansalAsistanApi.Models;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;

namespace FinansalAsistanApi.Services;

public class AuthService : IAuthService
{
    private readonly MongoDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(MongoDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<(AuthResponseDto Response, string RefreshToken)> RegisterAsync(RegisterRequestDto request)
    {
        var existing = await _context.Users
            .Find(u => u.Email == request.Email)
            .FirstOrDefaultAsync();

        if (existing != null)
        {
            throw new InvalidOperationException("Bilgilerinizi kontrol edin.");
        }

        var user = new User
        {
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        await _context.Users.InsertOneAsync(user);

        return await IssueTokensAsync(user);
    }

    public async Task<(AuthResponseDto Response, string RefreshToken)> LoginAsync(LoginRequestDto request)
    {
        var user = await _context.Users
            .Find(u => u.Email == request.Email)
            .FirstOrDefaultAsync();

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Lütfen bilgilerinizi kontrol edin.");
        }

        return await IssueTokensAsync(user);
    }
    public async Task<(AuthResponseDto Response, string RefreshToken)> RefreshAsync(string refreshToken)
    {
       
        var userId = ValidateRefreshTokenAndGetUserId(refreshToken);

        
        var user = await _context.Users
            .Find(u => u.Id == userId)
            .FirstOrDefaultAsync();

        if (user == null || user.RefreshTokenHash == null || user.RefreshTokenExpiresAt == null)
        {
            throw new UnauthorizedAccessException("Geçersiz oturum, lütfen tekrar giriş yapın.");
        }

        
        if (user.RefreshTokenExpiresAt < DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Oturum süresi dolmuş, lütfen tekrar giriş yapın.");
        }

        if (!BCrypt.Net.BCrypt.Verify(refreshToken, user.RefreshTokenHash))
        {
            throw new UnauthorizedAccessException("Geçersiz oturum, lütfen tekrar giriş yapın.");
        }
        
        return await IssueTokensAsync(user);
    }

    private async Task<(AuthResponseDto Response, string RefreshToken)> IssueTokensAsync(User user)
    {
        var accessToken = GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken(user);

        // Refresh tokenı hashleyip veritabanına kaydetme
        user.RefreshTokenHash = BCrypt.Net.BCrypt.HashPassword(refreshToken);
        var refreshExpireDays = int.Parse(_configuration["Jwt:RefreshExpireDays"] ?? "7");
        user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(refreshExpireDays);

        await _context.Users.ReplaceOneAsync(u => u.Id == user.Id, user);

        var response = new AuthResponseDto { Token = accessToken, Email = user.Email };
        return (response, refreshToken);
    }

    private string GenerateAccessToken(User user)
    {
        var secret = _configuration["Jwt:Secret"]
            ?? throw new InvalidOperationException("Jwt:Secret user-secrets içinde bulunamadı.");
        var expireMinutes = int.Parse(_configuration["Jwt:ExpireMinutes"] ?? "60");

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expireMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken(User user)
    {
        var secret = _configuration["Jwt:RefreshSecret"]
            ?? throw new InvalidOperationException("Jwt:RefreshSecret user-secrets içinde bulunamadı.");
        var expireDays = int.Parse(_configuration["Jwt:RefreshExpireDays"] ?? "7");

        var claims = new[]
        {
            new Claim("userId", user.Id)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddDays(expireDays),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string ValidateRefreshTokenAndGetUserId(string refreshToken)
    {
        var secret = _configuration["Jwt:RefreshSecret"]
            ?? throw new InvalidOperationException("Jwt:RefreshSecret user-secrets içinde bulunamadı.");

        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            var principal = tokenHandler.ValidateToken(refreshToken, validationParameters, out _);
            var userId = principal.FindFirst("userId")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("Geçersiz oturum, lütfen tekrar giriş yapın.");
            }

            return userId;
        }
        catch (Exception ex) when (ex is not UnauthorizedAccessException)
        {
            throw new UnauthorizedAccessException("Geçersiz oturum, lütfen tekrar giriş yapın.");
        }
    }
}