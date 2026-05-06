using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using api.Common;
using api.Dtos.Annonces;
using api.Interfaces.Annonces;
using api.Helpers.Security;

namespace api.Controllers;

[Route("api/annonces")]
[ApiController]
public class AvisController : ControllerBase
{
    private readonly IAvisService _avisService;
    private readonly ICurrentUserService _currentUserService;

    public AvisController(IAvisService avisService, ICurrentUserService currentUserService)
    {
        _avisService = avisService;
        _currentUserService = currentUserService;
    }

    [HttpGet("{idAnnonce}/avis")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<object>>> GetByAnnonceId(long idAnnonce, [FromQuery] int page = 1, [FromQuery] int pageSize = 5)
    {
        // If we want to force pagination, we use the paged method
        var result = await _avisService.GetPagedByAnnonceIdAsync(idAnnonce, page, pageSize);
        return Ok(ApiResponse<object>.Ok(result));
    }

    [HttpPost("{idAnnonce}/avis")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<AvisDto>>> Create(long idAnnonce, [FromBody] CreateAvisDto dto)
    {
        var currentUserId = _currentUserService.GetUserId();
        var result = await _avisService.CreateAsync(idAnnonce, currentUserId, dto);
        return Ok(ApiResponse<AvisDto>.Ok(result, "Votre avis a été publié avec succès."));
    }

    [HttpPut("{idAnnonce}/avis/me")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<bool>>> Update(long idAnnonce, [FromBody] CreateAvisDto dto)
    {
        var currentUserId = _currentUserService.GetUserId();
        var result = await _avisService.UpdateAsync(idAnnonce, currentUserId, dto);
        return Ok(ApiResponse<bool>.Ok(result, "Votre avis a été mis à jour avec succès."));
    }

    [HttpDelete("{idAnnonce}/avis/me")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(long idAnnonce)
    {
        var currentUserId = _currentUserService.GetUserId();
        var result = await _avisService.DeleteAsync(idAnnonce, currentUserId);
        return Ok(ApiResponse<bool>.Ok(result, "Votre avis a été supprimé avec succès."));
    }
}
