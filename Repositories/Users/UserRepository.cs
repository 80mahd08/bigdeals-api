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
        var command = new SqlCommand("SELECT IdUtilisateur, Nom, Prenom, Email, Telephone, MotDePasseHash, Role, StatutCompte, DateCreation, DerniereConnexion, PhotoProfilUrl, Adresse, EstActif, Ville FROM Utilisateurs WHERE IdUtilisateur = @IdUtilisateur", (SqlConnection)connection);
        command.Parameters.AddWithValue("@IdUtilisateur", idUtilisateur);

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

    public async Task UpdateUserAsync(Utilisateur user)
    {
        using var connection = _connectionFactory.CreateConnection();
        var command = new SqlCommand(@"
            UPDATE Utilisateurs 
            SET Telephone = @Telephone, 
                Adresse = @Adresse, 
                Ville = @Ville,
                PhotoProfilUrl = @PhotoProfilUrl
            WHERE IdUtilisateur = @IdUtilisateur", 
            (SqlConnection)connection);

        command.Parameters.AddWithValue("@Telephone", (object?)user.Telephone ?? DBNull.Value);
        command.Parameters.AddWithValue("@Adresse", (object?)user.Adresse ?? DBNull.Value);
        command.Parameters.AddWithValue("@Ville", (object?)user.Ville ?? DBNull.Value);
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
