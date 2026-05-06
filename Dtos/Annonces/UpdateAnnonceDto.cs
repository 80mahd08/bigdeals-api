using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace api.Dtos.Annonces;

public class UpdateAnnonceDto
{
    [Required]
    [MaxLength(150)]
    public string Titre { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    [Range(0, double.MaxValue)]
    public decimal Prix { get; set; }
    
    [MaxLength(255)]
    public string? Localisation { get; set; }
    
    public List<AnnonceAttributeValueDto> Valeurs { get; set; } = new();
}
