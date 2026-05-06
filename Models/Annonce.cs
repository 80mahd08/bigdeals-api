using System;
using api.Models.Enums;

namespace api.Models;

public class Annonce
{
    public long IdAnnonce { get; set; }
    public long IdUtilisateur { get; set; }
    public int IdCategorie { get; set; }
    public string Titre { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Prix { get; set; }
    public string? Localisation { get; set; }
    public StatutAnnonce Statut { get; set; }
    public DateTime DateCreation { get; set; }
    public DateTime? DatePublication { get; set; }
    public DateTime? DateExpiration { get; set; }
    public bool EstActive { get; set; }

    // Search Result Helpers (Populated in Repository.SearchAsync)
    public string? CategorieNom { get; set; }
    public string? MainImageUrl { get; set; }
    public string? AnnonceurNom { get; set; }
    public string? AnnonceurPhotoUrl { get; set; }
    public string? AnnonceurTelephone { get; set; }
    public string? Ville { get; set; }
}
