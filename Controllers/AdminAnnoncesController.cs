using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using api.Common;
using api.Dtos.Annonces;
using api.Interfaces.Annonces;

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
    public async Task<ActionResult<ApiResponse<PagedResponse<AnnonceDto>>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 12)
    {
        var result = await _service.GetAdminAnnoncesAsync(pageNumber, pageSize);
        return Ok(ApiResponse<PagedResponse<AnnonceDto>>.Ok(result));
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
        await _service.RestoreAnnonceAsync(id);
        return Ok(ApiResponse<bool>.Ok(true, "Annonce restored successfully."));
    }
}
