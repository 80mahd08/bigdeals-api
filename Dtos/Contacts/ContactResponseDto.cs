using System;
using api.Models.Enums;

namespace api.Dtos.Contacts;

public class ContactResponseDto
{
    public long IdContactAnnonceur { get; set; }
    
    // Annonce Details
    public long IdAnnonce { get; set; }
    public string TitreAnnonce { get; set; } = null!;
    
    // Annonceur Details (Target)
    public long IdAnnonceur { get; set; }
    public string NomAnnonceur { get; set; } = null!;
    public string PrenomAnnonceur { get; set; } = null!;
    
    // User Details (Initiator, nullable for visitors)
    public long? IdUtilisateur { get; set; }
    public string? NomUtilisateur { get; set; }
    public string? PrenomUtilisateur { get; set; }
    
    public TypeContact TypeContact { get; set; }
    public DateTime DateContact { get; set; }
}
