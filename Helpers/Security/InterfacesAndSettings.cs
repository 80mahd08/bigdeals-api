namespace api.Helpers.Security;

public class JwtSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpirationMinutes { get; set; }
}

public interface IJwtTokenGenerator
{
    string GenerateToken(long idUtilisateur, string email, string role);
}

public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
}

public interface ICurrentUserService
{
    long GetUserId();
    long? Id { get; }
    string? Role { get; }
}
