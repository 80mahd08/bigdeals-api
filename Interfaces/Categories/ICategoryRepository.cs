using System.Collections.Generic;
using System.Threading.Tasks;
using api.Models;

namespace api.Interfaces.Categories;

public interface ICategoryRepository
{
    Task<IReadOnlyList<Categorie>> GetAllAsync();
    Task<Categorie?> GetByIdAsync(int id);
    Task<IReadOnlyList<AttributCategorie>> GetAttributesByCategoryIdAsync(int idCategorie);
    Task<IReadOnlyList<OptionAttributCategorie>> GetOptionsByAttributeIdAsync(int idAttributCategorie);
}
