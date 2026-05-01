using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using api.Common;
using api.Dtos.Annonces;
using api.Interfaces.Annonces;
using api.Helpers.Security;

namespace api.Controllers;

[Route("api/users/me/annonces")]
[ApiController]
[Authorize(Roles = "ANNONCEUR")]
public class UserAnnoncesController : ControllerBase
{
    private readonly IAnnonceService _service;
    private readonly ICurrentUserService _currentUserService;

    public UserAnnoncesController(IAnnonceService service, ICurrentUserService currentUserService)
    {
        _service = service;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResponse<AnnonceDto>>>> GetMyAnnonces([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 12)
    {
        var currentUserId = _currentUserService.GetUserId();
        var result = await _service.GetUserAnnoncesAsync(currentUserId, pageNumber, pageSize);
        return Ok(ApiResponse<PagedResponse<AnnonceDto>>.Ok(result));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<AnnonceDetailsDto>>> GetMyAnnonceById(long id)
    {
        var currentUserId = _currentUserService.GetUserId();
        var result = await _service.GetAnnonceByIdAsync(id, currentUserId);
        
        if (result.IdUtilisateur != currentUserId)
            return Forbid();

        return Ok(ApiResponse<AnnonceDetailsDto>.Ok(result));
    }
}
