using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using api.Common;
using api.Dtos.Users;
using api.Helpers.Security;
using api.Interfaces.Users;

namespace api.Controllers;

[Route("api/users")]
[ApiController]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ICurrentUserService _currentUserService;

    public UsersController(IUserService userService, ICurrentUserService currentUserService)
    {
        _userService = userService;
        _currentUserService = currentUserService;
    }

    [HttpGet("me")]
    public async Task<ActionResult<ApiResponse<UserProfileDto>>> GetMe()
    {
        var id = _currentUserService.GetUserId();
        var profile = await _userService.GetCurrentUserAsync(id);
        return Ok(ApiResponse<UserProfileDto>.Ok(profile));
    }

    [HttpPut("me")]
    public async Task<ActionResult<ApiResponse<UserProfileDto>>> UpdateMe([FromForm] UpdateUserProfileDto request)
    {
        var id = _currentUserService.GetUserId();
        var updatedProfile = await _userService.UpdateCurrentUserAsync(id, request);
        return Ok(ApiResponse<UserProfileDto>.Ok(updatedProfile, "Profile updated successfully."));
    }

    [HttpDelete("me")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteMe()
    {
        var id = _currentUserService.GetUserId();
        await _userService.DeleteCurrentUserAsync(id);
        return Ok(ApiResponse<object>.Ok(null, "Account deleted successfully."));
    }

    [HttpPut("me/password")]
    public async Task<ActionResult<ApiResponse<object>>> ChangePassword([FromForm] ChangePasswordRequestDto request)
    {
        var id = _currentUserService.GetUserId();
        await _userService.ChangePasswordAsync(id, request);
        return Ok(ApiResponse<object>.Ok(null, "Password changed successfully."));
    }

    [HttpGet("me/document")]
    public async Task<IActionResult> GetProfilePhoto()
    {
        var id = _currentUserService.GetUserId();
        var (content, contentType, fileName) = await _userService.GetProfilePhotoAsync(id);
        return File(content, contentType, fileName);
    }
}
