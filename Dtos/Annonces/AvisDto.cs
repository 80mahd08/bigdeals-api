using System;

namespace api.Dtos.Annonces;

public class AvisDto
{
    public long IdAvis { get; set; }
    public long IdAnnonce { get; set; }
    public long IdUtilisateur { get; set; }
    public string NomUtilisateur { get; set; } = string.Empty;
    public string PrenomUtilisateur { get; set; } = string.Empty;
    public int Note { get; set; }
    public string Commentaire { get; set; } = string.Empty;
    public string? PhotoProfilUrl { get; set; }
    public string TitreAnnonce { get; set; } = string.Empty;
    public DateTime DateCreation { get; set; }
    public DateTime? DateModification { get; set; }
}

public class CreateAvisDto
{
    public int Note { get; set; }
    public string Commentaire { get; set; } = string.Empty;
}
