using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using api.Common;
using api.Interfaces.Favorites;
using api.Interfaces.Users;
using api.Helpers.Security;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Globally requires auth
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
        // Only CLIENT and ANNONCEUR should favorite. Admin shouldn't really use this.
        if (User.IsInRole("ADMIN"))
            return Forbid(); // 403

        var userId = _currentUserService.GetUserId();
        await _favoriteService.AddFavoriteAsync(userId, idAnnonce);
        return Ok(ApiResponse<object>.Ok(null, "Annonce added to favorites successfully."));
    }

    [HttpDelete("{idAnnonce}")]
    public async Task<IActionResult> RemoveFavorite(long idAnnonce)
    {
        if (User.IsInRole("ADMIN"))
            return Forbid(); // 403

        var userId = _currentUserService.GetUserId();
        await _favoriteService.RemoveFavoriteAsync(userId, idAnnonce);
        return Ok(ApiResponse<object>.Ok(null, "Annonce removed from favorites successfully."));
    }

    [HttpGet("ids")]
    public async Task<IActionResult> GetFavoriteIds()
    {
        var userId = _currentUserService.GetUserId();
        var ids = await _favoriteService.GetUserFavoriteIdsAsync(userId);
        return Ok(ApiResponse<IReadOnlyList<long>>.Ok(ids));
    }
}
