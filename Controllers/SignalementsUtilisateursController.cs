using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using api.Common;
using api.Helpers.Security;
using api.Dtos.SignalementsUtilisateurs;
using api.Extensions;
using api.Interfaces.Signalements;

namespace api.Controllers;

[ApiController]
[Route("api/signalements-utilisateurs")]
public class SignalementsUtilisateursController : ControllerBase
{
    private readonly ISignalementUtilisateurService _signalementService;
    private readonly ICurrentUserService _currentUserService;

    public SignalementsUtilisateursController(ISignalementUtilisateurService signalementService, ICurrentUserService currentUserService)
    {
        _signalementService = signalementService;
        _currentUserService = currentUserService;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateSignalement([FromBody] CreateSignalementUtilisateurDto dto)
    {
        var currentUserId = _currentUserService.GetUserId();
        var result = await _signalementService.CreateAsync(dto, currentUserId);
        return Created("", ApiResponse<SignalementUtilisateurDto>.Ok(result, "Utilisateur signalé avec succès."));
    }

    [HttpGet("admin")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> GetSignalementsAdmin(
        [FromQuery] int page = 1, 
        [FromQuery] int limit = 10, 
        [FromQuery] int? statut = null, 
        [FromQuery] string? search = null,
        [FromQuery] string? sortByDate = null,
        [FromQuery] int? raison = null)
    {
        var pagedResult = await _signalementService.GetPagedAdminAsync(page, limit, statut, search, sortByDate, raison);
        return Ok(ApiResponse<PagedResponse<SignalementUtilisateurDto>>.Ok(pagedResult, "Signalements récupérés avec succès."));
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> UpdateStatus(long id, [FromBody] UpdateSignalementUtilisateurStatusDto dto)
    {
        var adminId = _currentUserService.GetUserId();
        await _signalementService.UpdateStatusAsync(id, dto, adminId);
        return Ok(ApiResponse<object>.Ok(null, "Statut du signalement mis à jour."));
    }
}
