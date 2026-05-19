using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using api.Common;
using api.Dtos.Signalements;
using api.Interfaces.Signalements;

namespace api.Controllers;

[ApiController]
[Route("api/interactions")]
[Authorize(Roles = "CLIENT,ANNONCEUR")]
public class InteractionsController : ControllerBase
{
    private readonly ISignalementService _signalementService;

    public InteractionsController(ISignalementService signalementService)
    {
        _signalementService = signalementService;
    }

    [HttpPost("report")]
    public async Task<IActionResult> CreateSignalement([FromBody] CreateSignalementDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var userId))
            return Unauthorized();

        var result = await _signalementService.CreateAsync(dto, userId);
        return Ok(ApiResponse<SignalementDto>.Ok(result!, "Signalement envoyé avec succès."));
    }
}
