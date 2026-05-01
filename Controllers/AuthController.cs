using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using api.Common;
using api.Dtos.Auth;
using api.Interfaces.Auth;

namespace api.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register([FromForm] RegisterRequestDto request)
    {
        var response = await _authService.RegisterAsync(request);
        return Ok(ApiResponse<AuthResponseDto>.Ok(response, "User registered successfully."));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromForm] LoginRequestDto request)
    {
        var response = await _authService.LoginAsync(request);
        return Ok(ApiResponse<AuthResponseDto>.Ok(response, "Login successful."));
    }
}
