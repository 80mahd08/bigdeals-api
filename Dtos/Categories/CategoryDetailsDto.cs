using System;

namespace api.Dtos.Categories;

public class CategoryDetailsDto
{
    public int IdCategorie { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IconKey { get; set; }
    public int OrdreAffichage { get; set; }
    public DateTime DateCreation { get; set; }
}
