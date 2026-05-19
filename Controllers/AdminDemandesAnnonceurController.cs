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
    public async Task<ActionResult<ApiResponse<PagedResponse<DemandeAnnonceurDto>>>> GetAllRequests(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? statut = null,
        [FromQuery] string? search = null,
        [FromQuery] string? sortByDateDemande = null,
        [FromQuery] string? sortByDateTraitement = null)
    {
        // Clamp pageSize to max 50
        if (pageSize > 50) pageSize = 50;
        if (pageSize < 1) pageSize = 10;
        if (pageNumber < 1) pageNumber = 1;

        var (items, totalCount) = await _service.GetAllRequestsPagedAsync(pageNumber, pageSize, statut, search, sortByDateDemande, sortByDateTraitement);
        
        var response = new PagedResponse<DemandeAnnonceurDto>(items, totalCount, pageNumber, pageSize);
        return Ok(ApiResponse<PagedResponse<DemandeAnnonceurDto>>.Ok(response));
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
        return Ok(ApiResponse<object>.Ok(null, "Advertiser request document accepted. Payment is now required."));
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
