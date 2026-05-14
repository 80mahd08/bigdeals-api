using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using api.Data.Connections;
using api.Interfaces.Orders;
using api.Models;
using api.Models.Enums;

namespace api.Repositories.Orders;

public class OrdersRepository : IOrdersRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public OrdersRepository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    // ─── Helper: map a reader row to Commande ────────────────────
    private static Commande MapCommande(IDataReader reader)
    {
        return new Commande
        {
            IdCommande = Convert.ToInt64(reader["IdCommande"]),
            IdAnnonce = Convert.ToInt64(reader["IdAnnonce"]),
            IdAcheteur = Convert.ToInt64(reader["IdAcheteur"]),
            IdAnnonceur = Convert.ToInt64(reader["IdAnnonceur"]),
            Montant = Convert.ToDecimal(reader["Montant"]),
            StatutCommande = (StatutCommande)Convert.ToInt32(reader["StatutCommande"]),
            DateCreation = Convert.ToDateTime(reader["DateCreation"]),
            StatutLivraison = (StatutLivraison)Convert.ToInt32(reader["StatutLivraison"]),
            AdresseLivraison = reader["AdresseLivraison"] == DBNull.Value ? null : reader["AdresseLivraison"].ToString(),
            VilleLivraison = reader["VilleLivraison"] == DBNull.Value ? null : reader["VilleLivraison"].ToString(),
            TelephoneLivraison = reader["TelephoneLivraison"] == DBNull.Value ? null : reader["TelephoneLivraison"].ToString(),
            DateExpedition = reader["DateExpedition"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["DateExpedition"]),
            DateLivraison = reader["DateLivraison"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["DateLivraison"]),
            DateDerniereMiseAJourLivraison = reader["DateDerniereMiseAJourLivraison"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["DateDerniereMiseAJourLivraison"]),
            AnnonceTitre = reader["AnnonceTitre"] == DBNull.Value ? null : reader["AnnonceTitre"].ToString()
        };
    }

    // ─── Buyer orders ────────────────────────────────────────────
    public async Task<IEnumerable<Commande>> GetOrdersByUserIdAsync(long userId)
    {
        var orders = new List<Commande>();
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT c.IdCommande, c.IdAnnonce, c.IdAcheteur, c.IdAnnonceur, c.Montant, c.StatutCommande, c.DateCreation,
                   c.StatutLivraison, c.AdresseLivraison, c.VilleLivraison, c.TelephoneLivraison,
                   c.DateExpedition, c.DateLivraison, c.DateDerniereMiseAJourLivraison,
                   a.Titre as AnnonceTitre
            FROM Commandes c
            INNER JOIN Annonces a ON c.IdAnnonce = a.IdAnnonce
            WHERE c.IdAcheteur = @IdAcheteur 
            ORDER BY c.DateCreation DESC";

        using var cmd = new SqlCommand(sql, (SqlConnection)connection);
        cmd.Parameters.AddWithValue("@IdAcheteur", userId);

        if (connection.State != ConnectionState.Open) await ((SqlConnection)connection).OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            orders.Add(MapCommande(reader));
        return orders;
    }

    // ─── Announcer orders ────────────────────────────────────────
    public async Task<IEnumerable<Commande>> GetOrdersByAnnouncerIdAsync(long announcerId)
    {
        var orders = new List<Commande>();
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT c.IdCommande, c.IdAnnonce, c.IdAcheteur, c.IdAnnonceur, c.Montant, c.StatutCommande, c.DateCreation,
                   c.StatutLivraison, c.AdresseLivraison, c.VilleLivraison, c.TelephoneLivraison,
                   c.DateExpedition, c.DateLivraison, c.DateDerniereMiseAJourLivraison,
                   a.Titre as AnnonceTitre
            FROM Commandes c
            INNER JOIN Annonces a ON c.IdAnnonce = a.IdAnnonce
            WHERE c.IdAnnonceur = @IdAnnonceur 
            ORDER BY c.DateCreation DESC";

        using var cmd = new SqlCommand(sql, (SqlConnection)connection);
        cmd.Parameters.AddWithValue("@IdAnnonceur", announcerId);

        if (connection.State != ConnectionState.Open) await ((SqlConnection)connection).OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            orders.Add(MapCommande(reader));
        return orders;
    }

    // ─── Single order by ID ──────────────────────────────────────
    public async Task<Commande?> GetOrderByIdAsync(long orderId)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT c.IdCommande, c.IdAnnonce, c.IdAcheteur, c.IdAnnonceur, c.Montant, c.StatutCommande, c.DateCreation,
                   c.StatutLivraison, c.AdresseLivraison, c.VilleLivraison, c.TelephoneLivraison,
                   c.DateExpedition, c.DateLivraison, c.DateDerniereMiseAJourLivraison,
                   a.Titre as AnnonceTitre
            FROM Commandes c
            INNER JOIN Annonces a ON c.IdAnnonce = a.IdAnnonce
            WHERE c.IdCommande = @IdCommande";

        using var cmd = new SqlCommand(sql, (SqlConnection)connection);
        cmd.Parameters.AddWithValue("@IdCommande", orderId);

        if (connection.State != ConnectionState.Open) await ((SqlConnection)connection).OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
            return MapCommande(reader);
        return null;
    }

    // ─── Admin: all orders with JOINs ────────────────────────────
    public async Task<IEnumerable<dynamic>> GetAllOrdersAsync()
    {
        var orders = new List<dynamic>();
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT c.*, 
                   a.Titre AS AnnonceTitre, 
                   (acheteur.Prenom + ' ' + acheteur.Nom) AS AcheteurNom,
                   (annonceur.Prenom + ' ' + annonceur.Nom) AS AnnonceurNom
            FROM Commandes c
            LEFT JOIN Annonces a ON c.IdAnnonce = a.IdAnnonce
            LEFT JOIN Utilisateurs acheteur ON c.IdAcheteur = acheteur.IdUtilisateur
            LEFT JOIN Utilisateurs annonceur ON c.IdAnnonceur = annonceur.IdUtilisateur
            ORDER BY c.DateCreation DESC";

        using var cmd = new SqlCommand(sql, (SqlConnection)connection);

        if (connection.State != ConnectionState.Open) await ((SqlConnection)connection).OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            orders.Add(new
            {
                IdCommande = (long)reader["IdCommande"],
                IdAnnonce = (long)reader["IdAnnonce"],
                IdAcheteur = (long)reader["IdAcheteur"],
                IdAnnonceur = (long)reader["IdAnnonceur"],
                Montant = (decimal)reader["Montant"],
                StatutCommande = (int)reader["StatutCommande"],
                DateCreation = (DateTime)reader["DateCreation"],
                StatutLivraison = (int)reader["StatutLivraison"],
                AdresseLivraison = reader["AdresseLivraison"] as string,
                VilleLivraison = reader["VilleLivraison"] as string,
                TelephoneLivraison = reader["TelephoneLivraison"] as string,
                DateExpedition = reader["DateExpedition"] as DateTime?,
                DateLivraison = reader["DateLivraison"] as DateTime?,
                DateDerniereMiseAJourLivraison = reader["DateDerniereMiseAJourLivraison"] as DateTime?,
                AnnonceTitre = reader["AnnonceTitre"] as string ?? "—",
                AcheteurNom = reader["AcheteurNom"] as string ?? "—",
                AnnonceurNom = reader["AnnonceurNom"] as string ?? "—"
            });
        }
        return orders;
    }

    // ─── Create order (with delivery address) ────────────────────
    public async Task<long> CreateOrderAsync(Commande order)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            INSERT INTO Commandes (IdAnnonce, IdAcheteur, IdAnnonceur, Montant, StatutCommande, StatutLivraison,
                                   AdresseLivraison, VilleLivraison, TelephoneLivraison, DateCreation)
            VALUES (@IdAnnonce, @IdAcheteur, @IdAnnonceur, @Montant, @StatutCommande, @StatutLivraison,
                    @AdresseLivraison, @VilleLivraison, @TelephoneLivraison, @DateCreation);
            SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

        using var cmd = new SqlCommand(sql, (SqlConnection)connection);
        cmd.Parameters.AddWithValue("@IdAnnonce", order.IdAnnonce);
        cmd.Parameters.AddWithValue("@IdAcheteur", order.IdAcheteur);
        cmd.Parameters.AddWithValue("@IdAnnonceur", order.IdAnnonceur);
        cmd.Parameters.AddWithValue("@Montant", order.Montant);
        cmd.Parameters.AddWithValue("@StatutCommande", (int)order.StatutCommande);
        cmd.Parameters.AddWithValue("@StatutLivraison", (int)order.StatutLivraison);
        cmd.Parameters.AddWithValue("@AdresseLivraison", (object?)order.AdresseLivraison ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@VilleLivraison", (object?)order.VilleLivraison ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@TelephoneLivraison", (object?)order.TelephoneLivraison ?? DBNull.Value);

        cmd.Parameters.AddWithValue("@DateCreation", order.DateCreation);

        if (connection.State != ConnectionState.Open) await ((SqlConnection)connection).OpenAsync();
        var idObj = await cmd.ExecuteScalarAsync();
        return idObj != null ? Convert.ToInt64(idObj) : 0;
    }

    // ─── Update payment/order status ─────────────────────────────
    public async Task<bool> UpdateOrderStatusAsync(long orderId, int status)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "UPDATE Commandes SET StatutCommande = @Status WHERE IdCommande = @IdCommande";

        using var cmd = new SqlCommand(sql, (SqlConnection)connection);
        cmd.Parameters.AddWithValue("@Status", status);
        cmd.Parameters.AddWithValue("@IdCommande", orderId);

        if (connection.State != ConnectionState.Open) await ((SqlConnection)connection).OpenAsync();
        var rows = await cmd.ExecuteNonQueryAsync();
        return rows > 0;
    }

    // ─── Update delivery status ──────────────────────────────────
    public async Task<bool> UpdateDeliveryStatusAsync(long orderId, int statutLivraison, string? notes,
        DateTime? dateExpedition, DateTime? dateLivraison)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            UPDATE Commandes 
            SET StatutLivraison = @StatutLivraison,
                DateExpedition = COALESCE(@DateExpedition, DateExpedition),
                DateLivraison = COALESCE(@DateLivraison, DateLivraison),
                DateDerniereMiseAJourLivraison = @DateMaj
            WHERE IdCommande = @IdCommande";

        using var cmd = new SqlCommand(sql, (SqlConnection)connection);
        cmd.Parameters.AddWithValue("@StatutLivraison", statutLivraison);

        cmd.Parameters.AddWithValue("@DateExpedition", (object?)dateExpedition ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@DateLivraison", (object?)dateLivraison ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@DateMaj", DateTime.UtcNow);
        cmd.Parameters.AddWithValue("@IdCommande", orderId);

        if (connection.State != ConnectionState.Open) await ((SqlConnection)connection).OpenAsync();
        var rows = await cmd.ExecuteNonQueryAsync();
        return rows > 0;
    }
}
