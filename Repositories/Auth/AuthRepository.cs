using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using api.Data.Connections;
using api.Interfaces.Auth;
using api.Models;
using api.Models.Enums;

namespace api.Repositories.Auth;

public class AuthRepository : IAuthRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public AuthRepository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        using var connection = _connectionFactory.CreateConnection();
        var command = new SqlCommand("SELECT COUNT(1) FROM Utilisateurs WHERE Email = @Email", (SqlConnection)connection);
        command.Parameters.AddWithValue("@Email", email);

        await ((SqlConnection)connection).OpenAsync();
        var result = await command.ExecuteScalarAsync();
        var count = result != null ? Convert.ToInt32(result) : 0;
        return count > 0;
    }

    public async Task<Utilisateur?> GetUserByEmailAsync(string email)
    {
        using var connection = _connectionFactory.CreateConnection();
        var command = new SqlCommand("SELECT IdUtilisateur, Nom, Prenom, Email, Telephone, MotDePasseHash, Role, StatutCompte, DateCreation, DerniereConnexion, PhotoProfilUrl, Adresse, EstActif, Ville FROM Utilisateurs WHERE Email = @Email", (SqlConnection)connection);
        command.Parameters.AddWithValue("@Email", email);

        await ((SqlConnection)connection).OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new Utilisateur
            {
                IdUtilisateur = reader.GetInt64(reader.GetOrdinal("IdUtilisateur")),
                Nom = reader.GetString(reader.GetOrdinal("Nom")),
                Prenom = reader.GetString(reader.GetOrdinal("Prenom")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                Telephone = reader.IsDBNull(reader.GetOrdinal("Telephone")) ? null : reader.GetString(reader.GetOrdinal("Telephone")),
                MotDePasseHash = reader.GetString(reader.GetOrdinal("MotDePasseHash")),
                Role = (RoleUtilisateur)reader.GetInt32(reader.GetOrdinal("Role")),
                StatutCompte = (StatutCompte)reader.GetInt32(reader.GetOrdinal("StatutCompte")),
                DateCreation = reader.GetDateTime(reader.GetOrdinal("DateCreation")),
                DerniereConnexion = reader.IsDBNull(reader.GetOrdinal("DerniereConnexion")) ? null : reader.GetDateTime(reader.GetOrdinal("DerniereConnexion")),
                PhotoProfilUrl = reader.IsDBNull(reader.GetOrdinal("PhotoProfilUrl")) ? null : reader.GetString(reader.GetOrdinal("PhotoProfilUrl")),
                Adresse = reader.IsDBNull(reader.GetOrdinal("Adresse")) ? null : reader.GetString(reader.GetOrdinal("Adresse")),
                EstActif = reader.GetBoolean(reader.GetOrdinal("EstActif")),
                Ville = reader.IsDBNull(reader.GetOrdinal("Ville")) ? null : reader.GetString(reader.GetOrdinal("Ville"))
            };
        }

        return null;
    }

    public async Task<long> CreateUserAsync(Utilisateur user)
    {
        using var connection = _connectionFactory.CreateConnection();
        var command = new SqlCommand(@"
            INSERT INTO Utilisateurs (Nom, Prenom, Email, MotDePasseHash, Role, StatutCompte, DateCreation, EstActif)
            OUTPUT INSERTED.IdUtilisateur
            VALUES (@Nom, @Prenom, @Email, @MotDePasseHash, @Role, @StatutCompte, @DateCreation, @EstActif)", 
            (SqlConnection)connection);

        command.Parameters.AddWithValue("@Nom", user.Nom);
        command.Parameters.AddWithValue("@Prenom", user.Prenom);
        command.Parameters.AddWithValue("@Email", user.Email);
        command.Parameters.AddWithValue("@MotDePasseHash", user.MotDePasseHash);
        command.Parameters.AddWithValue("@Role", (int)user.Role);
        command.Parameters.AddWithValue("@StatutCompte", (int)user.StatutCompte);
        command.Parameters.AddWithValue("@DateCreation", user.DateCreation);
        command.Parameters.AddWithValue("@EstActif", user.EstActif);

        await ((SqlConnection)connection).OpenAsync();
        var result = await command.ExecuteScalarAsync();
        return result != null ? Convert.ToInt64(result) : 0;
    }

    public async Task InvalidateUnusedResetTokensAsync(long idUtilisateur)
    {
        using var connection = _connectionFactory.CreateConnection();
        var command = new SqlCommand(@"
            UPDATE PasswordResetTokens 
            SET EstUtilise = 1, DateUtilisation = SYSUTCDATETIME() 
            WHERE IdUtilisateur = @IdUtilisateur AND EstUtilise = 0", 
            (SqlConnection)connection);
        command.Parameters.AddWithValue("@IdUtilisateur", idUtilisateur);

        await ((SqlConnection)connection).OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    public async Task CreatePasswordResetTokenAsync(long idUtilisateur, string tokenHash, DateTime expiration)
    {
        using var connection = _connectionFactory.CreateConnection();
        var command = new SqlCommand(@"
            INSERT INTO PasswordResetTokens (IdUtilisateur, TokenHash, DateExpiration, DateCreation, EstUtilise)
            VALUES (@IdUtilisateur, @TokenHash, @DateExpiration, SYSUTCDATETIME(), 0)", 
            (SqlConnection)connection);
        command.Parameters.AddWithValue("@IdUtilisateur", idUtilisateur);
        command.Parameters.AddWithValue("@TokenHash", tokenHash);
        command.Parameters.AddWithValue("@DateExpiration", expiration);

        await ((SqlConnection)connection).OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    public async Task<(long IdPasswordResetToken, long IdUtilisateur)?> GetValidResetTokenAsync(string tokenHash)
    {
        using var connection = _connectionFactory.CreateConnection();
        var command = new SqlCommand(@"
            SELECT IdPasswordResetToken, IdUtilisateur 
            FROM PasswordResetTokens 
            WHERE TokenHash = @TokenHash AND EstUtilise = 0 AND DateExpiration > SYSUTCDATETIME()", 
            (SqlConnection)connection);
        command.Parameters.AddWithValue("@TokenHash", tokenHash);

        await ((SqlConnection)connection).OpenAsync();
        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return (reader.GetInt64(0), reader.GetInt64(1));
        }
        return null;
    }

    public async Task MarkResetTokenAsUsedAsync(long idPasswordResetToken)
    {
        using var connection = _connectionFactory.CreateConnection();
        var command = new SqlCommand(@"
            UPDATE PasswordResetTokens 
            SET EstUtilise = 1, DateUtilisation = SYSUTCDATETIME() 
            WHERE IdPasswordResetToken = @IdPasswordResetToken", 
            (SqlConnection)connection);
        command.Parameters.AddWithValue("@IdPasswordResetToken", idPasswordResetToken);

        await ((SqlConnection)connection).OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    public async Task UpdatePasswordHashAsync(long idUtilisateur, string newPasswordHash)
    {
        using var connection = _connectionFactory.CreateConnection();
        var command = new SqlCommand(@"
            UPDATE Utilisateurs SET MotDePasseHash = @Hash WHERE IdUtilisateur = @IdUtilisateur", 
            (SqlConnection)connection);
        command.Parameters.AddWithValue("@Hash", newPasswordHash);
        command.Parameters.AddWithValue("@IdUtilisateur", idUtilisateur);

        await ((SqlConnection)connection).OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    public async Task UpdateRefreshTokenAsync(long idUtilisateur, string? refreshToken, DateTime? expiry)
    {
        using var connection = _connectionFactory.CreateConnection();
        var command = new SqlCommand(@"
            UPDATE Utilisateurs 
            SET RefreshToken = @Token, RefreshTokenExpiry = @Expiry 
            WHERE IdUtilisateur = @IdUtilisateur", 
            (SqlConnection)connection);
        command.Parameters.AddWithValue("@IdUtilisateur", idUtilisateur);
        command.Parameters.AddWithValue("@Token", (object?)refreshToken ?? DBNull.Value);
        command.Parameters.AddWithValue("@Expiry", (object?)expiry ?? DBNull.Value);

        await ((SqlConnection)connection).OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    public async Task<Utilisateur?> GetUserByRefreshTokenAsync(string refreshToken)
    {
        using var connection = _connectionFactory.CreateConnection();
        var command = new SqlCommand(@"
            SELECT IdUtilisateur, Nom, Prenom, Email, Telephone, MotDePasseHash, Role, StatutCompte, DateCreation, DerniereConnexion, PhotoProfilUrl, Adresse, EstActif, Ville, RefreshToken, RefreshTokenExpiry 
            FROM Utilisateurs 
            WHERE RefreshToken = @Token", 
            (SqlConnection)connection);
        command.Parameters.AddWithValue("@Token", refreshToken);

        await ((SqlConnection)connection).OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new Utilisateur
            {
                IdUtilisateur = reader.GetInt64(reader.GetOrdinal("IdUtilisateur")),
                Nom = reader.GetString(reader.GetOrdinal("Nom")),
                Prenom = reader.GetString(reader.GetOrdinal("Prenom")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                Telephone = reader.IsDBNull(reader.GetOrdinal("Telephone")) ? null : reader.GetString(reader.GetOrdinal("Telephone")),
                MotDePasseHash = reader.GetString(reader.GetOrdinal("MotDePasseHash")),
                Role = (RoleUtilisateur)reader.GetInt32(reader.GetOrdinal("Role")),
                StatutCompte = (StatutCompte)reader.GetInt32(reader.GetOrdinal("StatutCompte")),
                DateCreation = reader.GetDateTime(reader.GetOrdinal("DateCreation")),
                DerniereConnexion = reader.IsDBNull(reader.GetOrdinal("DerniereConnexion")) ? null : reader.GetDateTime(reader.GetOrdinal("DerniereConnexion")),
                PhotoProfilUrl = reader.IsDBNull(reader.GetOrdinal("PhotoProfilUrl")) ? null : reader.GetString(reader.GetOrdinal("PhotoProfilUrl")),
                Adresse = reader.IsDBNull(reader.GetOrdinal("Adresse")) ? null : reader.GetString(reader.GetOrdinal("Adresse")),
                EstActif = reader.GetBoolean(reader.GetOrdinal("EstActif")),
                Ville = reader.IsDBNull(reader.GetOrdinal("Ville")) ? null : reader.GetString(reader.GetOrdinal("Ville")),
                RefreshToken = reader.IsDBNull(reader.GetOrdinal("RefreshToken")) ? null : reader.GetString(reader.GetOrdinal("RefreshToken")),
                RefreshTokenExpiry = reader.IsDBNull(reader.GetOrdinal("RefreshTokenExpiry")) ? null : reader.GetDateTime(reader.GetOrdinal("RefreshTokenExpiry"))
            };
        }

        return null;
    }

    public async Task<DateTime?> GetLatestResetTokenDateAsync(long idUtilisateur)
    {
        using var connection = _connectionFactory.CreateConnection();
        var command = new SqlCommand(@"
            SELECT TOP 1 DateCreation FROM PasswordResetTokens 
            WHERE IdUtilisateur = @IdUtilisateur 
            ORDER BY DateCreation DESC", 
            (SqlConnection)connection);
        command.Parameters.AddWithValue("@IdUtilisateur", idUtilisateur);

        await ((SqlConnection)connection).OpenAsync();
        var result = await command.ExecuteScalarAsync();
        return result != DBNull.Value ? (DateTime?)result : null;
    }
}
