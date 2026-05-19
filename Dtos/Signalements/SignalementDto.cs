using System;

namespace api.Dtos.Signalements;

public class SignalementDto
{
    public long IdSignalement { get; set; }
    public long IdAnnonce { get; set; }
    public string TitreAnnonce { get; set; } = string.Empty;
    public string DescriptionAnnonce { get; set; } = string.Empty;
    public string CategorieAnnonce { get; set; } = string.Empty;
    public string AnnonceurNom { get; set; } = string.Empty;
    public string AnnonceurTelephone { get; set; } = string.Empty;
    public long IdUtilisateur { get; set; }
    public string ReporterNomComplet { get; set; } = string.Empty;
    public string ReporterEmail { get; set; } = string.Empty;
    public int TypeSignalement { get; set; }
    public string TypeSignalementLabel { get; set; } = string.Empty;
    public string Motif { get; set; } = string.Empty;
    public int Statut { get; set; }
    public string StatutLabel { get; set; } = string.Empty;
    public DateTime DateCreation { get; set; }
    public DateTime? DateTraitement { get; set; }
    public long? IdAdminTraitant { get; set; }
    
    // Detailed ad data for moderation
    public List<Annonces.ImageAnnonceDto> Images { get; set; } = new();
    public List<Annonces.AnnonceAttributeValueDetailsDto> ValeursAttributs { get; set; } = new();
}
