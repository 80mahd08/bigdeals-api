using System;
using System.Threading.Tasks;
using api.Dtos.Auth;
using api.Dtos.Users;
using api.Exceptions;
using api.Helpers.Security;
using api.Interfaces.Auth;
using api.Models;
using api.Models.Enums;

namespace api.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _repository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _tokenGenerator;

    public AuthService(IAuthRepository repository, IPasswordHasher passwordHasher, IJwtTokenGenerator tokenGenerator)
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
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
            DateCreation = DateTime.UtcNow,
            EstActif = true
        };

        var newUserId = await _repository.CreateUserAsync(user);

        var token = _tokenGenerator.GenerateToken(newUserId, user.Email, user.Role.ToString());

        return new AuthResponseDto
        {
            Token = token,
            User = new UserProfileDto
            {
                IdUtilisateur = newUserId,
                Nom = user.Nom,
                Prenom = user.Prenom,
                Email = user.Email,
                Role = user.Role.ToString(),
                StatutCompte = user.StatutCompte.ToString(),
                DateCreation = user.DateCreation
            }
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _repository.GetUserByEmailAsync(request.Email);
        if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.MotDePasseHash))
            throw new UnauthorizedException("Invalid email or password.");

        if (user.StatutCompte == StatutCompte.INACTIF || user.StatutCompte == StatutCompte.BLOQUE || !user.EstActif)
            throw new ForbiddenException("Account is not active.");

        var token = _tokenGenerator.GenerateToken(user.IdUtilisateur, user.Email, user.Role.ToString());

        return new AuthResponseDto
        {
            Token = token,
            User = new UserProfileDto
            {
                IdUtilisateur = user.IdUtilisateur,
                Nom = user.Nom,
                Prenom = user.Prenom,
                Email = user.Email,
                Telephone = user.Telephone,
                Role = user.Role.ToString(),
                StatutCompte = user.StatutCompte.ToString(),
                DateCreation = user.DateCreation,
                PhotoProfilUrl = user.PhotoProfilUrl,
                Adresse = user.Adresse
            }
        };
    }
}
