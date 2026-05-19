using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using api.Common;
using api.Data.Connections;
using api.Dtos.Signalements;
using api.Interfaces.Signalements;
using api.Models;
using api.Models.Enums;

namespace api.Repositories.Signalements;

public class SignalementRepository : ISignalementRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public SignalementRepository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<bool> AnnonceExistsAndIsPublicAsync(long idAnnonce)
    {
        using var connection = (SqlConnection)_connectionFactory.CreateConnection();
        const string sql = "SELECT COUNT(1) FROM Annonces WHERE IdAnnonce = @IdAnnonce AND Statut = 1 AND EstActive = 1";
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@IdAnnonce", idAnnonce);
        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();
        return result != null && result != DBNull.Value && Convert.ToInt32(result) > 0;
    }

    public async Task<long?> GetAnnonceOwnerIdAsync(long idAnnonce)
    {
        using var connection = (SqlConnection)_connectionFactory.CreateConnection();
        const string sql = "SELECT IdUtilisateur FROM Annonces WHERE IdAnnonce = @IdAnnonce";
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@IdAnnonce", idAnnonce);
        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();
        return result == null || result == DBNull.Value ? null : (long?)Convert.ToInt64(result);
    }

    public async Task<bool> HasUserAlreadyReportedAsync(long idAnnonce, long idUtilisateur)
    {
        using var connection = (SqlConnection)_connectionFactory.CreateConnection();
        const string sql = "SELECT COUNT(1) FROM Signalements WHERE IdAnnonce = @IdAnnonce AND IdUtilisateur = @IdUtilisateur";
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@IdAnnonce", idAnnonce);
        command.Parameters.AddWithValue("@IdUtilisateur", idUtilisateur);
        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();
        return result != null && result != DBNull.Value && Convert.ToInt32(result) > 0;
    }

    public async Task<int?> GetAnnonceStatusAsync(long idAnnonce)
    {
        using var connection = (SqlConnection)_connectionFactory.CreateConnection();
        const string sql = "SELECT Statut FROM Annonces WHERE IdAnnonce = @IdAnnonce";
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@IdAnnonce", idAnnonce);
        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();
        return result == null || result == DBNull.Value ? null : (int?)Convert.ToInt32(result);
    }

    public async Task<long> CreateAsync(Signalement signalement)
    {
        using var connection = (SqlConnection)_connectionFactory.CreateConnection();
        const string sql = @"
            INSERT INTO Signalements (IdAnnonce, IdUtilisateur, TypeSignalement, Motif, Statut, DateCreation)
            OUTPUT INSERTED.IdSignalement
            VALUES (@IdAnnonce, @IdUtilisateur, @TypeSignalement, @Motif, @Statut, @DateCreation)";
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@IdAnnonce", signalement.IdAnnonce);
        command.Parameters.AddWithValue("@IdUtilisateur", signalement.IdUtilisateur);
        command.Parameters.AddWithValue("@TypeSignalement", (int)signalement.TypeSignalement);
        command.Parameters.AddWithValue("@Motif", signalement.Motif);
        command.Parameters.AddWithValue("@Statut", (int)signalement.Statut);
        command.Parameters.AddWithValue("@DateCreation", signalement.DateCreation);
        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();
        return result != null && result != DBNull.Value ? Convert.ToInt64(result) : 0;
    }

    public async Task<PagedResponse<SignalementDto>> GetPagedAdminAsync(int pageNumber, int pageSize, int? statut, string? search)
    {
        using var connection = (SqlConnection)_connectionFactory.CreateConnection();
        await connection.OpenAsync();

        string whereClause = "WHERE 1=1";
        var parameters = new List<SqlParameter>();

        if (statut.HasValue)
        {
            whereClause += " AND s.Statut = @Statut";
            parameters.Add(new SqlParameter("@Statut", statut.Value));
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            whereClause += " AND (a.Titre LIKE @Search OR u.Email LIKE @Search OR u.Nom LIKE @Search OR u.Prenom LIKE @Search OR s.Motif LIKE @Search)";
            parameters.Add(new SqlParameter("@Search", $"%{search}%"));
        }

        string countSql = $@"
            SELECT COUNT(*) 
            FROM Signalements s
            JOIN Annonces a ON a.IdAnnonce = s.IdAnnonce
            JOIN Utilisateurs u ON u.IdUtilisateur = s.IdUtilisateur
            {whereClause}";

        using var countCommand = new SqlCommand(countSql, connection);
        foreach (var p in parameters) countCommand.Parameters.Add(new SqlParameter(p.ParameterName, p.Value));
        int totalCount = Convert.ToInt32(await countCommand.ExecuteScalarAsync());

        string dataSql = $@"
            SELECT s.*, 
                   a.Titre as TitreAnnonce, a.Description as DescriptionAnnonce, 
                   c.Nom as CategorieAnnonce, 
                   u.Nom as ReporterNom, u.Prenom as ReporterPrenom, u.Email as ReporterEmail,
                   ua.Nom as AnnonceurNom, ua.Prenom as AnnonceurPrenom, ua.Telephone as AnnonceurTelephone
            FROM Signalements s
            JOIN Annonces a ON a.IdAnnonce = s.IdAnnonce
            JOIN Categories c ON a.IdCategorie = c.IdCategorie
            JOIN Utilisateurs u ON u.IdUtilisateur = s.IdUtilisateur
            JOIN Utilisateurs ua ON ua.IdUtilisateur = a.IdUtilisateur
            {whereClause}
            ORDER BY s.DateCreation DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

        using var dataCommand = new SqlCommand(dataSql, connection);
        foreach (var p in parameters) dataCommand.Parameters.Add(new SqlParameter(p.ParameterName, p.Value));
        dataCommand.Parameters.AddWithValue("@Offset", (pageNumber - 1) * pageSize);
        dataCommand.Parameters.AddWithValue("@PageSize", pageSize);

        var items = new List<SignalementDto>();
        using (var reader = await dataCommand.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                items.Add(new SignalementDto
                {
                    IdSignalement = Convert.ToInt64(reader["IdSignalement"]),
                    IdAnnonce = Convert.ToInt64(reader["IdAnnonce"]),
                    TitreAnnonce = reader["TitreAnnonce"].ToString() ?? "",
                    DescriptionAnnonce = reader["DescriptionAnnonce"].ToString() ?? "",
                    CategorieAnnonce = reader["CategorieAnnonce"].ToString() ?? "",
                    AnnonceurNom = $"{reader["AnnonceurPrenom"]} {reader["AnnonceurNom"]}",
                    AnnonceurTelephone = reader["AnnonceurTelephone"].ToString() ?? "",
                    IdUtilisateur = Convert.ToInt64(reader["IdUtilisateur"]),
                    ReporterNomComplet = $"{reader["ReporterPrenom"]} {reader["ReporterNom"]}",
                    ReporterEmail = reader["ReporterEmail"].ToString() ?? "",
                    TypeSignalement = Convert.ToInt32(reader["TypeSignalement"]),
                    TypeSignalementLabel = GetTypeLabel(Convert.ToInt32(reader["TypeSignalement"])),
                    Motif = reader["Motif"].ToString() ?? "",
                    Statut = Convert.ToInt32(reader["Statut"]),
                    StatutLabel = GetStatusLabel(Convert.ToInt32(reader["Statut"])),
                    DateCreation = Convert.ToDateTime(reader["DateCreation"]),
                    DateTraitement = reader["DateTraitement"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["DateTraitement"]),
                    IdAdminTraitant = reader["IdAdminTraitant"] == DBNull.Value ? null : (long?)Convert.ToInt64(reader["IdAdminTraitant"])
                });
            }
        }

        // Fetch details for each item (Images and Attributes)
        foreach (var item in items)
        {
            // 1. Fetch Images
            const string imgSql = "SELECT Url, EstPrincipale FROM ImagesAnnonce WHERE IdAnnonce = @IdAnnonce ORDER BY EstPrincipale DESC, OrdreAffichage ASC";
            using (var imgCmd = new SqlCommand(imgSql, connection))
            {
                imgCmd.Parameters.AddWithValue("@IdAnnonce", item.IdAnnonce);
                using var imgReader = await imgCmd.ExecuteReaderAsync();
                while (await imgReader.ReadAsync())
                {
                    item.Images.Add(new api.Dtos.Annonces.ImageAnnonceDto
                    {
                        Url = imgReader["Url"].ToString() ?? "",
                        EstPrincipale = Convert.ToBoolean(imgReader["EstPrincipale"])
                    });
                }
            }

            // 2. Fetch Attributes
            const string attrSql = @"
                SELECT v.*, a.Nom as AttributNom, opt.Valeur as OptionValeur
                FROM ValeursAttributAnnonce v
                JOIN AttributsCategorie a ON v.IdAttributCategorie = a.IdAttributCategorie
                LEFT JOIN OptionsAttributCategorie opt ON v.IdOptionAttributCategorie = opt.IdOptionAttributCategorie
                WHERE v.IdAnnonce = @IdAnnonce";
            using (var attrCmd = new SqlCommand(attrSql, connection))
            {
                attrCmd.Parameters.AddWithValue("@IdAnnonce", item.IdAnnonce);
                using var attrReader = await attrCmd.ExecuteReaderAsync();
                while (await attrReader.ReadAsync())
                {
                    string valStr = "";
                    if (attrReader["ValeurTexte"] != DBNull.Value) valStr = attrReader["ValeurTexte"].ToString()!;
                    else if (attrReader["ValeurNombre"] != DBNull.Value) valStr = attrReader["ValeurNombre"].ToString()!;
                    else if (attrReader["ValeurDate"] != DBNull.Value) valStr = Convert.ToDateTime(attrReader["ValeurDate"]).ToShortDateString();
                    else if (attrReader["ValeurBooleen"] != DBNull.Value) valStr = Convert.ToBoolean(attrReader["ValeurBooleen"]) ? "Oui" : "Non";
                    else if (attrReader["OptionValeur"] != DBNull.Value) valStr = attrReader["OptionValeur"].ToString()!;
                    else if (attrReader["IdOptionAttributCategorie"] != DBNull.Value) valStr = "Option #" + attrReader["IdOptionAttributCategorie"];

                    item.ValeursAttributs.Add(new api.Dtos.Annonces.AnnonceAttributeValueDetailsDto
                    {
                        Nom = attrReader["AttributNom"].ToString() ?? "",
                        Valeur = valStr
                    });
                }
            }
        }

        return new PagedResponse<SignalementDto>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<bool> ExistsAsync(long idSignalement)
    {
        using var connection = (SqlConnection)_connectionFactory.CreateConnection();
        const string sql = "SELECT COUNT(1) FROM Signalements WHERE IdSignalement = @Id";
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", idSignalement);
        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();
        return result != null && result != DBNull.Value && Convert.ToInt32(result) > 0;
    }

    public async Task UpdateStatusAsync(long idSignalement, int statut, long adminId)
    {
        using var connection = (SqlConnection)_connectionFactory.CreateConnection();
        const string sql = @"
            UPDATE Signalements 
            SET Statut = @Statut, 
                DateTraitement = SYSUTCDATETIME(), 
                IdAdminTraitant = @AdminId 
            WHERE IdSignalement = @Id";
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Statut", statut);
        command.Parameters.AddWithValue("@AdminId", adminId);
        command.Parameters.AddWithValue("@Id", idSignalement);
        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    public async Task<long?> GetAnnonceIdAsync(long idSignalement)
    {
        using var connection = (SqlConnection)_connectionFactory.CreateConnection();
        const string sql = "SELECT IdAnnonce FROM Signalements WHERE IdSignalement = @Id";
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", idSignalement);
        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();
        return result == null || result == DBNull.Value ? null : (long?)Convert.ToInt64(result);
    }

    private string GetTypeLabel(int type) => type switch {
        1 => "Contenu inapproprié",
        2 => "Fraude",
        3 => "Harcèlement",
        4 => "Autre",
        _ => "Inconnu"
    };

    private string GetStatusLabel(int status) => status switch {
        1 => "En attente",
        2 => "Traité",
        3 => "Rejeté",
        _ => "Inconnu"
    };

    public async Task<bool> HasAdBeenModeratedAsync(long idAnnonce)
    {
        using var connection = (SqlConnection)_connectionFactory.CreateConnection();
        const string sql = "SELECT COUNT(1) FROM Signalements WHERE IdAnnonce = @IdAnnonce AND Statut = 2"; // 2 = TRAITE
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@IdAnnonce", idAnnonce);
        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();
        return result != null && result != DBNull.Value && Convert.ToInt32(result) > 0;
    }
}
