using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Categories;
using api.Exceptions;
using api.Interfaces.Categories;
using api.Models.Enums;

namespace api.Services.Categories;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repository;

    public CategoryService(ICategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<CategoryDto>> GetAllCategoriesAsync()
    {
        var categories = await _repository.GetAllAsync();
        return categories.Select(c => new CategoryDto
        {
            IdCategorie = c.IdCategorie,
            Nom = c.Nom,
            Description = c.Description,
            IconKey = c.IconKey,
            OrdreAffichage = c.OrdreAffichage
        }).ToList();
    }

    public async Task<CategoryDetailsDto> GetCategoryByIdAsync(int id)
    {
        var category = await _repository.GetByIdAsync(id);
        if (category == null) throw new NotFoundException($"Category with ID {id} not found.");

        return new CategoryDetailsDto
        {
            IdCategorie = category.IdCategorie,
            Nom = category.Nom,
            Description = category.Description,
            IconKey = category.IconKey,
            OrdreAffichage = category.OrdreAffichage,
            DateCreation = category.DateCreation
        };
    }

    public async Task<IReadOnlyList<AttributeCategoryDto>> GetCategoryAttributesAsync(int id)
    {
        var category = await _repository.GetByIdAsync(id);
        if (category == null) throw new NotFoundException($"Category with ID {id} not found.");

        var attributes = await _repository.GetAttributesByCategoryIdAsync(id);
        var result = new List<AttributeCategoryDto>();

        foreach (var attr in attributes)
        {
            var attrDto = new AttributeCategoryDto
            {
                IdAttributCategorie = attr.IdAttributCategorie,
                Nom = attr.Nom,
                TypeDonnee = attr.TypeDonnee.ToString(),
                Obligatoire = attr.Obligatoire,
                Filtrable = attr.Filtrable,
                OrdreAffichage = attr.OrdreAffichage,
                Options = new List<OptionAttributeCategoryDto>()
            };

            if (attr.TypeDonnee == TypeDonneeAttribut.LISTE)
            {
                var options = await _repository.GetOptionsByAttributeIdAsync(attr.IdAttributCategorie);
                attrDto.Options = options.Select(o => new OptionAttributeCategoryDto
                {
                    IdOptionAttributCategorie = o.IdOptionAttributCategorie,
                    Valeur = o.Valeur,
                    OrdreAffichage = o.OrdreAffichage
                }).ToList();
            }

            result.Add(attrDto);
        }

        return result;
    }

    public async Task<CategorySchemaDto> GetCategorySchemaAsync(int id)
    {
        var category = await _repository.GetByIdAsync(id);
        if (category == null) throw new NotFoundException($"Category with ID {id} not found.");

        var attributes = await GetCategoryAttributesAsync(id);

        return new CategorySchemaDto
        {
            IdCategorie = category.IdCategorie,
            Nom = category.Nom,
            Description = category.Description,
            IconKey = category.IconKey,
            Attributs = attributes.ToList()
        };
    }
}
