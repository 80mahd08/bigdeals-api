using System;
using System.Collections.Generic;

namespace api.Dtos.Annonces;

public class AnnonceDetailsDto
{
    public long IdAnnonce { get; set; }
    public long IdUtilisateur { get; set; }
    public string AnnonceurNom { get; set; } = string.Empty;
    public string? AnnonceurPhotoUrl { get; set; }
    public string? AnnonceurTelephone { get; set; }
    public int IdCategorie { get; set; }
    public string CategorieNom { get; set; } = string.Empty;
    public string Titre { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Prix { get; set; }
    public string? Localisation { get; set; }
    public string? Ville { get; set; }
    public string Statut { get; set; } = string.Empty;
    public bool EstActive { get; set; }
    public DateTime DateCreation { get; set; }
    public DateTime? DatePublication { get; set; }
    public string? MainImageUrl { get; set; }
    public List<AnnonceAttributeValueDetailsDto> ValeursAttributs { get; set; } = new();
    public List<ImageAnnonceDto> Images { get; set; } = new();
}

public class AnnonceAttributeValueDetailsDto
{
    public int IdAttributCategorie { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Valeur { get; set; } = string.Empty;
    public int? ValeurId { get; set; }
}
