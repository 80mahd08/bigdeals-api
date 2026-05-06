using System;

namespace api.Models;

public class Favori
{
    public long IdFavori { get; set; }
    public long IdUtilisateur { get; set; }
    public long IdAnnonce { get; set; }
    public DateTime DateCreation { get; set; }
}
