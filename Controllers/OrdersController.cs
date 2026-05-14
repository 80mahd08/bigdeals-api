using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using api.Interfaces.Orders;
using api.Common;

namespace api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrdersService _ordersService;

    public OrdersController(IOrdersService ordersService)
    {
        _ordersService = ordersService;
    }

    // ─── Buyer: get my orders ────────────────────────────────────
    [HttpGet]
    public async Task<IActionResult> GetMyOrders()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var userId))
            return Unauthorized();

        var result = await _ordersService.GetUserOrdersAsync(userId);
        return Ok(result);
    }

    // ─── Announcer: get my sales ─────────────────────────────────
    [HttpGet("announcer")]
    [Authorize(Roles = "ANNONCEUR")]
    public async Task<IActionResult> GetAnnouncerOrders()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var userId))
            return Unauthorized();

        var result = await _ordersService.GetAnnouncerOrdersAsync(userId);
        return Ok(result);
    }

    // ─── Admin: get all orders ───────────────────────────────────
    [HttpGet("admin/all")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> GetAllOrders()
    {
        var result = await _ordersService.GetAllOrdersAsync();
        return Ok(result);
    }

    // ─── Checkout ────────────────────────────────────────────────
    [HttpPost]
    public async Task<IActionResult> Checkout([FromBody] CreateOrderRequest request)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var userId))
            return Unauthorized();

        var result = await _ordersService.CheckoutAsync(userId, request);
        return Ok(result);
    }

    // ─── Buyer: cancel order ─────────────────────────────────────
    [HttpPatch("client/{idCommande}/cancel")]
    public async Task<IActionResult> CancelOrder(long idCommande)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var userId))
            return Unauthorized();

        var request = new UpdateDeliveryStatusRequest { StatutLivraison = 7 }; // 7 = ANNULEE
        var result = await _ordersService.UpdateDeliveryStatusAsync(idCommande, userId, request, isAdmin: false);
        return Ok(result);
    }

    // ─── Announcer: update delivery status ───────────────────────
    [HttpPatch("announcer/{idCommande}/delivery-status")]
    [Authorize(Roles = "ANNONCEUR")]
    public async Task<IActionResult> UpdateAnnouncerDeliveryStatus(long idCommande, [FromBody] UpdateDeliveryStatusRequest request)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var userId))
            return Unauthorized();

        var result = await _ordersService.UpdateDeliveryStatusAsync(idCommande, userId, request, isAdmin: false);
        return Ok(result);
    }

    // ─── Admin: update delivery status ───────────────────────────
    [HttpPatch("admin/{idCommande}/delivery-status")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> UpdateAdminDeliveryStatus(long idCommande, [FromBody] UpdateDeliveryStatusRequest request)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var userId))
            return Unauthorized();

        var result = await _ordersService.UpdateDeliveryStatusAsync(idCommande, userId, request, isAdmin: true);
        return Ok(result);
    }
}
