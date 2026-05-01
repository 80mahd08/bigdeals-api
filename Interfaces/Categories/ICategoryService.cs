using System.Collections.Generic;
using System.Threading.Tasks;
using api.Dtos.Categories;

namespace api.Interfaces.Categories;

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryDto>> GetAllCategoriesAsync();
    Task<CategoryDetailsDto> GetCategoryByIdAsync(int id);
    Task<IReadOnlyList<AttributeCategoryDto>> GetCategoryAttributesAsync(int id);
    Task<CategorySchemaDto> GetCategorySchemaAsync(int id);
}
