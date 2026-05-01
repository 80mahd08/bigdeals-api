using System;

namespace api.Dtos.Annonces;

public class ImageAnnonceDto
{
    public long IdImageAnnonce { get; set; }
    public string Url { get; set; } = string.Empty;
    public int OrdreAffichage { get; set; }
    public bool EstPrincipale { get; set; }
}
