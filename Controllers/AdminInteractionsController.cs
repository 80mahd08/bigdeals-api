using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using api.Common;
using api.Dtos.Signalements;
using api.Interfaces.Contacts;
using api.Interfaces.Signalements;

namespace api.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Policy = "AdminOnly")]
public class AdminInteractionsController : ControllerBase
{
    private readonly IContactService _contactService;
    private readonly ISignalementService _signalementService;

    public AdminInteractionsController(IContactService contactService, ISignalementService signalementService)
    {
        _contactService = contactService;
        _signalementService = signalementService;
    }

    [HttpGet("contacts-annonceur")]
    public async Task<IActionResult> GetAllContacts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var contacts = await _contactService.GetAllContactsAdminAsync(pageNumber, pageSize);
        return Ok(ApiResponse<object>.Ok(contacts));
    }

    [HttpGet("signalements")]
    public async Task<IActionResult> GetSignalements(
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10, 
        [FromQuery] int? statut = null, 
        [FromQuery] string? search = null,
        [FromQuery] string? sortByDate = null,
        [FromQuery] int? type = null)
    {
        var result = await _signalementService.GetPagedAdminAsync(pageNumber, pageSize, statut, search, sortByDate, type);
        return Ok(ApiResponse<PagedResponse<SignalementDto>>.Ok(result));
    }

    [HttpPatch("signalements/{id}/status")]
    public async Task<IActionResult> UpdateSignalementStatus(long id, [FromBody] UpdateSignalementStatusDto dto)
    {
        var adminIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(adminIdString) || !long.TryParse(adminIdString, out var adminId))
            return Unauthorized();

        await _signalementService.UpdateStatusAsync(id, dto, adminId);
        return Ok(ApiResponse<bool>.Ok(true, "Statut du signalement mis à jour avec succès."));
    }
}
