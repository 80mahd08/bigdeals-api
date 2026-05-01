using System;
using api.Models.Enums;

namespace api.Models;

public class DemandeAnnonceur
{
    public long IdDemandeAnnonceur { get; set; }
    public long IdUtilisateur { get; set; }
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
