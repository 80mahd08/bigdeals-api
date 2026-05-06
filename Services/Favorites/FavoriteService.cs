using System.Threading.Tasks;
using api.Common;
using api.Dtos.Favorites;
using api.Exceptions;
using api.Interfaces.Annonces;
using api.Interfaces.Favorites;
using api.Models.Enums;
using api.Services.Storage;

namespace api.Services.Favorites;

public class FavoriteService : IFavoriteService
{
    private readonly IFavoriteRepository _favoriteRepository;
    private readonly IAnnonceRepository _annonceRepository;
    private readonly ILocalFileStorageService _storageService;

    public FavoriteService(IFavoriteRepository favoriteRepository, IAnnonceRepository annonceRepository, ILocalFileStorageService storageService)
    {
        _favoriteRepository = favoriteRepository;
        _annonceRepository = annonceRepository;
        _storageService = storageService;
    }

    public async Task<bool> AddFavoriteAsync(long userId, long annonceId)
    {
        var annonce = await _annonceRepository.GetByIdAsync(annonceId);
        if (annonce == null)
            throw new NotFoundException($"Annonce with ID {annonceId} not found.");

        if (annonce.Statut != StatutAnnonce.PUBLIEE || !annonce.EstActive)
            throw new BadRequestException("You can only favorite active and published announcements.");

        await _favoriteRepository.AddAsync(userId, annonceId);
        // We don't throw ConflictException here to avoid breaking the frontend if it's out of sync
        return true;
    }

    public async Task<bool> RemoveFavoriteAsync(long userId, long annonceId)
    {
        // Must be clean and predictable. Return true if removed, false if it wasn't there.
        // We'll let the controller return 404 or 204.
        var removed = await _favoriteRepository.RemoveAsync(userId, annonceId);
        if (!removed)
            throw new NotFoundException("Favorite not found.");
            
        return true;
    }

    public async Task<PagedResponse<FavoriteDto>> GetUserFavoritesAsync(long userId, int pageNumber, int pageSize)
    {
        var response = await _favoriteRepository.GetPagedByUserIdAsync(userId, pageNumber, pageSize);
        
        foreach (var fav in response.Items)
        {
            fav.MainImageUrl = _storageService.GetFullUrl(fav.MainImageUrl);
            fav.AnnonceurPhotoUrl = _storageService.GetFullUrl(fav.AnnonceurPhotoUrl);
        }
        
        return response;
    }

    public async Task<IReadOnlyList<long>> GetUserFavoriteIdsAsync(long userId)
    {
        return await _favoriteRepository.GetIdsByUserIdAsync(userId);
    }
}
