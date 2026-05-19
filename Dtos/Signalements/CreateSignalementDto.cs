using System.ComponentModel.DataAnnotations;

namespace api.Dtos.Signalements;

public class CreateSignalementDto
{
    [Required]
    [Range(1, long.MaxValue, ErrorMessage = "L'ID de l'annonce doit être supérieur à 0.")]
    public long IdAnnonce { get; set; }

    [Required]
    [Range(1, 4, ErrorMessage = "Le type de signalement est invalide.")]
    public int TypeSignalement { get; set; }

    [StringLength(1000, ErrorMessage = "Le motif ne peut pas dépasser 1000 caractères.")]
    public string Motif { get; set; } = string.Empty;
}
