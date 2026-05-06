using System.Collections.Generic;
using System.Threading.Tasks;
using api.Dtos.Annonces;
using api.Common;

namespace api.Interfaces.Annonces;

public interface IAvisService
{
    Task<IEnumerable<AvisDto>> GetByAnnonceIdAsync(long idAnnonce);
    Task<IEnumerable<AvisDto>> GetByAnnonceurIdAsync(long idAnnonceur);
    Task<PagedResponse<AvisDto>> GetPagedByAnnonceIdAsync(long idAnnonce, int page, int pageSize);
    Task<AvisDto> CreateAsync(long idAnnonce, long idUtilisateur, CreateAvisDto dto);
    Task<bool> UpdateAsync(long idAnnonce, long idUtilisateur, CreateAvisDto dto);
    Task<bool> DeleteAsync(long idAnnonce, long idUtilisateur);
}
