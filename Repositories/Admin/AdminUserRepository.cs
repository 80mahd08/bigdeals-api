using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using api.Common;
using api.Data.Connections;
using api.Dtos.Admin;
using api.Interfaces.Admin;

namespace api.Repositories.Admin;

public class AdminUserRepository : IAdminUserRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public AdminUserRepository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResponse<AdminUserListItemDto>> GetPagedUsersAsync(int pageNumber, int pageSize, string? search, int? statutCompte, int? role, string? ville, string? sortByDateInscription = null, string? sortByNbAnnonces = null)
    {
        using var connection = _connectionFactory.CreateConnection();
        await ((SqlConnection)connection).OpenAsync();

        var offset = (pageNumber - 1) * pageSize;
        var searchLike = string.IsNullOrEmpty(search) ? null : $"%{search}%";

        var whereClause = @"
            u.Role IN (1, 2)
            AND (@Search IS NULL OR
                 u.Nom LIKE @Search OR
                 u.Prenom LIKE @Search OR
                 u.Email LIKE @Search OR
                 u.Telephone LIKE @Search)
            AND (@StatutCompte IS NULL OR u.StatutCompte = @StatutCompte)
            AND (@Role IS NULL OR u.Role = @Role)
            AND (@Ville IS NULL OR u.Ville = @Ville)";

        string orderByClause = "u.DateCreation DESC"; // Default
        if (!string.IsNullOrWhiteSpace(sortByDateInscription))
        {
            string dir = sortByDateInscription.ToLower() == "asc" ? "ASC" : "DESC";
            orderByClause = $"u.DateCreation {dir}";
        }
        else if (!string.IsNullOrWhiteSpace(sortByNbAnnonces))
        {
            string dir = sortByNbAnnonces.ToLower() == "asc" ? "ASC" : "DESC";
            orderByClause = $"COUNT(a.IdAnnonce) {dir}";
        }

        var countSql = $"SELECT COUNT(*) FROM dbo.Utilisateurs u WHERE {whereClause}";
        var dataSql = $@"
            SELECT
                u.IdUtilisateur,
                u.Nom,
                u.Prenom,
                u.Email,
                u.Telephone,
                u.Ville,
                u.Role,
                u.StatutCompte,
                u.DateCreation,
                u.PhotoProfilUrl,
                COUNT(a.IdAnnonce) AS NombreAnnonces
            FROM dbo.Utilisateurs u
            LEFT JOIN dbo.Annonces a ON a.IdUtilisateur = u.IdUtilisateur
            WHERE {whereClause}
            GROUP BY
                u.IdUtilisateur, u.Nom, u.Prenom, u.Email, u.Telephone, u.Ville, u.Role, u.StatutCompte, u.DateCreation, u.PhotoProfilUrl
            ORDER BY {orderByClause}
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

        using var countCmd = new SqlCommand(countSql, (SqlConnection)connection);
        countCmd.Parameters.AddWithValue("@Search", (object?)searchLike ?? DBNull.Value);
        countCmd.Parameters.AddWithValue("@StatutCompte", (object?)statutCompte ?? DBNull.Value);
        countCmd.Parameters.AddWithValue("@Role", (object?)role ?? DBNull.Value);
        countCmd.Parameters.AddWithValue("@Ville", (object?)ville ?? DBNull.Value);
        var totalCount = Convert.ToInt32(await countCmd.ExecuteScalarAsync());

        var items = new List<AdminUserListItemDto>();
        using var dataCmd = new SqlCommand(dataSql, (SqlConnection)connection);
        dataCmd.Parameters.AddWithValue("@Search", (object?)searchLike ?? DBNull.Value);
        dataCmd.Parameters.AddWithValue("@StatutCompte", (object?)statutCompte ?? DBNull.Value);
        dataCmd.Parameters.AddWithValue("@Role", (object?)role ?? DBNull.Value);
        dataCmd.Parameters.AddWithValue("@Ville", (object?)ville ?? DBNull.Value);
        dataCmd.Parameters.AddWithValue("@Offset", offset);
        dataCmd.Parameters.AddWithValue("@PageSize", pageSize);

        using var reader = await dataCmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            items.Add(new AdminUserListItemDto
            {
                IdUtilisateur = reader.GetInt64(reader.GetOrdinal("IdUtilisateur")),
                Nom = reader.GetString(reader.GetOrdinal("Nom")),
                Prenom = reader.GetString(reader.GetOrdinal("Prenom")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                Telephone = reader.IsDBNull(reader.GetOrdinal("Telephone")) ? null : reader.GetString(reader.GetOrdinal("Telephone")),
                Ville = reader.IsDBNull(reader.GetOrdinal("Ville")) ? null : reader.GetString(reader.GetOrdinal("Ville")),
                Role = reader.GetInt32(reader.GetOrdinal("Role")),
                StatutCompte = reader.GetInt32(reader.GetOrdinal("StatutCompte")),
                DateCreation = reader.GetDateTime(reader.GetOrdinal("DateCreation")),
                NombreAnnonces = reader.GetInt32(reader.GetOrdinal("NombreAnnonces")),
                PhotoProfilUrl = reader.IsDBNull(reader.GetOrdinal("PhotoProfilUrl")) ? null : reader.GetString(reader.GetOrdinal("PhotoProfilUrl"))
            });
        }

        return new PagedResponse<AdminUserListItemDto>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<bool> UpdateUserStatusAsync(long idUtilisateur, int newStatus)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = @"
            UPDATE dbo.Utilisateurs
            SET StatutCompte = @StatutCompte
            WHERE IdUtilisateur = @IdUtilisateur
            AND Role IN (1, 2)";

        using var cmd = new SqlCommand(sql, (SqlConnection)connection);
        cmd.Parameters.AddWithValue("@StatutCompte", newStatus);
        cmd.Parameters.AddWithValue("@IdUtilisateur", idUtilisateur);

        await ((SqlConnection)connection).OpenAsync();
        var affected = await cmd.ExecuteNonQueryAsync();
        return affected > 0;
    }

    public async Task<bool> IsAdminAsync(long idUtilisateur)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = "SELECT Role FROM dbo.Utilisateurs WHERE IdUtilisateur = @Id";
        using var cmd = new SqlCommand(sql, (SqlConnection)connection);
        cmd.Parameters.AddWithValue("@Id", idUtilisateur);

        await ((SqlConnection)connection).OpenAsync();
        var role = await cmd.ExecuteScalarAsync();
        return role != null && Convert.ToInt32(role) == 3;
    }
}
