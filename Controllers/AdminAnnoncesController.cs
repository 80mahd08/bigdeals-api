using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using api.Common;
using api.Dtos.Annonces;
using api.Interfaces.Annonces;
using api.Models.Enums;

namespace api.Controllers;

[Route("api/admin/annonces")]
[ApiController]
[Authorize(Roles = "ADMIN")]
public class AdminAnnoncesController : ControllerBase
{
    private readonly IAnnonceService _service;

    public AdminAnnoncesController(IAnnonceService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResponse<AnnonceDto>>>> GetAll(
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 12, 
        [FromQuery] string? search = null,
        [FromQuery] int? idCategorie = null,
        [FromQuery] int? statut = null,
        [FromQuery] string? ville = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortDirection = null)
    {
        StatutAnnonce? statutEnum = statut.HasValue ? (StatutAnnonce)statut.Value : null;
        var result = await _service.GetAdminAnnoncesAsync(pageNumber, pageSize, search, idCategorie, statutEnum, ville, sortBy, sortDirection);
        return Ok(ApiResponse<PagedResponse<AnnonceDto>>.Ok(result));
    }

    [HttpGet("villes")]
    public async Task<ActionResult<ApiResponse<IEnumerable<string>>>> GetVilles()
    {
        var villes = await _service.GetDistinctVillesAsync();
        return Ok(ApiResponse<IEnumerable<string>>.Ok(villes));
    }

    [HttpPut("{id}/suspend")]
    public async Task<ActionResult<ApiResponse<bool>>> Suspend(long id)
    {
        await _service.SuspendAnnonceAsync(id);
        return Ok(ApiResponse<bool>.Ok(true, "Annonce suspended successfully."));
    }

    [HttpPut("{id}/restore")]
    public async Task<ActionResult<ApiResponse<bool>>> Restore(long id)
    {
        await _service.RestoreAnnonceAsync(id, true);
        return Ok(ApiResponse<bool>.Ok(true, "Annonce restored successfully."));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(long id)
    {
        await _service.DeleteAdminAnnonceAsync(id);
        return Ok(ApiResponse<bool>.Ok(true, "Annonce deleted successfully."));
    }
}
