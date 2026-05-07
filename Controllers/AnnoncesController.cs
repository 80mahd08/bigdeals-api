using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using api.Common;
using api.Dtos.Annonces;
using api.Interfaces.Annonces;
using api.Helpers.Security;
using api.Models.Enums;
using api.Exceptions;
using api.Interfaces.Categories;
using api.Models;
using System.Collections.Generic;
using System.Linq;

namespace api.Controllers;

[Route("api/annonces")]
[ApiController]
public class AnnoncesController : ControllerBase
{
    private readonly IAnnonceService _service;
    private readonly ICurrentUserService _currentUserService;
    private readonly ICategoryRepository _categoryRepository;

    public AnnoncesController(IAnnonceService service, ICurrentUserService currentUserService, ICategoryRepository categoryRepository)
    {
        _service = service;
        _currentUserService = currentUserService;
        _categoryRepository = categoryRepository;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<PagedResponse<AnnonceDto>>>> GetPublicList([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 12)
    {
        if (pageSize > 50) pageSize = 50;
        var result = await _service.GetPublicAnnoncesAsync(pageNumber, pageSize);
        return Ok(ApiResponse<PagedResponse<AnnonceDto>>.Ok(result));
    }

    [HttpPost("search")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<PagedResponse<AnnonceDto>>>> Search([FromBody] AnnonceSearchRequestDto request)
    {
        var result = await _service.SearchAnnoncesAsync(request);
        return Ok(ApiResponse<PagedResponse<AnnonceDto>>.Ok(result));
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<AnnonceDetailsDto>>> GetById(long id)
    {
        var result = await _service.GetAnnonceByIdAsync(id);
        
        // Strict Visibility: Public can ONLY see PUBLIEE and EstActive = 1
        if (result.Statut != StatutAnnonce.PUBLIEE.ToString() || !result.EstActive)
        {
            throw new NotFoundException("Annonce not found.");
        }

        return Ok(ApiResponse<AnnonceDetailsDto>.Ok(result));
    }

    [HttpGet("admin/{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<ApiResponse<AnnonceDetailsDto>>> GetByIdAdmin(long id)
    {
        var result = await _service.GetAnnonceByIdAsync(id);
        return Ok(ApiResponse<AnnonceDetailsDto>.Ok(result));
    }

    [HttpPost]
    [Authorize(Roles = "ANNONCEUR")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ApiResponse<long>>> Create([FromForm] CreateAnnonceFormDto dto)
    {
        var currentUserId = _currentUserService.GetUserId();
        var id = await _service.CreateAnnonceAsync(dto, currentUserId);
        return Ok(ApiResponse<long>.Ok(id, "Annonce created successfully."));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "ANNONCEUR")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ApiResponse<bool>>> Update(long id, [FromForm] UpdateAnnonceFormDto dto)
    {
        var currentUserId = _currentUserService.GetUserId();
        await _service.UpdateAnnonceAsync(id, dto, currentUserId);
        return Ok(ApiResponse<bool>.Ok(true, "Annonce updated successfully."));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "ANNONCEUR")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(long id)
    {
        var currentUserId = _currentUserService.GetUserId();
        await _service.DeleteAnnonceAsync(id, currentUserId);
        return Ok(ApiResponse<bool>.Ok(true, "Annonce deleted successfully."));
    }

    [HttpPatch("{id}/suspend")]
    [Authorize(Roles = "ANNONCEUR")]
    public async Task<ActionResult<ApiResponse<bool>>> Suspend(long id)
    {
        var currentUserId = _currentUserService.GetUserId();
        var existing = await _service.GetAnnonceByIdAsync(id, currentUserId);
        if (existing.IdUtilisateur != currentUserId) throw new ForbiddenException("You don't own this annonce.");

        await _service.SuspendAnnonceAsync(id);
        return Ok(ApiResponse<bool>.Ok(true, "Annonce suspendue avec succès."));
    }

    [HttpPatch("{id}/resume")]
    [Authorize(Roles = "ANNONCEUR")]
    public async Task<ActionResult<ApiResponse<bool>>> Resume(long id)
    {
        var currentUserId = _currentUserService.GetUserId();
        var existing = await _service.GetAnnonceByIdAsync(id, currentUserId);
        if (existing.IdUtilisateur != currentUserId) throw new ForbiddenException("You don't own this annonce.");

        await _service.RestoreAnnonceAsync(id);
        return Ok(ApiResponse<bool>.Ok(true, "Annonce publiée avec succès."));
    }
}
