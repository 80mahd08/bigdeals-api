using System;
using System.Threading.Tasks; 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using api.Common;
using api.Dtos.Users;
using api.Helpers.Security;
using api.Interfaces.Users;
using api.Interfaces.Favorites;
using api.Interfaces.Contacts;

namespace api.Controllers;

[Route("api/users")]
[ApiController]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFavoriteService _favoriteService;
    private readonly IContactService _contactService;

    public UsersController(
        IUserService userService, 
        ICurrentUserService currentUserService,
        IFavoriteService favoriteService,
        IContactService contactService)
    {
        _userService = userService;
        _currentUserService = currentUserService;
        _favoriteService = favoriteService;
        _contactService = contactService;
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

    [HttpGet("me/favorites")]
    public async Task<IActionResult> GetMyFavorites([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var userId = _currentUserService.GetUserId();
        var favorites = await _favoriteService.GetUserFavoritesAsync(userId, pageNumber, pageSize);
        return Ok(ApiResponse<object>.Ok(favorites));
    }

    [HttpGet("me/contacts")]
    public async Task<IActionResult> GetMyOutgoingContacts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var userId = _currentUserService.GetUserId();
        var contacts = await _contactService.GetMyOutgoingContactsAsync(userId, pageNumber, pageSize);
        return Ok(ApiResponse<object>.Ok(contacts));
    }
    [HttpGet("{id}/profile")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<PublicProfileDto>>> GetPublicProfile(long id)
    {
        var profile = await _userService.GetPublicProfileAsync(id);
        return Ok(ApiResponse<PublicProfileDto>.Ok(profile));
    }
}
