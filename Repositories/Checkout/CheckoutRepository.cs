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
            INSERT INTO Commandes (IdAnnonce, IdAcheteur, IdAnnonceur, MontantAnnonce, FraisLivraison, Montant, StatutCommande, StatutLivraison, AdresseLivraison, VilleLivraison, TelephoneLivraison, DateCreation)
            VALUES (@IdAnnonce, @IdAcheteur, @IdAnnonceur, @MontantAnnonce, @FraisLivraison, @Montant, @StatutCommande, @StatutLivraison, @AdresseLivraison, @VilleLivraison, @TelephoneLivraison, @DateCreation);
            SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

        using var cmd = new SqlCommand(sql, (SqlConnection)connection);
        cmd.Parameters.AddWithValue("@IdAnnonce", commande.IdAnnonce);
        cmd.Parameters.AddWithValue("@IdAcheteur", commande.IdAcheteur);
        cmd.Parameters.AddWithValue("@IdAnnonceur", commande.IdAnnonceur);
        cmd.Parameters.AddWithValue("@MontantAnnonce", commande.MontantAnnonce);
        cmd.Parameters.AddWithValue("@FraisLivraison", commande.FraisLivraison);
        cmd.Parameters.AddWithValue("@Montant", commande.Montant);
        cmd.Parameters.AddWithValue("@StatutCommande", (int)commande.StatutCommande);
        cmd.Parameters.AddWithValue("@StatutLivraison", (int)commande.StatutLivraison);
        cmd.Parameters.AddWithValue("@AdresseLivraison", (object?)commande.AdresseLivraison ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@VilleLivraison", (object?)commande.VilleLivraison ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@TelephoneLivraison", (object?)commande.TelephoneLivraison ?? DBNull.Value);
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
                MontantAnnonce = reader["MontantAnnonce"] != DBNull.Value ? (decimal)reader["MontantAnnonce"] : 0,
                FraisLivraison = reader["FraisLivraison"] != DBNull.Value ? (decimal)reader["FraisLivraison"] : 0,
                Montant = (decimal)reader["Montant"],
                StatutCommande = (StatutCommande)(int)reader["StatutCommande"],
                StatutLivraison = (StatutLivraison)(int)reader["StatutLivraison"],
                AdresseLivraison = reader["AdresseLivraison"]?.ToString(),
                VilleLivraison = reader["VilleLivraison"]?.ToString(),
                TelephoneLivraison = reader["TelephoneLivraison"]?.ToString(),
                DateCreation = (DateTime)reader["DateCreation"],
                DateExpedition = reader["DateExpedition"] != DBNull.Value ? (DateTime)reader["DateExpedition"] : null,
                DateLivraison = reader["DateLivraison"] != DBNull.Value ? (DateTime)reader["DateLivraison"] : null,
                DateDerniereMiseAJourLivraison = reader["DateDerniereMiseAJourLivraison"] != DBNull.Value ? (DateTime)reader["DateDerniereMiseAJourLivraison"] : null
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
                MontantAnnonce = reader["MontantAnnonce"] != DBNull.Value ? (decimal)reader["MontantAnnonce"] : 0,
                FraisLivraison = reader["FraisLivraison"] != DBNull.Value ? (decimal)reader["FraisLivraison"] : 0,
                Montant = (decimal)reader["Montant"],
                StatutCommande = (StatutCommande)(int)reader["StatutCommande"],
                StatutLivraison = (StatutLivraison)(int)reader["StatutLivraison"],
                AdresseLivraison = reader["AdresseLivraison"]?.ToString(),
                VilleLivraison = reader["VilleLivraison"]?.ToString(),
                TelephoneLivraison = reader["TelephoneLivraison"]?.ToString(),
                DateCreation = (DateTime)reader["DateCreation"]
            };
        }
        return null;
    }
}
