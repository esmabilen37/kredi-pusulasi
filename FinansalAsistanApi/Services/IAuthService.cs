using FinansalAsistanApi.Models;

namespace FinansalAsistanApi.Services;

public interface IAuthService
{
    Task<(AuthResponseDto Response, string RefreshToken)> RegisterAsync(RegisterRequestDto request);
    Task<(AuthResponseDto Response, string RefreshToken)> LoginAsync(LoginRequestDto request);
    Task<(AuthResponseDto Response, string RefreshToken)> RefreshAsync(string refreshToken);
    Task LogoutAsync(string? refreshToken);
}