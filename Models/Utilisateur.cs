using System;
using api.Models.Enums;

namespace api.Models;

public class Utilisateur
{
    public long IdUtilisateur { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telephone { get; set; }
    public string MotDePasseHash { get; set; } = string.Empty;
    public RoleUtilisateur Role { get; set; }
    public StatutCompte StatutCompte { get; set; }
    public DateTime DateCreation { get; set; }
    public DateTime? DerniereConnexion { get; set; }
    public string? PhotoProfilUrl { get; set; }
    public string? Adresse { get; set; }
    public string? Ville { get; set; }
    public bool EstActif { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }
}
