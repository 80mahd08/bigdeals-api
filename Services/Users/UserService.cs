using System.Threading.Tasks;
using api.Dtos.Users;
using api.Exceptions;
using api.Helpers.Security;
using api.Interfaces.Users;
using api.Interfaces.Annonces;
using api.Services.Storage;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace api.Services.Users;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILocalFileStorageService _fileStorage;
    private readonly IAnnonceRepository _annonceRepository;

    public UserService(
        IUserRepository repository, 
        IPasswordHasher passwordHasher, 
        ILocalFileStorageService fileStorage,
        IAnnonceRepository annonceRepository)
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
        _fileStorage = fileStorage;
        _annonceRepository = annonceRepository;
    }

    public async Task<UserProfileDto> GetCurrentUserAsync(long idUtilisateur)
    {
        var user = await _repository.GetByIdAsync(idUtilisateur);
        if (user == null)
            throw new NotFoundException("User not found.");

        return new UserProfileDto
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
            Adresse = user.Adresse,
            Ville = user.Ville
        };
    }

    public async Task<UserProfileDto> UpdateCurrentUserAsync(long idUtilisateur, UpdateUserProfileDto request)
    {
        var user = await _repository.GetByIdAsync(idUtilisateur);
        if (user == null)
            throw new NotFoundException("User not found.");

        if (request.Photo != null)
        {
            user.PhotoProfilUrl = await _fileStorage.SaveProfilePhotoAsync(request.Photo);
        }

        if (request.Nom != null) user.Nom = request.Nom;
        if (request.Prenom != null) user.Prenom = request.Prenom;
        
        // Always assign these fields to allow clearing them (null updates)
        user.Telephone = string.IsNullOrWhiteSpace(request.Telephone) ? null : request.Telephone;
        user.Adresse = string.IsNullOrWhiteSpace(request.Adresse) ? null : request.Adresse;
        user.Ville = string.IsNullOrWhiteSpace(request.Ville) ? null : request.Ville;

        await _repository.UpdateUserAsync(user);

        return await GetCurrentUserAsync(idUtilisateur);
    }

    public async Task DeleteCurrentUserAsync(long idUtilisateur)
    {
        var user = await _repository.GetByIdAsync(idUtilisateur);
        if (user == null)
            throw new NotFoundException("User not found.");

        await _repository.DeleteUserAsync(idUtilisateur);
    }

    public async Task ChangePasswordAsync(long idUtilisateur, ChangePasswordRequestDto request)
    {
        if (request.NouveauMotDePasse == request.AncienMotDePasse)
            throw new BadRequestException("New password cannot be the same as the old password.");

        if (request.NouveauMotDePasse != request.ConfirmationMotDePasse)
            throw new BadRequestException("New password and confirmation do not match.");

        var user = await _repository.GetByIdAsync(idUtilisateur);
        if (user == null)
            throw new NotFoundException("User not found.");

        if (!_passwordHasher.VerifyPassword(request.AncienMotDePasse, user.MotDePasseHash))
            throw new BadRequestException("Invalid old password.");

        var newHash = _passwordHasher.HashPassword(request.NouveauMotDePasse);
        await _repository.UpdatePasswordAsync(idUtilisateur, newHash);
    }

    public async Task<(byte[] Content, string ContentType, string FileName)> GetProfilePhotoAsync(long idUtilisateur)
    {
        var user = await _repository.GetByIdAsync(idUtilisateur);
        if (user == null)
            throw new NotFoundException("User not found.");

        if (string.IsNullOrEmpty(user.PhotoProfilUrl))
            throw new NotFoundException("User has no profile photo.");

        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", user.PhotoProfilUrl.TrimStart('/'));
        if (!File.Exists(filePath))
            throw new NotFoundException("Profile photo file not found on server.");

        var content = await File.ReadAllBytesAsync(filePath);
        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        var contentType = extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            _ => "application/octet-stream"
        };

        return (content, contentType, Path.GetFileName(filePath));
    }
    public async Task<PublicProfileDto> GetPublicProfileAsync(long idUtilisateur)
    {
        var user = await _repository.GetByIdAsync(idUtilisateur);
        if (user == null)
            throw new NotFoundException("User not found.");

        var userProfile = new UserProfileDto
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
            Adresse = user.Adresse,
            Ville = user.Ville
        };

        // Fetch public ads for this user
        var (ads, _) = await _annonceRepository.GetPagedAsync(1, 50, api.Models.Enums.StatutAnnonce.PUBLIEE, true, idUtilisateur);
        
        var adDtos = new List<api.Dtos.Annonces.AnnonceDto>();
        foreach (var a in ads)
        {
            string? mainImage = a.MainImageUrl;
            if (string.IsNullOrEmpty(mainImage))
            {
                var images = await _annonceRepository.GetImagesByAnnonceIdAsync(a.IdAnnonce);
                mainImage = images.FirstOrDefault(i => i.EstPrincipale)?.Url ?? images.FirstOrDefault()?.Url;
            }

            adDtos.Add(new api.Dtos.Annonces.AnnonceDto
            {
                IdAnnonce = a.IdAnnonce,
                IdUtilisateur = a.IdUtilisateur,
                IdCategorie = a.IdCategorie,
                CategorieNom = a.CategorieNom ?? "Catégorie Inconnue",
                Titre = a.Titre,
                Prix = a.Prix,
                Localisation = a.Localisation,
                Ville = a.Ville,
                Statut = a.Statut.ToString(),
                DateCreation = a.DateCreation,
                MainImageUrl = mainImage,
                AnnonceurNom = $"{user.Prenom} {user.Nom}",
                AnnonceurPhotoUrl = user.PhotoProfilUrl
            });
        }

        return new PublicProfileDto
        {
            User = userProfile,
            Ads = adDtos
        };
    }
}
