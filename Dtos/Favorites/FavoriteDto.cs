using System;

namespace api.Dtos.Favorites;

public class FavoriteDto
{
    public long IdAnnonce { get; set; }
    public long IdUtilisateur { get; set; }
    public string AnnonceurNom { get; set; } = string.Empty;
    public string? AnnonceurPhotoUrl { get; set; }
    public int IdCategorie { get; set; }
    public string CategorieNom { get; set; } = string.Empty;
    public string Titre { get; set; } = null!;
    public decimal Prix { get; set; }
    public string Localisation { get; set; } = null!;
    public string? Ville { get; set; }
    public string? MainImageUrl { get; set; }
    public DateTime DateCreation { get; set; } // When the favorite was created
}
