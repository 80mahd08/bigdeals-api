using api.Models.Enums;

namespace api.Models;

public class AttributCategorie
{
    public int IdAttributCategorie { get; set; }
    public int IdCategorie { get; set; }
    public string Nom { get; set; } = string.Empty;
    public TypeDonneeAttribut TypeDonnee { get; set; }
    public int OrdreAffichage { get; set; }
    public string? Placeholder { get; set; }
    public bool EstPlage { get; set; }
}
