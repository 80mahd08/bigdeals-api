using System.ComponentModel.DataAnnotations;

namespace api.Dtos.Auth;

public class RegisterRequestDto
{
    [Required]
    public string Nom { get; set; } = null!;
    
    [Required]
    public string Prenom { get; set; } = null!;
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
    
    [Required]
    public string Password { get; set; } = null!;
}

public class LoginRequestDto
{
    [Required]
    public string Email { get; set; } = null!;
    
    [Required]
    public string Password { get; set; } = null!;
}

public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public api.Dtos.Users.UserProfileDto User { get; set; } = new();
}
