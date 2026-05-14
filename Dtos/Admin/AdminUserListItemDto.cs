using System;

namespace api.Dtos.Admin;

public class AdminUserListItemDto
{
    public long IdUtilisateur { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string NomComplet => $"{Prenom} {Nom}";
    public string Email { get; set; } = string.Empty;
    public string? Telephone { get; set; }
    public string? Ville { get; set; }
    public int Role { get; set; }
    public string RoleLabel => Role == 1 ? "Client" : Role == 2 ? "Annonceur" : "Inconnu";
    public int StatutCompte { get; set; }
    public string StatutLabel => StatutCompte == 1 ? "Actif" : StatutCompte == 2 ? "Bloqué" : "Inconnu";
    public DateTime DateCreation { get; set; }
    public int NombreAnnonces { get; set; }
    public string? PhotoProfilUrl { get; set; }
}
