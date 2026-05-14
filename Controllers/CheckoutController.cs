using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using api.Interfaces.Checkout;

namespace api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "1,2")] // CLIENT and ANNONCEUR
public class CheckoutController : ControllerBase
{
    private readonly ICheckoutService _checkoutService;

    public CheckoutController(ICheckoutService checkoutService)
    {
        _checkoutService = checkoutService;
    }

    [HttpPost("create/{idAnnonce}")]
    public async Task<IActionResult> CreateCheckout(long idAnnonce)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var userId))
            return Unauthorized();

        var result = await _checkoutService.CreateCheckoutAsync(idAnnonce, userId);
        return Ok(result);
    }

    [HttpGet("{idCommande}")]
    public async Task<IActionResult> GetCheckoutDetails(long idCommande)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var userId))
            return Unauthorized();

        var result = await _checkoutService.GetCheckoutDetailsAsync(idCommande, userId);
        return Ok(result);
    }
}
