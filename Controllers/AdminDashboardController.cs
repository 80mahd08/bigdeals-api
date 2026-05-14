using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using api.Common;
using api.Dtos.Admin;
using api.Interfaces.Admin;

namespace api.Controllers;

[Route("api/admin/dashboard")]
[ApiController]
[Authorize(Roles = "ADMIN")]
public class AdminDashboardController : ControllerBase
{
    private readonly IAdminDashboardService _service;

    public AdminDashboardController(IAdminDashboardService service)
    {
        _service = service;
    }

    [HttpGet("stats")]
    public async Task<ActionResult<ApiResponse<AdminDashboardStatsDto>>> GetStats()
    {
        var stats = await _service.GetDashboardStatsAsync();
        return Ok(ApiResponse<AdminDashboardStatsDto>.Ok(stats));
    }

    [HttpGet("growth")]
    public async Task<ActionResult<ApiResponse<AdminGrowthChartDto>>> GetGrowth([FromQuery] string metric = "users", [FromQuery] string period = "30d")
    {
        var chart = await _service.GetGrowthChartAsync(metric, period);
        return Ok(ApiResponse<AdminGrowthChartDto>.Ok(chart));
    }
}
