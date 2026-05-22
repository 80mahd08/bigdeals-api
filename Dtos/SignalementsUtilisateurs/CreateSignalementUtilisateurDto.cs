using System.ComponentModel.DataAnnotations;

namespace api.Dtos.SignalementsUtilisateurs;

public class CreateSignalementUtilisateurDto
{
    [Required]
    public long IdUtilisateurSignale { get; set; }

    [Required]
    [Range(1, 4)]
    public int TypeSignalement { get; set; }

    [MaxLength(1000)]
    public string Motif { get; set; } = string.Empty;
}
