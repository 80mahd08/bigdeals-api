using System.Collections.Generic;

namespace api.Dtos.Annonces;

public class AnnonceSearchRequestDto
{
    public string? Keyword { get; set; }
    public int? IdCategorie { get; set; }
    public decimal? PrixMin { get; set; }
    public decimal? PrixMax { get; set; }
    public string? Localisation { get; set; }
    public string? SortBy { get; set; } // newest, price, title
    public string? SortDirection { get; set; } // asc, desc
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 12;
    public List<DynamicAttributeFilterDto> FiltresDynamiques { get; set; } = new();
}
