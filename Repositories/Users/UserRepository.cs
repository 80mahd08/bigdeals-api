using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using api.Data.Connections;
using api.Interfaces.Users;
using api.Models;
using api.Models.Enums;

namespace api.Repositories.Users;

public class UserRepository : IUserRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public UserRepository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Utilisateur?> GetByIdAsync(long idUtilisateur)
    {
        using var connection = _connectionFactory.CreateConnection();
        var command = new SqlCommand("SELECT IdUtilisateur, Nom, Prenom, Email, Telephone, MotDePasseHash, Role, StatutCompte, DateCreation, DerniereConnexion, PhotoProfilUrl, Adresse, EstActif FROM Utilisateurs WHERE IdUtilisateur = @IdUtilisateur", (SqlConnection)connection);
        command.Parameters.AddWithValue("@IdUtilisateur", idUtilisateur);

        await ((SqlConnection)connection).OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new Utilisateur
            {
                IdUtilisateur = reader.GetInt64(0),
                Nom = reader.GetString(1),
                Prenom = reader.GetString(2),
                Email = reader.GetString(3),
                Telephone = reader.IsDBNull(4) ? null : reader.GetString(4),
                MotDePasseHash = reader.GetString(5),
                Role = (RoleUtilisateur)reader.GetInt32(6),
                StatutCompte = (StatutCompte)reader.GetInt32(7),
                DateCreation = reader.GetDateTime(8),
                DerniereConnexion = reader.IsDBNull(9) ? null : reader.GetDateTime(9),
                PhotoProfilUrl = reader.IsDBNull(10) ? null : reader.GetString(10),
                Adresse = reader.IsDBNull(11) ? null : reader.GetString(11),
                EstActif = reader.GetBoolean(12)
            };
        }

        return null;
    }

    public async Task UpdateUserAsync(Utilisateur user)
    {
        using var connection = _connectionFactory.CreateConnection();
        var command = new SqlCommand(@"
            UPDATE Utilisateurs 
            SET Telephone = @Telephone, 
                Adresse = @Adresse, 
                PhotoProfilUrl = @PhotoProfilUrl
            WHERE IdUtilisateur = @IdUtilisateur", 
            (SqlConnection)connection);

        command.Parameters.AddWithValue("@Telephone", (object?)user.Telephone ?? DBNull.Value);
        command.Parameters.AddWithValue("@Adresse", (object?)user.Adresse ?? DBNull.Value);
        command.Parameters.AddWithValue("@PhotoProfilUrl", (object?)user.PhotoProfilUrl ?? DBNull.Value);
        command.Parameters.AddWithValue("@IdUtilisateur", user.IdUtilisateur);

        await ((SqlConnection)connection).OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    public async Task DeleteUserAsync(long idUtilisateur)
    {
        using var connection = _connectionFactory.CreateConnection();
        var command = new SqlCommand(@"
            UPDATE Utilisateurs 
            SET EstActif = 0, 
                StatutCompte = 2 -- INACTIF
            WHERE IdUtilisateur = @IdUtilisateur", 
            (SqlConnection)connection);

        command.Parameters.AddWithValue("@IdUtilisateur", idUtilisateur);

        await ((SqlConnection)connection).OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    public async Task UpdatePasswordAsync(long idUtilisateur, string newPasswordHash)
    {
        using var connection = _connectionFactory.CreateConnection();
        var command = new SqlCommand(@"
            UPDATE Utilisateurs 
            SET MotDePasseHash = @MotDePasseHash
            WHERE IdUtilisateur = @IdUtilisateur", 
            (SqlConnection)connection);

        command.Parameters.AddWithValue("@MotDePasseHash", newPasswordHash);
        command.Parameters.AddWithValue("@IdUtilisateur", idUtilisateur);

        await ((SqlConnection)connection).OpenAsync();
        await command.ExecuteNonQueryAsync();
    }
}
