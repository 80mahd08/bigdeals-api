using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace api.Dtos.Users;

public class UserProfileDto
{
    public long IdUtilisateur { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telephone { get; set; }
    public string Role { get; set; } = string.Empty;
    public string StatutCompte { get; set; } = string.Empty;
    public DateTime DateCreation { get; set; }
    public string? PhotoProfilUrl { get; set; }
    public string? Adresse { get; set; }
    public string? Ville { get; set; }
}

public class UpdateUserProfileDto
{
    public string? Nom { get; set; }
    public string? Prenom { get; set; }
    public string? Telephone { get; set; }
    public string? Adresse { get; set; }
    public string? Ville { get; set; }
    public IFormFile? Photo { get; set; }
}

public class ChangePasswordRequestDto
{
    [Required]
    public string AncienMotDePasse { get; set; } = null!;
    
    [Required]
    public string NouveauMotDePasse { get; set; } = null!;
    
    [Required]
    [Compare(nameof(NouveauMotDePasse), ErrorMessage = "Passwords do not match.")]
    public string ConfirmationMotDePasse { get; set; } = null!;
}
