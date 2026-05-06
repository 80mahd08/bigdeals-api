using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using api.Common;
using api.Interfaces.Contacts;

namespace api.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Policy = "AdminOnly")]
public class AdminInteractionsController : ControllerBase
{
    private readonly IContactService _contactService;

    public AdminInteractionsController(IContactService contactService)
    {
        _contactService = contactService;
    }

    [HttpGet("contacts-annonceur")]
    public async Task<IActionResult> GetAllContacts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var contacts = await _contactService.GetAllContactsAdminAsync(pageNumber, pageSize);
        return Ok(ApiResponse<object>.Ok(contacts));
    }
}
