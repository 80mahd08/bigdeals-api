using System.Threading.Tasks;
using api.Dtos.Auth;
using api.Models;

namespace api.Interfaces.Auth;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
}

public interface IAuthRepository
{
    Task<bool> EmailExistsAsync(string email);
    Task<Utilisateur?> GetUserByEmailAsync(string email);
    Task<long> CreateUserAsync(Utilisateur user);
}
