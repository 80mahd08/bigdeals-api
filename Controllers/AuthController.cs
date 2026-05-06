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
        SetRefreshTokenCookie(response.RefreshToken);
        return Ok(ApiResponse<AuthResponseDto>.Ok(response, "User registered successfully."));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromForm] LoginRequestDto request)
    {
        var response = await _authService.LoginAsync(request);
        SetRefreshTokenCookie(response.RefreshToken);
        return Ok(ApiResponse<AuthResponseDto>.Ok(response, "Login successful."));
    }

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> RefreshToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken))
            return BadRequest(ApiResponse<AuthResponseDto>.Fail("Refresh token is required."));

        var response = await _authService.RefreshTokenAsync(refreshToken);
        SetRefreshTokenCookie(response.RefreshToken);
        return Ok(ApiResponse<AuthResponseDto>.Ok(response, "Token refreshed successfully."));
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<object>>> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
    {
        await _authService.ForgotPasswordAsync(request);
        return Ok(ApiResponse<object>.Ok(null, "Si cet email existe dans notre base, un lien de réinitialisation a été envoyé."));
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<object>>> ResetPassword([FromBody] ResetPasswordRequestDto request)
    {
        await _authService.ResetPasswordAsync(request);
        return Ok(ApiResponse<object>.Ok(null, "Mot de passe réinitialisé avec succès."));
    }

    private void SetRefreshTokenCookie(string refreshToken)
    {
        var cookieOptions = new Microsoft.AspNetCore.Http.CookieOptions
        {
            HttpOnly = true,
            Expires = System.DateTime.UtcNow.AddDays(7),
            Secure = true, // Set to true in production
            SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict
        };
        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }
}
