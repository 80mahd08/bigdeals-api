using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using api.Common;
using api.Dtos.Contacts;
using api.Interfaces.Contacts;
using api.Interfaces.Users;
using api.Helpers.Security;

namespace api.Controllers;

[ApiController]
[Route("api/contacts-annonceur")]
public class ContactsAnnonceurController : ControllerBase
{
    private readonly IContactService _contactService;
    private readonly ICurrentUserService _currentUserService;

    public ContactsAnnonceurController(IContactService contactService, ICurrentUserService currentUserService)
    {
        _contactService = contactService;
        _currentUserService = currentUserService;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> TrackContact([FromBody] ContactCreateRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // _currentUserService.Id is safe for visitors (returns null)
        var userId = _currentUserService.Id; 
        
        var contactId = await _contactService.TrackContactAsync(request, userId);
        return Ok(ApiResponse<object>.Ok(new { IdContactAnnonceur = contactId }, "Contact tracked successfully."));
    }
}
