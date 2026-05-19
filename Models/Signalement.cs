using System;
using api.Models.Enums;

namespace api.Models;

public class Signalement
{
    public long IdSignalement { get; set; }
    public long IdAnnonce { get; set; }
    public long IdUtilisateur { get; set; }
    public TypeSignalement TypeSignalement { get; set; }
    public string Motif { get; set; } = string.Empty;
    public StatutSignalement Statut { get; set; } = StatutSignalement.EN_ATTENTE;
    public DateTime DateCreation { get; set; } = DateTime.UtcNow;
    public DateTime? DateTraitement { get; set; }
    public long? IdAdminTraitant { get; set; }
}
