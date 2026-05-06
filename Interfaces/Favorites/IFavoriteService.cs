using System.Threading.Tasks;
using api.Common;
using api.Dtos.Favorites;

namespace api.Interfaces.Favorites;

public interface IFavoriteService
{
    Task<bool> AddFavoriteAsync(long userId, long annonceId);
    Task<bool> RemoveFavoriteAsync(long userId, long annonceId);
    Task<PagedResponse<FavoriteDto>> GetUserFavoritesAsync(long userId, int pageNumber, int pageSize);
    Task<IReadOnlyList<long>> GetUserFavoriteIdsAsync(long userId);
}
