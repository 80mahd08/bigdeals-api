using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using api.Dtos.Auth;
using api.Dtos.Users;
using api.Exceptions;
using api.Helpers.Security;
using api.Interfaces.Auth;
using api.Models;
using api.Models.Enums;
using api.Interfaces.Email;
using Microsoft.Extensions.Options;

namespace api.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _repository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly IEmailService _emailService;
    private readonly JwtSettings _jwtSettings;

    public AuthService(
        IAuthRepository repository, 
        IPasswordHasher passwordHasher, 
        IJwtTokenGenerator tokenGenerator,
        IEmailService emailService,
        IOptions<JwtSettings> jwtSettings)
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
        _emailService = emailService;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        if (await _repository.EmailExistsAsync(request.Email))
            throw new ConflictException("Email already exists.");

        var user = new Utilisateur
        {
            Nom = request.Nom,
            Prenom = request.Prenom,
            Email = request.Email,
            MotDePasseHash = _passwordHasher.HashPassword(request.Password),
            Role = RoleUtilisateur.CLIENT,
            StatutCompte = StatutCompte.ACTIF,
            DateCreation = DateTime.UtcNow
        };

        var newUserId = await _repository.CreateUserAsync(user);

        var token = _tokenGenerator.GenerateToken(newUserId, user.Email, user.Role.ToString());
        var refreshToken = _tokenGenerator.GenerateRefreshToken();
        var refreshTokenExpiry = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);

        await _repository.UpdateRefreshTokenAsync(newUserId, refreshToken, refreshTokenExpiry);

        return new AuthResponseDto
        {
            Token = token,
            RefreshToken = refreshToken,
            User = new UserProfileDto
            {
                IdUtilisateur = newUserId,
                Nom = user.Nom,
                Prenom = user.Prenom,
                Email = user.Email,
                Role = user.Role.ToString(),
                StatutCompte = user.StatutCompte.ToString(),
                StatutLabel = user.StatutCompte == StatutCompte.ACTIF ? "Actif" : "Bloqué",
                DateCreation = user.DateCreation,
                Ville = user.Ville
            }
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _repository.GetUserByEmailAsync(request.Email);
        if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.MotDePasseHash))
            throw new UnauthorizedException("Invalid email or password.");

        if (user.StatutCompte == StatutCompte.BLOQUE)
            throw new ForbiddenException("Votre compte est bloqué. Veuillez contacter le support.");

        var token = _tokenGenerator.GenerateToken(user.IdUtilisateur, user.Email, user.Role.ToString());
        var refreshToken = _tokenGenerator.GenerateRefreshToken();
        var refreshTokenExpiry = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);

        await _repository.UpdateRefreshTokenAsync(user.IdUtilisateur, refreshToken, refreshTokenExpiry);

        return new AuthResponseDto
        {
            Token = token,
            RefreshToken = refreshToken,
            User = new UserProfileDto
            {
                IdUtilisateur = user.IdUtilisateur,
                Nom = user.Nom,
                Prenom = user.Prenom,
                Email = user.Email,
                Telephone = user.Telephone,
                Role = user.Role.ToString(),
                StatutCompte = user.StatutCompte.ToString(),
                StatutLabel = user.StatutCompte == StatutCompte.ACTIF ? "Actif" : "Bloqué",
                DateCreation = user.DateCreation,
                PhotoProfilUrl = user.PhotoProfilUrl,
                Adresse = user.Adresse,
                Ville = user.Ville
            }
        };
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
    {
        var user = await _repository.GetUserByRefreshTokenAsync(refreshToken);

        if (user == null || user.RefreshTokenExpiry < DateTime.UtcNow)
            throw new UnauthorizedException("Invalid or expired refresh token.");

        var newToken = _tokenGenerator.GenerateToken(user.IdUtilisateur, user.Email, user.Role.ToString());
        var newRefreshToken = _tokenGenerator.GenerateRefreshToken();
        var newRefreshTokenExpiry = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);

        await _repository.UpdateRefreshTokenAsync(user.IdUtilisateur, newRefreshToken, newRefreshTokenExpiry);

        return new AuthResponseDto
        {
            Token = newToken,
            RefreshToken = newRefreshToken,
            User = new UserProfileDto
            {
                IdUtilisateur = user.IdUtilisateur,
                Nom = user.Nom,
                Prenom = user.Prenom,
                Email = user.Email,
                Telephone = user.Telephone,
                Role = user.Role.ToString(),
                StatutCompte = user.StatutCompte.ToString(),
                StatutLabel = user.StatutCompte == StatutCompte.ACTIF ? "Actif" : "Bloqué",
                DateCreation = user.DateCreation,
                PhotoProfilUrl = user.PhotoProfilUrl,
                Adresse = user.Adresse,
                Ville = user.Ville
            }
        };
    }

    public async Task ForgotPasswordAsync(ForgotPasswordRequestDto request)
    {
        var user = await _repository.GetUserByEmailAsync(request.Email);
        if (user == null)
        {
            // For security reasons, don't reveal if the email exists or not
            return;
        }

        // Rate limiting: check last token creation date
        var latestDate = await _repository.GetLatestResetTokenDateAsync(user.IdUtilisateur);
        if (latestDate.HasValue && (DateTime.UtcNow - latestDate.Value).TotalMinutes < 2)
        {
            // Too soon, but return success to avoid enumeration
            return;
        }

        // 1. Generate secure random token
        var tokenBytes = RandomNumberGenerator.GetBytes(32);
        var token = Convert.ToBase64String(tokenBytes)
            .Replace("+", "-").Replace("/", "_").Replace("=", ""); // Base64Url

        // 2. Hash token for storage
        var tokenHash = HashToken(token);

        // 3. Invalidate old tokens
        await _repository.InvalidateUnusedResetTokensAsync(user.IdUtilisateur);

        // 4. Store token hash
        await _repository.CreatePasswordResetTokenAsync(user.IdUtilisateur, tokenHash, DateTime.UtcNow.AddMinutes(30));

        // 5. Send email
        await _emailService.SendResetPasswordEmailAsync(user.Email, token);
    }

    public async Task ResetPasswordAsync(ResetPasswordRequestDto request)
    {
        if (request.NewPassword != request.ConfirmPassword)
            throw new BadRequestException("Les mots de passe ne correspondent pas.");

        // Hash submitted token to compare
        var tokenHash = HashToken(request.Token);

        // Verify token
        var validToken = await _repository.GetValidResetTokenAsync(tokenHash);
        if (validToken == null)
            throw new BadRequestException("Jeton de réinitialisation invalide, expiré ou déjà utilisé.");

        // Get user to check old password
        var user = await _repository.GetByIdAsync(validToken.Value.IdUtilisateur);
        if (user == null)
            throw new BadRequestException("Utilisateur non trouvé.");

        // Check if new password is same as old one
        if (_passwordHasher.VerifyPassword(request.NewPassword, user.MotDePasseHash))
            throw new BadRequestException("Le nouveau mot de passe ne peut pas être identique à l'ancien.");

        // Update password
        var newHash = _passwordHasher.HashPassword(request.NewPassword);
        await _repository.UpdatePasswordHashAsync(validToken.Value.IdUtilisateur, newHash);

        // Mark token as used
        await _repository.MarkResetTokenAsUsedAsync(validToken.Value.IdPasswordResetToken);
    }

    private static string HashToken(string token)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(bytes);
    }
}
