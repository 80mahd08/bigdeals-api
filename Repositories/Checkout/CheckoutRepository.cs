using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using api.Data.Connections;
using api.Interfaces.Checkout;
using api.Models;
using api.Models.Enums;

namespace api.Repositories.Checkout;

public class CheckoutRepository : ICheckoutRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public CheckoutRepository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<long> CreateCommandeAsync(Commande commande)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            INSERT INTO Commandes (IdAnnonce, IdAcheteur, IdAnnonceur, Montant, StatutCommande, DateCreation)
            VALUES (@IdAnnonce, @IdAcheteur, @IdAnnonceur, @Montant, @StatutCommande, @DateCreation);
            SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

        using var cmd = new SqlCommand(sql, (SqlConnection)connection);
        cmd.Parameters.AddWithValue("@IdAnnonce", commande.IdAnnonce);
        cmd.Parameters.AddWithValue("@IdAcheteur", commande.IdAcheteur);
        cmd.Parameters.AddWithValue("@IdAnnonceur", commande.IdAnnonceur);
        cmd.Parameters.AddWithValue("@Montant", commande.Montant);
        cmd.Parameters.AddWithValue("@StatutCommande", (int)commande.StatutCommande);
        cmd.Parameters.AddWithValue("@DateCreation", commande.DateCreation);

        if (connection.State != ConnectionState.Open) await ((SqlConnection)connection).OpenAsync();
        var idObj = await cmd.ExecuteScalarAsync();
        return idObj != null ? Convert.ToInt64(idObj) : 0;
    }

    public async Task<Commande?> GetCommandeByIdAsync(long idCommande)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "SELECT * FROM Commandes WHERE IdCommande = @IdCommande";

        using var cmd = new SqlCommand(sql, (SqlConnection)connection);
        cmd.Parameters.AddWithValue("@IdCommande", idCommande);

        if (connection.State != ConnectionState.Open) await ((SqlConnection)connection).OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Commande
            {
                IdCommande = (long)reader["IdCommande"],
                IdAnnonce = (long)reader["IdAnnonce"],
                IdAcheteur = (long)reader["IdAcheteur"],
                IdAnnonceur = (long)reader["IdAnnonceur"],
                Montant = (decimal)reader["Montant"],
                StatutCommande = (StatutCommande)(int)reader["StatutCommande"],
                DateCreation = (DateTime)reader["DateCreation"]
            };
        }
        return null;
    }

    public async Task<Commande?> GetPendingCommandeForUserAndAnnonceAsync(long idAcheteur, long idAnnonce)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "SELECT * FROM Commandes WHERE IdAcheteur = @IdAcheteur AND IdAnnonce = @IdAnnonce AND StatutCommande = 1";

        using var cmd = new SqlCommand(sql, (SqlConnection)connection);
        cmd.Parameters.AddWithValue("@IdAcheteur", idAcheteur);
        cmd.Parameters.AddWithValue("@IdAnnonce", idAnnonce);

        if (connection.State != ConnectionState.Open) await ((SqlConnection)connection).OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Commande
            {
                IdCommande = (long)reader["IdCommande"],
                IdAnnonce = (long)reader["IdAnnonce"],
                IdAcheteur = (long)reader["IdAcheteur"],
                IdAnnonceur = (long)reader["IdAnnonceur"],
                Montant = (decimal)reader["Montant"],
                StatutCommande = (StatutCommande)(int)reader["StatutCommande"],
                DateCreation = (DateTime)reader["DateCreation"]
            };
        }
        return null;
    }
}
