using System;

namespace api.Models;

public class ImageAnnonce
{
    public long IdImageAnnonce { get; set; }
    public long IdAnnonce { get; set; }
    public string Url { get; set; } = string.Empty;
    public int OrdreAffichage { get; set; }
    public bool EstPrincipale { get; set; }
    public DateTime DateCreation { get; set; }
}
