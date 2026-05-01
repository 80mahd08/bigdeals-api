using System;
using System.Security.Cryptography;

namespace api.Helpers.Security;

public class PasswordHasher : IPasswordHasher
{
    private const int Iterations = 100000;
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const string FormatPrefix = "PBKDF2$100000$";

    public string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty.", nameof(password));

        byte[] salt = new byte[SaltSize];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            password: password,
            salt: salt,
            iterations: Iterations,
            hashAlgorithm: HashAlgorithmName.SHA256,
            outputLength: HashSize);

        string base64Salt = Convert.ToBase64String(salt);
        string base64Hash = Convert.ToBase64String(hash);

        return $"{FormatPrefix}{base64Salt}${base64Hash}";
    }

    public bool VerifyPassword(string password, string storedHash)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(storedHash))
            return false;

        var parts = storedHash.Split('$');
        if (parts.Length != 4 || parts[0] != "PBKDF2" || parts[1] != "100000")
            return false;

        try
        {
            byte[] salt = Convert.FromBase64String(parts[2]);
            byte[] expectedHash = Convert.FromBase64String(parts[3]);

            byte[] actualHash = Rfc2898DeriveBytes.Pbkdf2(
                password: password,
                salt: salt,
                iterations: Iterations,
                hashAlgorithm: HashAlgorithmName.SHA256,
                outputLength: HashSize);

            return CryptographicOperations.FixedTimeEquals(expectedHash, actualHash);
        }
        catch
        {
            return false;
        }
    }
}
