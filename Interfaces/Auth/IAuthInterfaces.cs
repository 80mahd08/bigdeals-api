using System.Threading.Tasks;
using api.Dtos.Auth;
using api.Models;

namespace api.Interfaces.Auth;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
    Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
    Task ForgotPasswordAsync(ForgotPasswordRequestDto request);
    Task ResetPasswordAsync(ResetPasswordRequestDto request);
}

public interface IAuthRepository
{
    Task<bool> EmailExistsAsync(string email);
    Task<Utilisateur?> GetUserByEmailAsync(string email);
    Task<long> CreateUserAsync(Utilisateur user);
    
    // Password Reset
    Task InvalidateUnusedResetTokensAsync(long idUtilisateur);
    Task CreatePasswordResetTokenAsync(long idUtilisateur, string tokenHash, DateTime expiration);
    Task<(long IdPasswordResetToken, long IdUtilisateur)?> GetValidResetTokenAsync(string tokenHash);
    Task MarkResetTokenAsUsedAsync(long idPasswordResetToken);
    Task UpdatePasswordHashAsync(long idUtilisateur, string newPasswordHash);
    Task<DateTime?> GetLatestResetTokenDateAsync(long idUtilisateur);

    // Refresh Token
    Task UpdateRefreshTokenAsync(long idUtilisateur, string? refreshToken, DateTime? expiry);
    Task<Utilisateur?> GetUserByRefreshTokenAsync(string refreshToken);
}
