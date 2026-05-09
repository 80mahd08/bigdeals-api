using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using api.Common;
using api.Interfaces.Favorites;
using api.Interfaces.Users;
using api.Helpers.Security;
using System.Collections.Generic;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "CLIENT,ANNONCEUR")]
public class FavoritesController : ControllerBase
{
    private readonly IFavoriteService _favoriteService;
    private readonly ICurrentUserService _currentUserService;

    public FavoritesController(IFavoriteService favoriteService, ICurrentUserService currentUserService)
    {
        _favoriteService = favoriteService;
        _currentUserService = currentUserService;
    }

    [HttpPost("{idAnnonce}")]
    public async Task<IActionResult> AddFavorite(long idAnnonce)
    {
        var userId = _currentUserService.GetUserId();
        await _favoriteService.AddFavoriteAsync(userId, idAnnonce);
        return Ok(ApiResponse<bool>.Ok(true, "Annonce ajoutée aux favoris."));
    }

    [HttpDelete("{idAnnonce}")]
    public async Task<IActionResult> RemoveFavorite(long idAnnonce)
    {
        var userId = _currentUserService.GetUserId();
        await _favoriteService.RemoveFavoriteAsync(userId, idAnnonce);
        return Ok(ApiResponse<bool>.Ok(true, "Annonce retirée des favoris."));
    }

    [HttpGet("ids")]
    public async Task<IActionResult> GetFavoriteIds()
    {
        var userId = _currentUserService.GetUserId();
        var ids = await _favoriteService.GetUserFavoriteIdsAsync(userId);
        return Ok(ApiResponse<IReadOnlyList<long>>.Ok(ids));
    }
}
