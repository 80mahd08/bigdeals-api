using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace api.Dtos.Annonces;

public class UpdateAnnonceFormDto
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
    
    /// <summary>
    /// JSON string(s) containing the list of AnnonceAttributeValueDto.
    /// </summary>
    public List<string> ValeursJson { get; set; } = new();
    
    public List<IFormFile> NewImages { get; set; } = new();
    
    public List<long> ImagesToDelete { get; set; } = new();
}
