using System;

namespace api.Models;

public class Avis
{
    public long IdAvis { get; set; }
    public long IdAnnonce { get; set; }
    public long IdUtilisateur { get; set; }
    public int Note { get; set; }
    public string Commentaire { get; set; } = string.Empty;
    public DateTime DateCreation { get; set; }
    public DateTime? DateModification { get; set; }
    public bool EstActif { get; set; }

    // Navigation-like properties for mapping
    public string? NomUtilisateur { get; set; }
    public string? PrenomUtilisateur { get; set; }
    public string? PhotoProfilUrl { get; set; }
    public string? TitreAnnonce { get; set; }
}
