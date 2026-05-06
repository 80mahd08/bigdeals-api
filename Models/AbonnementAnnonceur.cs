using System;

namespace api.Models;

public class AbonnementAnnonceur
{
    public long IdAbonnementAnnonceur { get; set; }
    public long IdUtilisateur { get; set; }
    public long IdAnnonceur { get; set; }
    public DateTime DateCreation { get; set; }
}
