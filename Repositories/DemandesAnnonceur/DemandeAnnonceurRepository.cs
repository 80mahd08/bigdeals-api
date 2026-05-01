using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using api.Data.Connections;
using api.Interfaces.DemandesAnnonceur;
using api.Models;
using api.Models.Enums;

namespace api.Repositories.DemandesAnnonceur;

public class DemandeAnnonceurRepository : IDemandeAnnonceurRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public DemandeAnnonceurRepository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<long> CreateAsync(DemandeAnnonceur demande)
    {
        using var connection = _connectionFactory.CreateConnection();
        var command = new SqlCommand(@"
            INSERT INTO DemandesAnnonceur (IdUtilisateur, Statut, DocumentUrl, DocumentNomOriginal, DocumentType, DocumentTaille, DateDemande)
            OUTPUT INSERTED.IdDemandeAnnonceur
            VALUES (@IdUtilisateur, @Statut, @DocumentUrl, @DocumentNomOriginal, @DocumentType, @DocumentTaille, @DateDemande)", 
            (SqlConnection)connection);

        command.Parameters.AddWithValue("@IdUtilisateur", demande.IdUtilisateur);
        command.Parameters.AddWithValue("@Statut", (int)demande.Statut);
        command.Parameters.AddWithValue("@DocumentUrl", demande.DocumentUrl);
        command.Parameters.AddWithValue("@DocumentNomOriginal", demande.DocumentNomOriginal);
        command.Parameters.AddWithValue("@DocumentType", demande.DocumentType);
        command.Parameters.AddWithValue("@DocumentTaille", demande.DocumentTaille);
        command.Parameters.AddWithValue("@DateDemande", demande.DateDemande);

        await ((SqlConnection)connection).OpenAsync();
        return (long)await command.ExecuteScalarAsync();
    }

    public async Task<bool> HasPendingRequestAsync(long idUtilisateur)
    {
        using var connection = _connectionFactory.CreateConnection();
        var command = new SqlCommand("SELECT COUNT(1) FROM DemandesAnnonceur WHERE IdUtilisateur = @IdUtilisateur AND Statut = 1", (SqlConnection)connection);
        command.Parameters.AddWithValue("@IdUtilisateur", idUtilisateur);

        await ((SqlConnection)connection).OpenAsync();
        var count = (int)await command.ExecuteScalarAsync();
        return count > 0;
    }

    public async Task<IReadOnlyList<DemandeAnnonceur>> GetByUserIdAsync(long idUtilisateur)
    {
        var list = new List<DemandeAnnonceur>();
        using var connection = _connectionFactory.CreateConnection();
        var command = new SqlCommand("SELECT IdDemandeAnnonceur, IdUtilisateur, Statut, DocumentUrl, DocumentNomOriginal, DocumentType, DocumentTaille, MotifRejet, DateDemande, DateTraitement, IdAdminTraitant FROM DemandesAnnonceur WHERE IdUtilisateur = @IdUtilisateur ORDER BY DateDemande DESC", (SqlConnection)connection);
        command.Parameters.AddWithValue("@IdUtilisateur", idUtilisateur);

        await ((SqlConnection)connection).OpenAsync();
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(MapFromReader(reader));
        }

        return list;
    }

    public async Task<IReadOnlyList<DemandeAnnonceur>> GetAllAsync()
    {
        var list = new List<DemandeAnnonceur>();
        using var connection = _connectionFactory.CreateConnection();
        var command = new SqlCommand("SELECT IdDemandeAnnonceur, IdUtilisateur, Statut, DocumentUrl, DocumentNomOriginal, DocumentType, DocumentTaille, MotifRejet, DateDemande, DateTraitement, IdAdminTraitant FROM DemandesAnnonceur ORDER BY DateDemande DESC", (SqlConnection)connection);

        await ((SqlConnection)connection).OpenAsync();
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(MapFromReader(reader));
        }

        return list;
    }

    public async Task<DemandeAnnonceur?> GetByIdAsync(long idDemandeAnnonceur)
    {
        using var connection = _connectionFactory.CreateConnection();
        var command = new SqlCommand("SELECT IdDemandeAnnonceur, IdUtilisateur, Statut, DocumentUrl, DocumentNomOriginal, DocumentType, DocumentTaille, MotifRejet, DateDemande, DateTraitement, IdAdminTraitant FROM DemandesAnnonceur WHERE IdDemandeAnnonceur = @IdDemandeAnnonceur", (SqlConnection)connection);
        command.Parameters.AddWithValue("@IdDemandeAnnonceur", idDemandeAnnonceur);

        await ((SqlConnection)connection).OpenAsync();
        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapFromReader(reader);
        }

        return null;
    }

    public async Task<bool> UpdateStatusToApprovedAsync(long idDemandeAnnonceur, long idAdminTraitant, long idUtilisateur)
    {
        using var connection = _connectionFactory.CreateConnection();
        await ((SqlConnection)connection).OpenAsync();
        using var transaction = ((SqlConnection)connection).BeginTransaction();

        try
        {
            var updateDemandeCmd = new SqlCommand(@"
                UPDATE DemandesAnnonceur 
                SET Statut = 2, 
                    IdAdminTraitant = @IdAdminTraitant, 
                    DateTraitement = @DateTraitement 
                WHERE IdDemandeAnnonceur = @IdDemandeAnnonceur", 
                (SqlConnection)connection, transaction);

            updateDemandeCmd.Parameters.AddWithValue("@IdAdminTraitant", idAdminTraitant);
            updateDemandeCmd.Parameters.AddWithValue("@DateTraitement", DateTime.UtcNow);
            updateDemandeCmd.Parameters.AddWithValue("@IdDemandeAnnonceur", idDemandeAnnonceur);

            int rowsDemande = await updateDemandeCmd.ExecuteNonQueryAsync();

            var updateUserCmd = new SqlCommand(@"
                UPDATE Utilisateurs 
                SET Role = 2 
                WHERE IdUtilisateur = @IdUtilisateur", 
                (SqlConnection)connection, transaction);
            
            updateUserCmd.Parameters.AddWithValue("@IdUtilisateur", idUtilisateur);
            
            int rowsUser = await updateUserCmd.ExecuteNonQueryAsync();

            if (rowsDemande > 0 && rowsUser > 0)
            {
                transaction.Commit();
                return true;
            }

            transaction.Rollback();
            return false;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<bool> UpdateStatusToRejectedAsync(long idDemandeAnnonceur, long idAdminTraitant, string motifRejet)
    {
        using var connection = _connectionFactory.CreateConnection();
        var command = new SqlCommand(@"
            UPDATE DemandesAnnonceur 
            SET Statut = 3, 
                MotifRejet = @MotifRejet, 
                IdAdminTraitant = @IdAdminTraitant, 
                DateTraitement = @DateTraitement 
            WHERE IdDemandeAnnonceur = @IdDemandeAnnonceur", 
            (SqlConnection)connection);

        command.Parameters.AddWithValue("@MotifRejet", motifRejet);
        command.Parameters.AddWithValue("@IdAdminTraitant", idAdminTraitant);
        command.Parameters.AddWithValue("@DateTraitement", DateTime.UtcNow);
        command.Parameters.AddWithValue("@IdDemandeAnnonceur", idDemandeAnnonceur);

        await ((SqlConnection)connection).OpenAsync();
        int rows = await command.ExecuteNonQueryAsync();
        return rows > 0;
    }

    private DemandeAnnonceur MapFromReader(SqlDataReader reader)
    {
        return new DemandeAnnonceur
        {
            IdDemandeAnnonceur = reader.GetInt64(0),
            IdUtilisateur = reader.GetInt64(1),
            Statut = (StatutDemandeAnnonceur)reader.GetInt32(2),
            DocumentUrl = reader.GetString(3),
            DocumentNomOriginal = reader.GetString(4),
            DocumentType = reader.GetString(5),
            DocumentTaille = reader.GetInt64(6),
            MotifRejet = reader.IsDBNull(7) ? null : reader.GetString(7),
            DateDemande = reader.GetDateTime(8),
            DateTraitement = reader.IsDBNull(9) ? null : reader.GetDateTime(9),
            IdAdminTraitant = reader.IsDBNull(10) ? null : reader.GetInt64(10)
        };
    }
}
