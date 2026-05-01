using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using api.Common;
using api.Dtos.DemandesAnnonceur;
using api.Helpers.Security;
using api.Interfaces.DemandesAnnonceur;

namespace api.Controllers;

[Route("api/admin/demandes-annonceur")]
[ApiController]
[Authorize(Policy = "AdminOnly")]
public class AdminDemandesAnnonceurController : ControllerBase
{
    private readonly IDemandeAnnonceurService _service;
    private readonly ICurrentUserService _currentUserService;

    public AdminDemandesAnnonceurController(IDemandeAnnonceurService service, ICurrentUserService currentUserService)
    {
        _service = service;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<DemandeAnnonceurDto>>>> GetAllRequests()
    {
        var result = await _service.GetAllRequestsAsync();
        return Ok(ApiResponse<IReadOnlyList<DemandeAnnonceurDto>>.Ok(result));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<DemandeAnnonceurDto>>> GetRequestById(long id)
    {
        var result = await _service.GetRequestByIdAsync(id);
        return Ok(ApiResponse<DemandeAnnonceurDto>.Ok(result));
    }

    [HttpPut("{id}/approve")]
    public async Task<ActionResult<ApiResponse<object>>> ApproveRequest(long id)
    {
        var adminId = _currentUserService.GetUserId();
        await _service.ApproveRequestAsync(id, adminId);
        return Ok(ApiResponse<object>.Ok(null, "Request approved successfully. User is now an advertiser."));
    }

    [HttpPut("{id}/reject")]
    public async Task<ActionResult<ApiResponse<object>>> RejectRequest(long id, [FromBody] RejectDemandeAnnonceurDto request)
    {
        var adminId = _currentUserService.GetUserId();
        await _service.RejectRequestAsync(id, adminId, request);
        return Ok(ApiResponse<object>.Ok(null, "Request rejected successfully."));
    }

    [HttpGet("{id}/document")]
    public async Task<IActionResult> GetDocument(long id)
    {
        var (content, contentType, fileName) = await _service.GetDocumentAsync(id);
        return File(content, contentType, fileName);
    }
}
