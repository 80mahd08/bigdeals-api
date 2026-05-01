using System.Collections.Generic;

namespace api.Dtos.Categories;

public class CategorySchemaDto
{
    public int IdCategorie { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IconKey { get; set; }
    public List<AttributeCategoryDto> Attributs { get; set; } = new();
}
