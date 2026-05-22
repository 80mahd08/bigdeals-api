using System.Collections.Generic;
using System.Threading.Tasks;
using api.Dtos.Annonces;
using api.Common;
using api.Models.Enums;

namespace api.Interfaces.Annonces;

public interface IAnnonceService
{
    Task<long> CreateAnnonceAsync(CreateAnnonceFormDto dto, long currentUserId);
    Task<bool> UpdateAnnonceAsync(long id, UpdateAnnonceFormDto dto, long currentUserId);
    Task<bool> DeleteAnnonceAsync(long id, long currentUserId);
    Task<AnnonceDetailsDto> GetAnnonceByIdAsync(long id, long? currentUserId = null);
    Task<PagedResponse<AnnonceDto>> GetPublicAnnoncesAsync(int pageNumber, int pageSize);
    Task<PagedResponse<AnnonceDto>> SearchAnnoncesAsync(AnnonceSearchRequestDto request);
    Task<PagedResponse<AnnonceDto>> GetUserAnnoncesAsync(long userId, int pageNumber, int pageSize, string? keyword = null, StatutAnnonce? statut = null, string? sortBy = null, string? sortDirection = null);
    Task<PagedResponse<AnnonceDto>> GetAdminAnnoncesAsync(int pageNumber, int pageSize, string? search = null, int? idCategorie = null, StatutAnnonce? statut = null, string? ville = null, string? sortBy = null, string? sortDirection = null);
    Task<IReadOnlyList<string>> GetDistinctVillesAsync();
    Task<bool> SuspendAnnonceAsync(long id);
    Task<bool> RestoreAnnonceAsync(long id, bool isAdminCall = false);
    Task<bool> DeleteAdminAnnonceAsync(long id);
}
