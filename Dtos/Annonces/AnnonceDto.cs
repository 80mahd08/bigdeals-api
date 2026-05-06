using System;

namespace api.Dtos.Annonces;

public class AnnonceDto
{
    public long IdAnnonce { get; set; }
    public long IdUtilisateur { get; set; }
    public int IdCategorie { get; set; }
    public string CategorieNom { get; set; } = string.Empty;
    public string Titre { get; set; } = string.Empty;
    public decimal Prix { get; set; }
    public string? Localisation { get; set; }
    public string? Ville { get; set; }
    public string Statut { get; set; } = string.Empty;
    public DateTime DateCreation { get; set; }
    public string? MainImageUrl { get; set; }
    public string? AnnonceurNom { get; set; }
    public string? AnnonceurPhotoUrl { get; set; }
    public string? AnnonceurTelephone { get; set; }
}
