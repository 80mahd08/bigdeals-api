using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using api.Common;
using api.Interfaces.Contacts;
using api.Interfaces.Annonces;
using api.Dtos.Annonces;
using api.Helpers.Security;

namespace api.Controllers;

[ApiController]
[Route("api/annonceurs")]
public class AnnonceursController : ControllerBase
{
    private readonly IContactService _contactService;
    private readonly IAvisService _avisService;
    private readonly ICurrentUserService _currentUserService;

    public AnnonceursController(IContactService contactService, IAvisService avisService, ICurrentUserService currentUserService)
    {
        _contactService = contactService;
        _avisService = avisService;
        _currentUserService = currentUserService;
    }

    [HttpGet("me/contacts")]
    [Authorize(Policy = "AnnonceurOnly")]
    public async Task<IActionResult> GetMyIncomingContacts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var advertiserId = _currentUserService.GetUserId();
        var contacts = await _contactService.GetMyIncomingContactsAsync(advertiserId, pageNumber, pageSize);
        return Ok(ApiResponse<object>.Ok(contacts));
    }

    [HttpGet("me/reviews")]
    [Authorize(Policy = "AnnonceurOnly")]
    public async Task<IActionResult> GetMyReviews()
    {
        var advertiserId = _currentUserService.GetUserId();
        var reviews = await _avisService.GetByAnnonceurIdAsync(advertiserId);
        return Ok(ApiResponse<IEnumerable<AvisDto>>.Ok(reviews));
    }
}
