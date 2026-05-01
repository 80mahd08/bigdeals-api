using System;

namespace api.Dtos.Annonces;

public class AnnonceAttributeValueDto
{
    public int IdAttributCategorie { get; set; }
    public int? IdOptionAttributCategorie { get; set; }
    public string? ValeurTexte { get; set; }
    public decimal? ValeurNombre { get; set; }
    public DateTime? ValeurDate { get; set; }
    public bool? ValeurBooleen { get; set; }
}
