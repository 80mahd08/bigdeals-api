namespace api.Models;

public class OptionAttributCategorie
{
    public int IdOptionAttributCategorie { get; set; }
    public int IdAttributCategorie { get; set; }
    public string Valeur { get; set; } = string.Empty;
    public int OrdreAffichage { get; set; }
}
