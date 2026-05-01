using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using api.Common;
using api.Dtos.DemandesAnnonceur;
using api.Helpers.Security;
using api.Interfaces.DemandesAnnonceur;

namespace api.Controllers;

[Route("api/demandes-annonceur")]
[ApiController]
[Authorize]
public class DemandesAnnonceurController : ControllerBase
{
    private readonly IDemandeAnnonceurService _service;
    private readonly ICurrentUserService _currentUserService;

    public DemandesAnnonceurController(IDemandeAnnonceurService service, ICurrentUserService currentUserService)
    {
        _service = service;
        _currentUserService = currentUserService;
    }

    [HttpPost]
    [Authorize(Policy = "ClientOnly")]
    public async Task<ActionResult<ApiResponse<DemandeAnnonceurDto>>> CreateRequest([FromForm] CreateDemandeAnnonceurDto request)
    {
        var idUtilisateur = _currentUserService.GetUserId();
        var result = await _service.CreateRequestAsync(idUtilisateur, request);
        return Ok(ApiResponse<DemandeAnnonceurDto>.Ok(result, "Request submitted successfully."));
    }

    [HttpGet("me")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<DemandeAnnonceurDto>>>> GetMyRequests()
    {
        var idUtilisateur = _currentUserService.GetUserId();
        var result = await _service.GetMyRequestsAsync(idUtilisateur);
        return Ok(ApiResponse<IReadOnlyList<DemandeAnnonceurDto>>.Ok(result));
    }

    [HttpGet("me/document")]
    public async Task<IActionResult> GetMyLatestDocument()
    {
        var idUtilisateur = _currentUserService.GetUserId();
        var requests = await _service.GetMyRequestsAsync(idUtilisateur);
        
        if (requests.Count == 0)
            return NotFound(ApiResponse<object>.Fail("No advertiser requests found."));

        var latestRequest = requests.OrderByDescending(r => r.DateDemande).First();
        var (content, contentType, fileName) = await _service.GetDocumentAsync(latestRequest.IdDemandeAnnonceur);
        return File(content, contentType, fileName);
    }

}
