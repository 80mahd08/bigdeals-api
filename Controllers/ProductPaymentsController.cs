using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using api.Dtos.ProductPayments;
using api.Interfaces.ProductPayments;

namespace api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "CLIENT,ANNONCEUR")]
public class ProductPaymentsController : ControllerBase
{
    private readonly IProductPaymentService _paymentService;

    public ProductPaymentsController(IProductPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost("mock-process")]
    public async Task<IActionResult> ProcessMockPayment([FromBody] MockProductPaymentRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var userId))
            return Unauthorized();

        var result = await _paymentService.ProcessMockPaymentAsync(request, userId);
        return Ok(result);
    }
}
