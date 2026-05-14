using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using api.Data.Connections;
using api.Interfaces.ProductPayments;
using api.Models;
using api.Models.Enums;

namespace api.Repositories.ProductPayments;

public class ProductPaymentRepository : IProductPaymentRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public ProductPaymentRepository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<long> CreatePaiementCommandeAsync(PaiementCommande paiement)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            INSERT INTO PaiementsCommandes (IdCommande, Montant, MethodePaiement, StatutPaiement, NumeroCarteMasque, DatePaiement)
            VALUES (@IdCommande, @Montant, @MethodePaiement, @StatutPaiement, @NumeroCarteMasque, @DatePaiement);
            SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

        using var cmd = new SqlCommand(sql, (SqlConnection)connection);
        cmd.Parameters.AddWithValue("@IdCommande", paiement.IdCommande);
        cmd.Parameters.AddWithValue("@Montant", paiement.Montant);
        cmd.Parameters.AddWithValue("@MethodePaiement", paiement.MethodePaiement);
        cmd.Parameters.AddWithValue("@StatutPaiement", (int)paiement.StatutPaiement);
        cmd.Parameters.AddWithValue("@NumeroCarteMasque", (object?)paiement.NumeroCarteMasque ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@DatePaiement", paiement.DatePaiement);

        if (connection.State != ConnectionState.Open) await ((SqlConnection)connection).OpenAsync();
        var idObj = await cmd.ExecuteScalarAsync();
        return idObj != null ? Convert.ToInt64(idObj) : 0;
    }

    public async Task UpdateCommandeStatutAsync(long idCommande, StatutCommande statut)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "UPDATE Commandes SET StatutCommande = @Statut WHERE IdCommande = @IdCommande";

        using var cmd = new SqlCommand(sql, (SqlConnection)connection);
        cmd.Parameters.AddWithValue("@Statut", (int)statut);
        cmd.Parameters.AddWithValue("@IdCommande", idCommande);

        if (connection.State != ConnectionState.Open) await ((SqlConnection)connection).OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }
}
