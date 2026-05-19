using System.ComponentModel.DataAnnotations;

namespace api.Dtos.Signalements;

public class UpdateSignalementStatusDto
{
    [Required]
    [Range(2, 3, ErrorMessage = "Le statut doit être Traité (2) ou Rejeté (3).")]
    public int Statut { get; set; }
}
