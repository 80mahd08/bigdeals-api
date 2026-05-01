using System;

namespace api.Dtos.Annonces;

public class DynamicAttributeFilterDto
{
    public int IdAttributCategorie { get; set; }
    public int? IdOptionAttributCategorie { get; set; }
    public string? ValeurTexte { get; set; }
    public decimal? ValeurNombreMin { get; set; }
    public decimal? ValeurNombreMax { get; set; }
    public DateTime? ValeurDateMin { get; set; }
    public DateTime? ValeurDateMax { get; set; }
    public bool? ValeurBooleen { get; set; }
}
