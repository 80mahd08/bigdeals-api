using System.Collections.Generic;

namespace api.Dtos.Categories;

public class AttributeCategoryDto
{
    public int IdAttributCategorie { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string TypeDonnee { get; set; } = string.Empty;
    public bool Obligatoire { get; set; }
    public bool Filtrable { get; set; }
    public int OrdreAffichage { get; set; }
    public string? Placeholder { get; set; }
    public bool EstPlage { get; set; }
    public List<OptionAttributeCategoryDto> Options { get; set; } = new();
}
