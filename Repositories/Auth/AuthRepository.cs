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
        var count = (int)await command.ExecuteScalarAsync();
        return count > 0;
    }

    public async Task<Utilisateur?> GetUserByEmailAsync(string email)
    {
        using var connection = _connectionFactory.CreateConnection();
        var command = new SqlCommand("SELECT IdUtilisateur, Nom, Prenom, Email, Telephone, MotDePasseHash, Role, StatutCompte, DateCreation, DerniereConnexion, PhotoProfilUrl, Adresse, EstActif FROM Utilisateurs WHERE Email = @Email", (SqlConnection)connection);
        command.Parameters.AddWithValue("@Email", email);

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
        return (long)await command.ExecuteScalarAsync();
    }
}
