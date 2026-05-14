using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace api.Dtos.DemandesAnnonceur;

public class CreateDemandeAnnonceurDto
{
    [Required]
    public IFormFile Document { get; set; } = null!;
}

public class DemandeAnnonceurDto
{
    public long IdDemandeAnnonceur { get; set; }
    public long IdUtilisateur { get; set; }
    
    // User Details (Joined)
    public string NomUtilisateur { get; set; } = string.Empty;
    public string PrenomUtilisateur { get; set; } = string.Empty;
    public string NomCompletUtilisateur { get; set; } = string.Empty;
    public string EmailUtilisateur { get; set; } = string.Empty;
    public string? PhotoProfilUrl { get; set; }

    public string Statut { get; set; } = string.Empty;
    public string DocumentUrl { get; set; } = string.Empty;
    public string DocumentNomOriginal { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;
    public long DocumentTaille { get; set; }
    public string? MotifRejet { get; set; }
    public DateTime DateDemande { get; set; }
    public DateTime? DateTraitement { get; set; }
    public long? IdAdminTraitant { get; set; }
}

public class RejectDemandeAnnonceurDto
{
    public string MotifRejet { get; set; } = string.Empty;
}
