using System;
using api.Models.Enums;

namespace api.Models;

public class ContactAnnonceur
{
    public long IdContactAnnonceur { get; set; }
    public long? IdUtilisateur { get; set; }
    public long IdAnnonce { get; set; }
    public long IdAnnonceur { get; set; }
    public TypeContact TypeContact { get; set; }
    public DateTime DateContact { get; set; }
}
