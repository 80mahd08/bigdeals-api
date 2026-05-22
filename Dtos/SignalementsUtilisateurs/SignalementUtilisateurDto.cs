using System;

namespace api.Dtos.SignalementsUtilisateurs;

public class SignalementUtilisateurDto
{
    public long IdSignalement { get; set; }
    public long IdUtilisateurSignale { get; set; }
    public string SignaleNomComplet { get; set; } = string.Empty;
    public string SignaleEmail { get; set; } = string.Empty;
    public string SignaleTelephone { get; set; } = string.Empty;
    
    public long IdUtilisateurReporter { get; set; }
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
}
