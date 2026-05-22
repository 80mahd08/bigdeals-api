using System;
using api.Models.Enums;

namespace api.Models;

public class SignalementUtilisateur
{
    public long IdSignalement { get; set; }
    public long IdUtilisateurSignale { get; set; }
    public long IdUtilisateurReporter { get; set; }
    public TypeSignalement TypeSignalement { get; set; }
    public string Motif { get; set; } = string.Empty;
    public StatutSignalement Statut { get; set; } = StatutSignalement.EN_ATTENTE;
    public DateTime DateCreation { get; set; } = DateTime.UtcNow;
    public DateTime? DateTraitement { get; set; }
    public long? IdAdminTraitant { get; set; }
}
