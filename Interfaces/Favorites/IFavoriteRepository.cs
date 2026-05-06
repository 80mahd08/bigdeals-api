using System.Threading.Tasks;
using api.Common;
using api.Dtos.Favorites;

namespace api.Interfaces.Favorites;

public interface IFavoriteRepository
{
    Task<bool> AddAsync(long userId, long annonceId);
    Task<bool> RemoveAsync(long userId, long annonceId);
    Task<PagedResponse<FavoriteDto>> GetPagedByUserIdAsync(long userId, int pageNumber, int pageSize);
    Task<bool> IsFavoritedAsync(long userId, long annonceId);
    Task<IReadOnlyList<long>> GetIdsByUserIdAsync(long userId);
}
