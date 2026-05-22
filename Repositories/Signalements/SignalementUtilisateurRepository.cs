using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using api.Common;
using api.Dtos.SignalementsUtilisateurs;
using api.Interfaces.Signalements;
using api.Models;
using api.Models.Enums;

namespace api.Repositories.Signalements;

public class SignalementUtilisateurRepository : ISignalementUtilisateurRepository
{
    private readonly string _connectionString;

    public SignalementUtilisateurRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    private SqlConnection CreateConnection() => new SqlConnection(_connectionString);

    public async Task<long> CreateAsync(SignalementUtilisateur signalement)
    {
        var query = @"
            INSERT INTO dbo.SignalementsUtilisateurs 
            (IdUtilisateurSignale, IdUtilisateurReporter, TypeSignalement, Motif, Statut, DateCreation)
            OUTPUT INSERTED.IdSignalement
            VALUES 
            (@IdUtilisateurSignale, @IdUtilisateurReporter, @TypeSignalement, @Motif, @Statut, @DateCreation);";

        using var connection = CreateConnection();
        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@IdUtilisateurSignale", signalement.IdUtilisateurSignale);
        command.Parameters.AddWithValue("@IdUtilisateurReporter", signalement.IdUtilisateurReporter);
        command.Parameters.AddWithValue("@TypeSignalement", (int)signalement.TypeSignalement);
        command.Parameters.AddWithValue("@Motif", signalement.Motif);
        command.Parameters.AddWithValue("@Statut", (int)signalement.Statut);
        command.Parameters.AddWithValue("@DateCreation", signalement.DateCreation);

        await connection.OpenAsync();
        return Convert.ToInt64(await command.ExecuteScalarAsync());
    }

    public async Task<bool> ExistsAsync(long idSignalement)
    {
        var query = "SELECT 1 FROM dbo.SignalementsUtilisateurs WHERE IdSignalement = @Id";
        using var connection = CreateConnection();
        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@Id", idSignalement);

        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();
        return result != null;
    }

    public async Task<bool> HasUserAlreadyReportedAsync(long idUtilisateurSignale, long idUtilisateurReporter)
    {
        var query = @"
            SELECT 1 
            FROM dbo.SignalementsUtilisateurs 
            WHERE IdUtilisateurSignale = @IdUtilisateurSignale 
              AND IdUtilisateurReporter = @IdUtilisateurReporter 
              AND Statut = @StatutEnAttente";

        using var connection = CreateConnection();
        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@IdUtilisateurSignale", idUtilisateurSignale);
        command.Parameters.AddWithValue("@IdUtilisateurReporter", idUtilisateurReporter);
        command.Parameters.AddWithValue("@StatutEnAttente", (int)StatutSignalement.EN_ATTENTE);

        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();
        return result != null;
    }

    public async Task<long?> GetReportedUserIdAsync(long idSignalement)
    {
        var query = "SELECT IdUtilisateurSignale FROM dbo.SignalementsUtilisateurs WHERE IdSignalement = @Id";
        using var connection = CreateConnection();
        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@Id", idSignalement);

        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();
        return result as long?;
    }

