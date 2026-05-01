using System.Collections.Generic;
using System.Threading.Tasks;
using api.Dtos.Annonces;
using api.Common;

namespace api.Interfaces.Annonces;

public interface IAnnonceService
{
    Task<long> CreateAnnonceAsync(CreateAnnonceFormDto dto, long currentUserId);
    Task<bool> UpdateAnnonceAsync(long id, UpdateAnnonceDto dto, long currentUserId);
    Task<bool> DeleteAnnonceAsync(long id, long currentUserId);
    Task<AnnonceDetailsDto> GetAnnonceByIdAsync(long id, long? currentUserId = null);
    Task<PagedResponse<AnnonceDto>> GetPublicAnnoncesAsync(int pageNumber, int pageSize);
    Task<PagedResponse<AnnonceDto>> SearchAnnoncesAsync(AnnonceSearchRequestDto request);
    Task<PagedResponse<AnnonceDto>> GetUserAnnoncesAsync(long userId, int pageNumber, int pageSize);
    Task<PagedResponse<AnnonceDto>> GetAdminAnnoncesAsync(int pageNumber, int pageSize);
    Task<bool> SuspendAnnonceAsync(long id);
    Task<bool> RestoreAnnonceAsync(long id);
}
