using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using api.Common;
using api.Dtos.AnnonceurPayments;
using api.Interfaces.AnnonceurPayments;
using api.Helpers.Security;

namespace api.Controllers;

[ApiController]
[Route("api/annonceur-payments")]
public class AnnonceurPaymentsController : ControllerBase
{
    private readonly IAnnonceurPaymentService _paymentService;
    private readonly ICurrentUserService _currentUserService;

    public AnnonceurPaymentsController(IAnnonceurPaymentService paymentService, ICurrentUserService currentUserService)
    {
        _paymentService = paymentService;
        _currentUserService = currentUserService;
    }

    [HttpPost("initiate/{demandeAnnonceurId}")]
    [Authorize(Roles = "CLIENT,ANNONCEUR")]
    public async Task<ActionResult<ApiResponse<InitiateAnnonceurPaymentResponseDto>>> InitiateMockPayment(long demandeAnnonceurId)
    {
        var userId = _currentUserService.GetUserId();
        var result = await _paymentService.InitiateMockPaymentAsync(demandeAnnonceurId, userId);
        return Ok(ApiResponse<InitiateAnnonceurPaymentResponseDto>.Ok(result, "Annonceur payment initialized."));
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<AnnonceurPaymentDto>>> GetById(long id)
    {
        var userId = _currentUserService.GetUserId();
        var isAdmin = User.IsInRole("ADMIN");
        var result = await _paymentService.GetByIdAsync(id, userId, isAdmin);
        return Ok(ApiResponse<AnnonceurPaymentDto>.Ok(result));
    }

    [HttpGet("/api/users/me/annonceur-payments")]
    [Authorize(Roles = "CLIENT,ANNONCEUR")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<AnnonceurPaymentDto>>>> GetMyPayments()
    {
        var userId = _currentUserService.GetUserId();
        var result = await _paymentService.GetMyPaymentsAsync(userId);
        return Ok(ApiResponse<IReadOnlyList<AnnonceurPaymentDto>>.Ok(result));
    }

    [HttpGet("/api/admin/annonceur-payments")]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<ApiResponse<PagedResponse<AnnonceurPaymentDto>>>> GetAdminPaged(
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 12,
        [FromQuery] string? search = null)
    {
        var result = await _paymentService.GetAdminPagedAsync(pageNumber, pageSize, search);
        return Ok(ApiResponse<PagedResponse<AnnonceurPaymentDto>>.Ok(result));
    }

    [HttpPost("/api/admin/annonceur-payments/{id}/mock-mark-paid")]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<ApiResponse<AnnonceurPaymentDto>>> MarkMockPaymentAsPaid(long id)
    {
        var adminId = _currentUserService.GetUserId();
        var result = await _paymentService.MarkMockPaymentAsPaidAsync(id, adminId);
        return Ok(ApiResponse<AnnonceurPaymentDto>.Ok(result, "Mock payment marked as paid. User is now an advertiser."));
    }
}
