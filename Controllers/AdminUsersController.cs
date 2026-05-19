using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using api.Common;
using api.Dtos.Admin;
using api.Interfaces.Admin;

namespace api.Controllers;

[Route("api/admin/users")]
[ApiController]
[Authorize(Roles = "ADMIN")]
public class AdminUsersController : ControllerBase
{
    private readonly IAdminUserService _adminUserService;

    public AdminUsersController(IAdminUserService adminUserService)
    {
        _adminUserService = adminUserService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResponse<AdminUserListItemDto>>>> GetUsers(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] int? statutCompte = null,
        [FromQuery] int? role = null,
        [FromQuery] string? ville = null,
        [FromQuery] string? sortByDateInscription = null,
        [FromQuery] string? sortByNbAnnonces = null)
    {
        var result = await _adminUserService.GetUsersAsync(pageNumber, pageSize, search, statutCompte, role, ville, sortByDateInscription, sortByNbAnnonces);
        return Ok(ApiResponse<PagedResponse<AdminUserListItemDto>>.Ok(result));
    }

    [HttpPut("{id}/block")]
    public async Task<ActionResult<ApiResponse<bool>>> BlockUser(long id)
    {
        await _adminUserService.BlockUserAsync(id);
        return Ok(ApiResponse<bool>.Ok(true, "Utilisateur bloqué avec succès."));
    }

    [HttpPut("{id}/unblock")]
    public async Task<ActionResult<ApiResponse<bool>>> UnblockUser(long id)
    {
        await _adminUserService.UnblockUserAsync(id);
        return Ok(ApiResponse<bool>.Ok(true, "Utilisateur débloqué avec succès."));
    }
}
