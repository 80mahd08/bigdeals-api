using System;

namespace api.Models;

public class ValeurAttributAnnonce
{
    public long IdValeurAttributAnnonce { get; set; }
    public long IdAnnonce { get; set; }
    public int IdAttributCategorie { get; set; }
    public int? IdOptionAttributCategorie { get; set; }
    public string? ValeurTexte { get; set; }
    public decimal? ValeurNombre { get; set; }
    public DateTime? ValeurDate { get; set; }
    public bool? ValeurBooleen { get; set; }
}
