using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using api.Common;
using api.Data.Connections;
using api.Dtos.AnnonceurPayments;
using api.Interfaces.AnnonceurPayments;
using api.Models;
using api.Models.Enums;
using api.Exceptions;

namespace api.Repositories.AnnonceurPayments;

public class AnnonceurPaymentRepository : IAnnonceurPaymentRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public AnnonceurPaymentRepository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<long> CreateAsync(AnnonceurPayment payment)
    {
        using var connection = _connectionFactory.CreateConnection();
        var command = new SqlCommand(@"
            INSERT INTO PaiementsAnnonceur (
                IdUtilisateur, IdDemandeAnnonceur, Provider, ProviderPaymentId, 
                DeveloperTrackingId, Montant, StatutPaiement, PaymentUrl, 
                RawResponseJson, DateCreation, DateConfirmation
            )
            OUTPUT INSERTED.IdPaiementAnnonceur
            VALUES (
                @IdUtilisateur, @IdDemandeAnnonceur, @Provider, @ProviderPaymentId, 
                @DeveloperTrackingId, @Montant, @StatutPaiement, @PaymentUrl, 
                @RawResponseJson, @DateCreation, @DateConfirmation
            )", (SqlConnection)connection);

        command.Parameters.AddWithValue("@IdUtilisateur", payment.UserId);
        command.Parameters.AddWithValue("@IdDemandeAnnonceur", payment.DemandeAnnonceurId);
        command.Parameters.AddWithValue("@Provider", payment.Provider);
        command.Parameters.AddWithValue("@ProviderPaymentId", (object?)payment.ProviderPaymentId ?? DBNull.Value);
        command.Parameters.AddWithValue("@DeveloperTrackingId", payment.DeveloperTrackingId);
        command.Parameters.AddWithValue("@Montant", payment.Amount);
        command.Parameters.AddWithValue("@StatutPaiement", (int)payment.PaymentStatus);
        command.Parameters.AddWithValue("@PaymentUrl", (object?)payment.PaymentUrl ?? DBNull.Value);
        command.Parameters.AddWithValue("@RawResponseJson", (object?)payment.RawResponseJson ?? DBNull.Value);
        command.Parameters.AddWithValue("@DateCreation", payment.CreatedAt);
        command.Parameters.AddWithValue("@DateConfirmation", (object?)payment.ConfirmedAt ?? DBNull.Value);

        await ((SqlConnection)connection).OpenAsync();
        var result = await command.ExecuteScalarAsync();
        return result != null ? Convert.ToInt64(result) : 0;
    }

    public async Task<AnnonceurPayment?> GetByIdAsync(long annonceurPaymentId)
    {
        using var connection = _connectionFactory.CreateConnection();
        var command = new SqlCommand(@"
            SELECT IdPaiementAnnonceur, IdUtilisateur, IdDemandeAnnonceur, Provider, 
                   ProviderPaymentId, DeveloperTrackingId, Montant, StatutPaiement, 
                   PaymentUrl, RawResponseJson, DateCreation, DateConfirmation 
            FROM PaiementsAnnonceur 
            WHERE IdPaiementAnnonceur = @IdPaiementAnnonceur", (SqlConnection)connection);
        
        command.Parameters.AddWithValue("@IdPaiementAnnonceur", annonceurPaymentId);

        await ((SqlConnection)connection).OpenAsync();
        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapFromReader(reader);
        }
        return null;
    }

    public async Task<AnnonceurPayment?> GetByDemandeAnnonceurIdAsync(long demandeAnnonceurId)
    {
        using var connection = _connectionFactory.CreateConnection();
        var command = new SqlCommand(@"
            SELECT IdPaiementAnnonceur, IdUtilisateur, IdDemandeAnnonceur, Provider, 
                   ProviderPaymentId, DeveloperTrackingId, Montant, StatutPaiement, 
                   PaymentUrl, RawResponseJson, DateCreation, DateConfirmation 
            FROM PaiementsAnnonceur 
            WHERE IdDemandeAnnonceur = @IdDemandeAnnonceur", (SqlConnection)connection);
        
        command.Parameters.AddWithValue("@IdDemandeAnnonceur", demandeAnnonceurId);

        await ((SqlConnection)connection).OpenAsync();
        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapFromReader(reader);
        }
        return null;
    }

    public async Task<AnnonceurPayment?> GetByDeveloperTrackingIdAsync(string developerTrackingId)
    {
        using var connection = _connectionFactory.CreateConnection();
        var command = new SqlCommand(@"
            SELECT IdPaiementAnnonceur, IdUtilisateur, IdDemandeAnnonceur, Provider, 
                   ProviderPaymentId, DeveloperTrackingId, Montant, StatutPaiement, 
                   PaymentUrl, RawResponseJson, DateCreation, DateConfirmation 
            FROM PaiementsAnnonceur 
            WHERE DeveloperTrackingId = @DeveloperTrackingId", (SqlConnection)connection);
        
        command.Parameters.AddWithValue("@DeveloperTrackingId", developerTrackingId);

        await ((SqlConnection)connection).OpenAsync();
        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapFromReader(reader);
        }
        return null;
    }

    public async Task<AnnonceurPayment?> GetByProviderPaymentIdAsync(string providerPaymentId)
    {
        if (string.IsNullOrEmpty(providerPaymentId)) return null;

        using var connection = _connectionFactory.CreateConnection();
        var command = new SqlCommand(@"
            SELECT IdPaiementAnnonceur, IdUtilisateur, IdDemandeAnnonceur, Provider, 
                   ProviderPaymentId, DeveloperTrackingId, Montant, StatutPaiement, 
                   PaymentUrl, RawResponseJson, DateCreation, DateConfirmation 
            FROM PaiementsAnnonceur 
            WHERE ProviderPaymentId = @ProviderPaymentId", (SqlConnection)connection);
        
        command.Parameters.AddWithValue("@ProviderPaymentId", providerPaymentId);

        await ((SqlConnection)connection).OpenAsync();
        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapFromReader(reader);
        }
        return null;
    }

    public async Task<IReadOnlyList<AnnonceurPaymentDto>> GetByUserIdAsync(long userId)
    {
        var list = new List<AnnonceurPaymentDto>();
        using var connection = _connectionFactory.CreateConnection();
        var command = new SqlCommand(@"
            SELECT IdPaiementAnnonceur, IdUtilisateur, IdDemandeAnnonceur, Provider, 
                   ProviderPaymentId, DeveloperTrackingId, Montant, StatutPaiement, 
                   PaymentUrl, DateCreation, DateConfirmation 
            FROM PaiementsAnnonceur 
            WHERE IdUtilisateur = @IdUtilisateur 
            ORDER BY DateCreation DESC", (SqlConnection)connection);
        
        command.Parameters.AddWithValue("@IdUtilisateur", userId);

        await ((SqlConnection)connection).OpenAsync();
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(MapToDtoFromReader(reader));
        }
        return list;
    }

    public async Task<PagedResponse<AnnonceurPaymentDto>> GetAdminPagedAsync(int pageNumber, int pageSize, string? search = null, string? provider = null, int? statutPaiement = null, string? sortByDateCreation = null, string? sortByDateConfirmation = null)
    {
        if (pageSize <= 0) pageSize = 12;
        if (pageSize > 50) pageSize = 50;
        int offset = (pageNumber - 1) * pageSize;

        var list = new List<AnnonceurPaymentDto>();
        int totalItems = 0;

        using var connection = _connectionFactory.CreateConnection();
        await ((SqlConnection)connection).OpenAsync();

        var searchLike = string.IsNullOrEmpty(search) ? null : $"%{search}%";
        var whereClause = @"
            (@Search IS NULL OR 
             u.Nom LIKE @Search OR 
             u.Prenom LIKE @Search OR 
             u.Email LIKE @Search)
            AND (@Provider IS NULL OR p.Provider = @Provider)
            AND (@StatutPaiement IS NULL OR p.StatutPaiement = @StatutPaiement)";

        string orderByClause = "p.DateCreation DESC"; // Default
        if (!string.IsNullOrWhiteSpace(sortByDateCreation))
        {
            string dir = sortByDateCreation.ToLower() == "asc" ? "ASC" : "DESC";
            orderByClause = $"p.DateCreation {dir}";
        }
        else if (!string.IsNullOrWhiteSpace(sortByDateConfirmation))
        {
            string dir = sortByDateConfirmation.ToLower() == "asc" ? "ASC" : "DESC";
            orderByClause = $"p.DateConfirmation {dir}";
        }

        // Get total count
        var countSql = $@"
            SELECT COUNT(1) 
            FROM PaiementsAnnonceur p
            JOIN Utilisateurs u ON p.IdUtilisateur = u.IdUtilisateur
            WHERE {whereClause}";

        using (var countCmd = new SqlCommand(countSql, (SqlConnection)connection))
        {
            countCmd.Parameters.AddWithValue("@Search", (object?)searchLike ?? DBNull.Value);
            countCmd.Parameters.AddWithValue("@Provider", (object?)provider ?? DBNull.Value);
            countCmd.Parameters.AddWithValue("@StatutPaiement", (object?)statutPaiement ?? DBNull.Value);
            totalItems = Convert.ToInt32(await countCmd.ExecuteScalarAsync());
        }

        // Get paged data
        var sql = $@"
            SELECT p.IdPaiementAnnonceur, p.IdUtilisateur, p.IdDemandeAnnonceur, p.Provider, 
                   p.ProviderPaymentId, p.DeveloperTrackingId, p.Montant, p.StatutPaiement, 
                   p.PaymentUrl, p.DateCreation, p.DateConfirmation,
                   u.Nom, u.Prenom, u.Email
            FROM PaiementsAnnonceur p
            JOIN Utilisateurs u ON p.IdUtilisateur = u.IdUtilisateur
            WHERE {whereClause}
            ORDER BY {orderByClause} 
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

        using (var command = new SqlCommand(sql, (SqlConnection)connection))
        {
            command.Parameters.AddWithValue("@Search", (object?)searchLike ?? DBNull.Value);
            command.Parameters.AddWithValue("@Provider", (object?)provider ?? DBNull.Value);
            command.Parameters.AddWithValue("@StatutPaiement", (object?)statutPaiement ?? DBNull.Value);
            command.Parameters.AddWithValue("@Offset", offset);
            command.Parameters.AddWithValue("@PageSize", pageSize);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var statusInt = reader.GetInt32(7);
                list.Add(new AnnonceurPaymentDto
                {
                    AnnonceurPaymentId = reader.GetInt64(0),
                    UserId = reader.GetInt64(1),
                    DemandeAnnonceurId = reader.GetInt64(2),
                    Provider = reader.GetString(3),
                    ProviderPaymentId = reader.IsDBNull(4) ? null : reader.GetString(4),
                    DeveloperTrackingId = reader.GetString(5),
                    Amount = reader.GetDecimal(6),
                    PaymentStatus = ((AnnonceurPaymentStatus)statusInt).ToString(),
                    PaymentUrl = reader.IsDBNull(8) ? null : reader.GetString(8),
                    CreatedAt = reader.GetDateTime(9),
                    ConfirmedAt = reader.IsDBNull(10) ? null : reader.GetDateTime(10),
                    NomUtilisateur = reader.GetString(11),
                    PrenomUtilisateur = reader.GetString(12),
                    EmailUtilisateur = reader.GetString(13)
                });
            }
        }

        return new PagedResponse<AnnonceurPaymentDto>(list, totalItems, pageNumber, pageSize);
    }

    public async Task UpdateProviderInfoAsync(long annonceurPaymentId, string? providerPaymentId, string? paymentUrl, string? rawResponseJson)
    {
        using var connection = _connectionFactory.CreateConnection();
        var command = new SqlCommand(@"
            UPDATE PaiementsAnnonceur 
            SET ProviderPaymentId = @ProviderPaymentId, 
                PaymentUrl = @PaymentUrl, 
                RawResponseJson = @RawResponseJson 
            WHERE IdPaiementAnnonceur = @IdPaiementAnnonceur", (SqlConnection)connection);
        
        command.Parameters.AddWithValue("@ProviderPaymentId", (object?)providerPaymentId ?? DBNull.Value);
        command.Parameters.AddWithValue("@PaymentUrl", (object?)paymentUrl ?? DBNull.Value);
        command.Parameters.AddWithValue("@RawResponseJson", (object?)rawResponseJson ?? DBNull.Value);
        command.Parameters.AddWithValue("@IdPaiementAnnonceur", annonceurPaymentId);

        await ((SqlConnection)connection).OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    public async Task UpdateStatusAsync(long annonceurPaymentId, AnnonceurPaymentStatus status, string? rawResponseJson = null, DateTime? confirmedAt = null)
    {
        using var connection = _connectionFactory.CreateConnection();
        var command = new SqlCommand(@"
            UPDATE PaiementsAnnonceur 
            SET StatutPaiement = @StatutPaiement, 
                RawResponseJson = COALESCE(@RawResponseJson, RawResponseJson), 
                DateConfirmation = COALESCE(@DateConfirmation, DateConfirmation) 
            WHERE IdPaiementAnnonceur = @IdPaiementAnnonceur", (SqlConnection)connection);
        
        command.Parameters.AddWithValue("@StatutPaiement", (int)status);
        command.Parameters.AddWithValue("@RawResponseJson", (object?)rawResponseJson ?? DBNull.Value);
        command.Parameters.AddWithValue("@DateConfirmation", (object?)confirmedAt ?? DBNull.Value);
        command.Parameters.AddWithValue("@IdPaiementAnnonceur", annonceurPaymentId);

        await ((SqlConnection)connection).OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    private AnnonceurPayment MapFromReader(SqlDataReader reader)
    {
        return new AnnonceurPayment
        {
            AnnonceurPaymentId = reader.GetInt64(0),
            UserId = reader.GetInt64(1),
            DemandeAnnonceurId = reader.GetInt64(2),
            Provider = reader.GetString(3),
            ProviderPaymentId = reader.IsDBNull(4) ? null : reader.GetString(4),
            DeveloperTrackingId = reader.GetString(5),
            Amount = reader.GetDecimal(6),
            PaymentStatus = (AnnonceurPaymentStatus)reader.GetInt32(7),
            PaymentUrl = reader.IsDBNull(8) ? null : reader.GetString(8),
            RawResponseJson = reader.IsDBNull(9) ? null : reader.GetString(9),
            CreatedAt = reader.GetDateTime(10),
            ConfirmedAt = reader.IsDBNull(11) ? null : reader.GetDateTime(11)
        };
    }

    private AnnonceurPaymentDto MapToDtoFromReader(SqlDataReader reader)
    {
        var statusInt = reader.GetInt32(7);
        return new AnnonceurPaymentDto
        {
            AnnonceurPaymentId = reader.GetInt64(0),
            UserId = reader.GetInt64(1),
            DemandeAnnonceurId = reader.GetInt64(2),
            Provider = reader.GetString(3),
            ProviderPaymentId = reader.IsDBNull(4) ? null : reader.GetString(4),
            DeveloperTrackingId = reader.GetString(5),
            Amount = reader.GetDecimal(6),
            PaymentStatus = ((AnnonceurPaymentStatus)statusInt).ToString(),
            PaymentUrl = reader.IsDBNull(8) ? null : reader.GetString(8),
            CreatedAt = reader.GetDateTime(9),
            ConfirmedAt = reader.IsDBNull(10) ? null : reader.GetDateTime(10)
        };
    }
    public async Task MarkAsPaidAndActivateAnnonceurAsync(long annonceurPaymentId, long adminId)
    {
        using var connection = (SqlConnection)_connectionFactory.CreateConnection();
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            // 1. Load payment
            var paymentCommand = new SqlCommand(@"
                SELECT IdPaiementAnnonceur, IdUtilisateur, IdDemandeAnnonceur, StatutPaiement 
                FROM PaiementsAnnonceur 
                WHERE IdPaiementAnnonceur = @IdPaiementAnnonceur", connection, transaction);
            paymentCommand.Parameters.AddWithValue("@IdPaiementAnnonceur", annonceurPaymentId);

            AnnonceurPayment? payment = null;
            using (var reader = await paymentCommand.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    payment = new AnnonceurPayment
                    {
                        AnnonceurPaymentId = reader.GetInt64(0),
                        UserId = reader.GetInt64(1),
                        DemandeAnnonceurId = reader.GetInt64(2),
                        PaymentStatus = (AnnonceurPaymentStatus)reader.GetInt32(3)
                    };
                }
            }

            if (payment == null)
                throw new NotFoundException("Annonceur payment not found.");

            // 2. Check payment status
            if (payment.PaymentStatus == AnnonceurPaymentStatus.Paid)
            {
                transaction.Commit();
                return;
            }

            if (payment.PaymentStatus != AnnonceurPaymentStatus.Pending)
                throw new BadRequestException("Only pending annonceur payments can be marked as paid.");

            // 3. Load linked DemandeAnnonceur
            var demandeCommand = new SqlCommand(@"
                SELECT IdDemandeAnnonceur, IdUtilisateur, Statut, IdAdminTraitant 
                FROM DemandesAnnonceur 
                WHERE IdDemandeAnnonceur = @IdDemandeAnnonceur", connection, transaction);
            demandeCommand.Parameters.AddWithValue("@IdDemandeAnnonceur", payment.DemandeAnnonceurId);

            long demandeId = 0, demandeUserId = 0;
            int demandeStatus = 0;
            bool demandeExists = false;
            long? existingAdminId = null;

            using (var reader = await demandeCommand.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    demandeExists = true;
                    demandeId = reader.GetInt64(0);
                    demandeUserId = reader.GetInt64(1);
                    demandeStatus = reader.GetInt32(2);
                    existingAdminId = reader.IsDBNull(3) ? null : (long?)reader.GetInt64(3);
                }
            }

            if (!demandeExists)
                throw new NotFoundException("Advertiser request not found.");

            if (adminId == 0)
            {
                adminId = existingAdminId ?? 1; // Fallback to seeded admin ID
            }

            // 4. Validate ownership consistency
            if (payment.UserId != demandeUserId)
                throw new BadRequestException("Payment user does not match advertiser request user.");

            // 5. Validate demande status
            if (demandeStatus != (int)StatutDemandeAnnonceur.EN_ATTENTE_PAIEMENT)
                throw new BadRequestException("Advertiser request is not waiting for payment.");

            // 6. Update dbo.PaiementsAnnonceur
            var updatePaymentCmd = new SqlCommand(@"
                UPDATE PaiementsAnnonceur 
                SET StatutPaiement = 2, 
                    DateConfirmation = SYSUTCDATETIME(), 
                    RawResponseJson = @RawResponse 
                WHERE IdPaiementAnnonceur = @IdPaiementAnnonceur", connection, transaction);
            
            updatePaymentCmd.Parameters.AddWithValue("@RawResponse", "{\"mock\":true,\"status\":\"SUCCESS\"}");
            updatePaymentCmd.Parameters.AddWithValue("@IdPaiementAnnonceur", annonceurPaymentId);
            await updatePaymentCmd.ExecuteNonQueryAsync();

            // 7. Update dbo.DemandesAnnonceur
            var updateDemandeCmd = new SqlCommand(@"
                UPDATE DemandesAnnonceur 
                SET Statut = 2, 
                    DateTraitement = SYSUTCDATETIME(), 
                    IdAdminTraitant = @AdminId 
                WHERE IdDemandeAnnonceur = @IdDemandeAnnonceur", connection, transaction);
            
            updateDemandeCmd.Parameters.AddWithValue("@AdminId", adminId);
            updateDemandeCmd.Parameters.AddWithValue("@IdDemandeAnnonceur", payment.DemandeAnnonceurId);
            await updateDemandeCmd.ExecuteNonQueryAsync();

            // 8. Update dbo.Utilisateurs
            var updateUserCmd = new SqlCommand(@"
                UPDATE Utilisateurs 
                SET Role = 2 
                WHERE IdUtilisateur = @IdUtilisateur", connection, transaction);
            
            updateUserCmd.Parameters.AddWithValue("@IdUtilisateur", payment.UserId);
            await updateUserCmd.ExecuteNonQueryAsync();

            // 9. Commit transaction
            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}
