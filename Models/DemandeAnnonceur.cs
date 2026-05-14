using System;
using api.Models.Enums;

namespace api.Models;

public class DemandeAnnonceur
{
    public long IdDemandeAnnonceur { get; set; }
    public long IdUtilisateur { get; set; }
    
    // Joined fields from Utilisateurs
    public string? NomUtilisateur { get; set; }
    public string? PrenomUtilisateur { get; set; }
    public string? PhotoProfilUrl { get; set; }
    public string? EmailUtilisateur { get; set; }

    public StatutDemandeAnnonceur Statut { get; set; }
    public string DocumentUrl { get; set; } = string.Empty;
    public string DocumentNomOriginal { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;
    public long DocumentTaille { get; set; }
    public string? MotifRejet { get; set; }
    public DateTime DateDemande { get; set; }
    public DateTime? DateTraitement { get; set; }
    public long? IdAdminTraitant { get; set; }
}
