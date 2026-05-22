using System.ComponentModel.DataAnnotations;

namespace api.Dtos.SignalementsUtilisateurs;

public class UpdateSignalementUtilisateurStatusDto
{
    [Required]
    [Range(2, 3)] // TRAITE (2) ou REJETE (3)
    public int Statut { get; set; }
}
