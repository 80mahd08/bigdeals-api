using System.Collections.Generic;
using System.Threading.Tasks;
using api.Models;

namespace api.Interfaces.Annonces;

public interface IAvisRepository
{
    Task<IEnumerable<Avis>> GetByAnnonceIdAsync(long idAnnonce);
    Task<(IEnumerable<Avis> Items, int TotalCount)> GetPagedByAnnonceIdAsync(long idAnnonce, int page, int pageSize);
    Task<IEnumerable<Avis>> GetByAnnonceurIdAsync(long idAnnonceur);
    Task<Avis?> GetByIdAsync(long idAvis);
    Task<Avis?> GetByUserAndAnnonceAsync(long idUtilisateur, long idAnnonce);
    Task<long> CreateAsync(Avis avis);
    Task<bool> UpdateAsync(Avis avis);
    Task<bool> DeleteAsync(long idAvis);
}