    public async Task UpdateStatusAsync(long idSignalement, int statut, long adminId)
    {
        var query = @"
            UPDATE dbo.SignalementsUtilisateurs
            SET Statut = @Statut,
                DateTraitement = SYSUTCDATETIME(),
                IdAdminTraitant = @AdminId
            WHERE IdSignalement = @Id";

        using var connection = CreateConnection();
        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@Id", idSignalement);
        command.Parameters.AddWithValue("@Statut", statut);
        command.Parameters.AddWithValue("@AdminId", adminId);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    public async Task<PagedResponse<SignalementUtilisateurDto>> GetPagedAdminAsync(int pageNumber, int pageSize, int? statut, string? search, string? sortByDate = null, int? raison = null)
    {
        using var connection = CreateConnection();
        await connection.OpenAsync();

        var whereConditions = new List<string> { "1 = 1" };
        var commandParameters = new List<SqlParameter>();

        if (statut.HasValue)
        {
            whereConditions.Add("su.Statut = @Statut");
            commandParameters.Add(new SqlParameter("@Statut", statut.Value));
        }

        if (raison.HasValue)
        {
            whereConditions.Add("su.TypeSignalement = @TypeSignalement");
            commandParameters.Add(new SqlParameter("@TypeSignalement", raison.Value));
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            whereConditions.Add("(uSignale.Nom LIKE @Search OR uSignale.Prenom LIKE @Search OR uSignale.Email LIKE @Search OR uReporter.Email LIKE @Search OR su.Motif LIKE @Search)");
            commandParameters.Add(new SqlParameter("@Search", $"%{search}%"));
        }

        var whereClause = string.Join(" AND ", whereConditions);

        var countQuery = $@"
            SELECT COUNT(1)
            FROM dbo.SignalementsUtilisateurs su
            INNER JOIN dbo.Utilisateurs uSignale ON su.IdUtilisateurSignale = uSignale.IdUtilisateur
            INNER JOIN dbo.Utilisateurs uReporter ON su.IdUtilisateurReporter = uReporter.IdUtilisateur
            WHERE {whereClause}";

        using var countCmd = new SqlCommand(countQuery, connection);
        foreach (var p in commandParameters)
        {
            countCmd.Parameters.Add(new SqlParameter(p.ParameterName, p.Value));
        }
        var totalRecords = Convert.ToInt32(await countCmd.ExecuteScalarAsync());

        string orderBy = "su.Statut ASC, su.DateCreation DESC";
        if (!string.IsNullOrWhiteSpace(sortByDate))
        {
            if (sortByDate.ToLower() == "asc")
                orderBy = "su.DateCreation ASC";
            else if (sortByDate.ToLower() == "desc")
                orderBy = "su.DateCreation DESC";
        }

        var dataQuery = $@"
            SELECT 
                su.IdSignalement,
                su.IdUtilisateurSignale,
                uSignale.Prenom + ' ' + uSignale.Nom AS SignaleNomComplet,
                uSignale.Email AS SignaleEmail,
                uSignale.Telephone AS SignaleTelephone,
                su.IdUtilisateurReporter,
                uReporter.Prenom + ' ' + uReporter.Nom AS ReporterNomComplet,
                uReporter.Email AS ReporterEmail,
                su.TypeSignalement,
                CASE su.TypeSignalement 
                    WHEN 1 THEN 'Contenu inapproprié'
                    WHEN 2 THEN 'Fraude'
                    WHEN 3 THEN 'Harcèlement'
                    WHEN 4 THEN 'Autre'
                    ELSE 'Inconnu'
                END AS TypeSignalementLabel,
                su.Motif,
                su.Statut,
                CASE su.Statut
                    WHEN 1 THEN 'En attente'
                    WHEN 2 THEN 'Traité'
                    WHEN 3 THEN 'Rejeté'
                    ELSE 'Inconnu'
                END AS StatutLabel,
                su.DateCreation,
                su.DateTraitement,
                su.IdAdminTraitant
            FROM dbo.SignalementsUtilisateurs su
            INNER JOIN dbo.Utilisateurs uSignale ON su.IdUtilisateurSignale = uSignale.IdUtilisateur
            INNER JOIN dbo.Utilisateurs uReporter ON su.IdUtilisateurReporter = uReporter.IdUtilisateur
            WHERE {whereClause}
            ORDER BY {orderBy}
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

        using var dataCmd = new SqlCommand(dataQuery, connection);
        foreach (var p in commandParameters)
        {
            dataCmd.Parameters.Add(new SqlParameter(p.ParameterName, p.Value));
        }
        dataCmd.Parameters.AddWithValue("@Offset", (pageNumber - 1) * pageSize);
        dataCmd.Parameters.AddWithValue("@PageSize", pageSize);

        var items = new List<SignalementUtilisateurDto>();
        using var reader = await dataCmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            items.Add(new SignalementUtilisateurDto
            {
                IdSignalement = reader.GetInt64(reader.GetOrdinal("IdSignalement")),
                IdUtilisateurSignale = reader.GetInt64(reader.GetOrdinal("IdUtilisateurSignale")),
                SignaleNomComplet = reader.GetString(reader.GetOrdinal("SignaleNomComplet")),
                SignaleEmail = reader.GetString(reader.GetOrdinal("SignaleEmail")),
                SignaleTelephone = reader.IsDBNull(reader.GetOrdinal("SignaleTelephone")) ? string.Empty : reader.GetString(reader.GetOrdinal("SignaleTelephone")),
                IdUtilisateurReporter = reader.GetInt64(reader.GetOrdinal("IdUtilisateurReporter")),
                ReporterNomComplet = reader.GetString(reader.GetOrdinal("ReporterNomComplet")),
                ReporterEmail = reader.GetString(reader.GetOrdinal("ReporterEmail")),
                TypeSignalement = reader.GetInt32(reader.GetOrdinal("TypeSignalement")),
                TypeSignalementLabel = reader.GetString(reader.GetOrdinal("TypeSignalementLabel")),
                Motif = reader.GetString(reader.GetOrdinal("Motif")),
                Statut = reader.GetInt32(reader.GetOrdinal("Statut")),
                StatutLabel = reader.GetString(reader.GetOrdinal("StatutLabel")),
                DateCreation = reader.GetDateTime(reader.GetOrdinal("DateCreation")),
                DateTraitement = reader.IsDBNull(reader.GetOrdinal("DateTraitement")) ? null : reader.GetDateTime(reader.GetOrdinal("DateTraitement")),
                IdAdminTraitant = reader.IsDBNull(reader.GetOrdinal("IdAdminTraitant")) ? null : reader.GetInt64(reader.GetOrdinal("IdAdminTraitant"))
            });
        }

        return new PagedResponse<SignalementUtilisateurDto>(items, totalRecords, pageNumber, pageSize);
    }
}
