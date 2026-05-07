using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using api.Data.Connections;
using api.Interfaces.Annonces;
using api.Models;
using api.Models.Enums;
using System.Text;
using api.Dtos.Annonces;
using System.Linq;

namespace api.Repositories.Annonces;

public class AnnonceRepository : IAnnonceRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public AnnonceRepository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<long> CreateAsync(Annonce annonce, List<ValeurAttributAnnonce> valeurs, List<ImageAnnonce> images)
    {
        using var connection = (SqlConnection)_connectionFactory.CreateConnection();
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            const string sqlAnnonce = @"
                INSERT INTO Annonces (IdUtilisateur, IdCategorie, Titre, Description, Prix, Localisation, Statut, DateCreation, DatePublication, EstActive)
                VALUES (@IdUtilisateur, @IdCategorie, @Titre, @Description, @Prix, @Localisation, @Statut, @DateCreation, @DatePublication, @EstActive);
                SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

            using var cmdAnnonce = new SqlCommand(sqlAnnonce, connection, transaction);
            cmdAnnonce.Parameters.AddWithValue("@IdUtilisateur", annonce.IdUtilisateur);
            cmdAnnonce.Parameters.AddWithValue("@IdCategorie", annonce.IdCategorie);
            cmdAnnonce.Parameters.AddWithValue("@Titre", annonce.Titre);
            cmdAnnonce.Parameters.AddWithValue("@Description", annonce.Description);
            cmdAnnonce.Parameters.AddWithValue("@Prix", annonce.Prix);
            cmdAnnonce.Parameters.AddWithValue("@Localisation", (object?)annonce.Localisation ?? DBNull.Value);
            cmdAnnonce.Parameters.AddWithValue("@Statut", (int)annonce.Statut);
            cmdAnnonce.Parameters.AddWithValue("@DateCreation", annonce.DateCreation);
            cmdAnnonce.Parameters.AddWithValue("@DatePublication", (object?)annonce.DatePublication ?? DBNull.Value);
            cmdAnnonce.Parameters.AddWithValue("@EstActive", annonce.EstActive);

            var idAnnonceObj = await cmdAnnonce.ExecuteScalarAsync();
            var idAnnonce = idAnnonceObj != null ? Convert.ToInt64(idAnnonceObj) : 0;

            foreach (var v in valeurs)
            {
                const string sqlVal = @"
                    INSERT INTO ValeursAttributAnnonce (IdAnnonce, IdAttributCategorie, IdOptionAttributCategorie, ValeurTexte, ValeurNombre, ValeurDate, ValeurBooleen)
                    VALUES (@IdAnnonce, @IdAttributCategorie, @IdOptionAttributCategorie, @ValeurTexte, @ValeurNombre, @ValeurDate, @ValeurBooleen)";
                using var cmdVal = new SqlCommand(sqlVal, connection, transaction);
                cmdVal.Parameters.AddWithValue("@IdAnnonce", idAnnonce);
                cmdVal.Parameters.AddWithValue("@IdAttributCategorie", v.IdAttributCategorie);
                cmdVal.Parameters.AddWithValue("@IdOptionAttributCategorie", (object?)v.IdOptionAttributCategorie ?? DBNull.Value);
                cmdVal.Parameters.AddWithValue("@ValeurTexte", (object?)v.ValeurTexte ?? DBNull.Value);
                cmdVal.Parameters.AddWithValue("@ValeurNombre", (object?)v.ValeurNombre ?? DBNull.Value);
                cmdVal.Parameters.AddWithValue("@ValeurDate", (object?)v.ValeurDate ?? DBNull.Value);
                cmdVal.Parameters.AddWithValue("@ValeurBooleen", (object?)v.ValeurBooleen ?? DBNull.Value);
                await cmdVal.ExecuteNonQueryAsync();
            }

            foreach (var img in images)
            {
                const string sqlImg = @"
                    INSERT INTO ImagesAnnonce (IdAnnonce, Url, OrdreAffichage, EstPrincipale, DateCreation)
                    VALUES (@IdAnnonce, @Url, @OrdreAffichage, @EstPrincipale, @DateCreation)";
                using var cmdImg = new SqlCommand(sqlImg, connection, transaction);
                cmdImg.Parameters.AddWithValue("@IdAnnonce", idAnnonce);
                cmdImg.Parameters.AddWithValue("@Url", img.Url);
                cmdImg.Parameters.AddWithValue("@OrdreAffichage", img.OrdreAffichage);
                cmdImg.Parameters.AddWithValue("@EstPrincipale", img.EstPrincipale);
                cmdImg.Parameters.AddWithValue("@DateCreation", DateTime.UtcNow);
                await cmdImg.ExecuteNonQueryAsync();
            }

            await transaction.CommitAsync();
            return idAnnonce;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> UpdateAsync(Annonce annonce, List<ValeurAttributAnnonce> valeurs)
    {
        using var connection = (SqlConnection)_connectionFactory.CreateConnection();
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            const string sqlUpdate = @"
                UPDATE Annonces 
                SET Titre = @Titre, Description = @Description, Prix = @Prix, Localisation = @Localisation
                WHERE IdAnnonce = @IdAnnonce";
            using var cmdUpdate = new SqlCommand(sqlUpdate, connection, transaction);
            cmdUpdate.Parameters.AddWithValue("@Titre", annonce.Titre);
            cmdUpdate.Parameters.AddWithValue("@Description", annonce.Description);
            cmdUpdate.Parameters.AddWithValue("@Prix", annonce.Prix);
            cmdUpdate.Parameters.AddWithValue("@Localisation", (object?)annonce.Localisation ?? DBNull.Value);
            cmdUpdate.Parameters.AddWithValue("@IdAnnonce", annonce.IdAnnonce);
            await cmdUpdate.ExecuteNonQueryAsync();

            const string sqlDelVals = "DELETE FROM ValeursAttributAnnonce WHERE IdAnnonce = @IdAnnonce";
            using var cmdDel = new SqlCommand(sqlDelVals, connection, transaction);
            cmdDel.Parameters.AddWithValue("@IdAnnonce", annonce.IdAnnonce);
            await cmdDel.ExecuteNonQueryAsync();

            foreach (var v in valeurs)
            {
                const string sqlVal = @"
                    INSERT INTO ValeursAttributAnnonce (IdAnnonce, IdAttributCategorie, IdOptionAttributCategorie, ValeurTexte, ValeurNombre, ValeurDate, ValeurBooleen)
                    VALUES (@IdAnnonce, @IdAttributCategorie, @IdOptionAttributCategorie, @ValeurTexte, @ValeurNombre, @ValeurDate, @ValeurBooleen)";
                using var cmdVal = new SqlCommand(sqlVal, connection, transaction);
                cmdVal.Parameters.AddWithValue("@IdAnnonce", annonce.IdAnnonce);
                cmdVal.Parameters.AddWithValue("@IdAttributCategorie", v.IdAttributCategorie);
                cmdVal.Parameters.AddWithValue("@IdOptionAttributCategorie", (object?)v.IdOptionAttributCategorie ?? DBNull.Value);
                cmdVal.Parameters.AddWithValue("@ValeurTexte", (object?)v.ValeurTexte ?? DBNull.Value);
                cmdVal.Parameters.AddWithValue("@ValeurNombre", (object?)v.ValeurNombre ?? DBNull.Value);
                cmdVal.Parameters.AddWithValue("@ValeurDate", (object?)v.ValeurDate ?? DBNull.Value);
                cmdVal.Parameters.AddWithValue("@ValeurBooleen", (object?)v.ValeurBooleen ?? DBNull.Value);
                await cmdVal.ExecuteNonQueryAsync();
            }

            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> DeleteAsync(long id)
    {
        using var connection = (SqlConnection)_connectionFactory.CreateConnection();
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            // 1. Delete favoris (if any)
            const string sqlFavoris = "DELETE FROM Favoris WHERE IdAnnonce = @Id";
            using var cmdFavoris = new SqlCommand(sqlFavoris, connection, transaction);
            cmdFavoris.Parameters.AddWithValue("@Id", id);
            await cmdFavoris.ExecuteNonQueryAsync();

            // 2. Delete advertiser contacts (if any)
            const string sqlContacts = "DELETE FROM ContactsAnnonceur WHERE IdAnnonce = @Id";
            using var cmdContacts = new SqlCommand(sqlContacts, connection, transaction);
            cmdContacts.Parameters.AddWithValue("@Id", id);
            await cmdContacts.ExecuteNonQueryAsync();

            // 3. Delete attribute values
            const string sqlVals = "DELETE FROM ValeursAttributAnnonce WHERE IdAnnonce = @Id";
            using var cmdVals = new SqlCommand(sqlVals, connection, transaction);
            cmdVals.Parameters.AddWithValue("@Id", id);
            await cmdVals.ExecuteNonQueryAsync();

            // 2. Delete images
            const string sqlImgs = "DELETE FROM ImagesAnnonce WHERE IdAnnonce = @Id";
            using var cmdImgs = new SqlCommand(sqlImgs, connection, transaction);
            cmdImgs.Parameters.AddWithValue("@Id", id);
            await cmdImgs.ExecuteNonQueryAsync();

            // 3. Delete the annonce
            const string sqlAnnonce = "DELETE FROM Annonces WHERE IdAnnonce = @Id";
            using var cmdAnnonce = new SqlCommand(sqlAnnonce, connection, transaction);
            cmdAnnonce.Parameters.AddWithValue("@Id", id);
            var result = await cmdAnnonce.ExecuteNonQueryAsync() > 0;

            await transaction.CommitAsync();
            return result;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<Annonce?> GetByIdAsync(long id)
    {
        using var connection = (SqlConnection)_connectionFactory.CreateConnection();
        const string sql = @"
            SELECT a.*, 
                   ISNULL(u.Prenom, '') + ' ' + ISNULL(u.Nom, '') AS AdSellerName,
                   u.PhotoProfilUrl AS AdSellerPhoto,
                   u.Telephone AS AdSellerPhone,
                   u.Ville as AdSellerVille 
            FROM Annonces a 
            LEFT JOIN Utilisateurs u ON a.IdUtilisateur = u.IdUtilisateur 
            WHERE a.IdAnnonce = @Id";
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);
        
        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapToAnnonce(reader);
        }
        return null;
    }

    public async Task<(IReadOnlyList<Annonce> Items, int TotalCount)> SearchAsync(AnnonceSearchRequestDto request)
    {
        using var connection = (SqlConnection)_connectionFactory.CreateConnection();
        await connection.OpenAsync();

        var queryParams = new Dictionary<string, object>();
        var whereClauses = new List<string> { "a.Statut = 1", "a.EstActive = 1" };

        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            whereClauses.Add("(a.Titre LIKE @Keyword OR a.Titre LIKE @KeywordWord OR a.Description LIKE @Keyword OR a.Description LIKE @KeywordWord)");
            queryParams.Add("@Keyword", $"{request.Keyword}%");
            queryParams.Add("@KeywordWord", $"% {request.Keyword}%");
        }

        if (request.IdCategorie.HasValue)
        {
            whereClauses.Add("a.IdCategorie = @IdCategorie");
            queryParams.Add("@IdCategorie", request.IdCategorie.Value);
        }

        if (request.PrixMin.HasValue)
        {
            whereClauses.Add("a.Prix >= @PrixMin");
            queryParams.Add("@PrixMin", request.PrixMin.Value);
        }

        if (request.PrixMax.HasValue)
        {
            whereClauses.Add("a.Prix <= @PrixMax");
            queryParams.Add("@PrixMax", request.PrixMax.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Localisation))
        {
            whereClauses.Add("a.Localisation LIKE @Localisation");
            queryParams.Add("@Localisation", $"%{request.Localisation}%");
        }

        var joinClauses = new StringBuilder();
        if (request.FiltresDynamiques != null && request.FiltresDynamiques.Any())
        {
            for (int i = 0; i < request.FiltresDynamiques.Count; i++)
            {
                var f = request.FiltresDynamiques[i];
                var alias = $"v{i}";
                joinClauses.Append($" INNER JOIN ValeursAttributAnnonce {alias} ON a.IdAnnonce = {alias}.IdAnnonce AND {alias}.IdAttributCategorie = @AttrId{i}");
                queryParams.Add($"@AttrId{i}", f.IdAttributCategorie);

                if (f.IdOptionAttributCategorie.HasValue)
                {
                    joinClauses.Append($" AND {alias}.IdOptionAttributCategorie = @OptId{i}");
                    queryParams.Add($"@OptId{i}", f.IdOptionAttributCategorie.Value);
                }
                else if (!string.IsNullOrWhiteSpace(f.ValeurTexte))
                {
                    joinClauses.Append($" AND {alias}.ValeurTexte LIKE @Txt{i}");
                    queryParams.Add($"@Txt{i}", $"%{f.ValeurTexte}%");
                }
                else if (f.ValeurNombreMin.HasValue || f.ValeurNombreMax.HasValue)
                {
                    if (f.ValeurNombreMin.HasValue)
                    {
                        joinClauses.Append($" AND {alias}.ValeurNombre >= @NumMin{i}");
                        queryParams.Add($"@NumMin{i}", f.ValeurNombreMin.Value);
                    }
                    if (f.ValeurNombreMax.HasValue)
                    {
                        joinClauses.Append($" AND {alias}.ValeurNombre <= @NumMax{i}");
                        queryParams.Add($"@NumMax{i}", f.ValeurNombreMax.Value);
                    }
                }
                else if (f.ValeurDateMin.HasValue || f.ValeurDateMax.HasValue)
                {
                    if (f.ValeurDateMin.HasValue)
                    {
                        joinClauses.Append($" AND {alias}.ValeurDate >= @DateMin{i}");
                        queryParams.Add($"@DateMin{i}", f.ValeurDateMin.Value);
                    }
                    if (f.ValeurDateMax.HasValue)
                    {
                        joinClauses.Append($" AND {alias}.ValeurDate <= @DateMax{i}");
                        queryParams.Add($"@DateMax{i}", f.ValeurDateMax.Value);
                    }
                }
                else if (f.ValeurBooleen.HasValue)
                {
                    joinClauses.Append($" AND {alias}.ValeurBooleen = @Bool{i}");
                    queryParams.Add($"@Bool{i}", f.ValeurBooleen.Value);
                }
            }
        }

        var whereSql = "WHERE " + string.Join(" AND ", whereClauses);
        var joinSql = joinClauses.ToString();

        var sortBy = request.SortBy?.ToLower() switch
        {
            "price" => "a.Prix",
            "title" => "a.Titre",
            _ => "a.DateCreation"
        };
        var sortDir = request.SortDirection?.ToLower() == "asc" ? "ASC" : "DESC";

        int totalCount = 0;
        using (var countConnection = (SqlConnection)_connectionFactory.CreateConnection())
        {
            var countSql = $@"
                SELECT COUNT(*) 
                FROM Annonces a 
                LEFT JOIN Categories c ON a.IdCategorie = c.IdCategorie
                LEFT JOIN Utilisateurs u ON a.IdUtilisateur = u.IdUtilisateur
                {joinSql}
                {whereSql}";
            using var countCmd = new SqlCommand(countSql, countConnection);
            foreach (var kvp in queryParams) countCmd.Parameters.AddWithValue(kvp.Key, kvp.Value);
            
            await countConnection.OpenAsync();
            var countResult = await countCmd.ExecuteScalarAsync();
            totalCount = countResult != null ? Convert.ToInt32(countResult) : 0;
        }

        var itemsSql = $@"
            SELECT 
                a.IdAnnonce, a.IdUtilisateur, a.IdCategorie, 
                ISNULL(c.Nom, 'Sans catégorie') AS AdCategoryName, 
                a.Titre, a.Prix, a.Localisation, a.Statut, a.DateCreation, 
                img.Url AS MainImageUrl,
                ISNULL(u.Prenom, '') + ' ' + ISNULL(u.Nom, '') AS AdSellerName,
                u.PhotoProfilUrl AS AdSellerPhoto,
                u.Telephone AS AdSellerPhone,
                u.Ville AS AdSellerVille
            FROM Annonces a
            LEFT JOIN Categories c ON a.IdCategorie = c.IdCategorie
            LEFT JOIN Utilisateurs u ON a.IdUtilisateur = u.IdUtilisateur
            OUTER APPLY (
                SELECT TOP 1 Url FROM ImagesAnnonce 
                WHERE IdAnnonce = a.IdAnnonce 
                ORDER BY EstPrincipale DESC, OrdreAffichage ASC
            ) img
            {joinSql}
            {whereSql}
            ORDER BY {sortBy} {sortDir}
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

        var items = new List<Annonce>();
        using (var itemsConnection = (SqlConnection)_connectionFactory.CreateConnection())
        {
            using var itemsCmd = new SqlCommand(itemsSql, itemsConnection);
            foreach (var kvp in queryParams) itemsCmd.Parameters.AddWithValue(kvp.Key, kvp.Value);
            itemsCmd.Parameters.AddWithValue("@Offset", (request.PageNumber - 1) * request.PageSize);
            itemsCmd.Parameters.AddWithValue("@PageSize", request.PageSize);
            
            await itemsConnection.OpenAsync();
            using var reader = await itemsCmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var a = new Annonce
                {
                    IdAnnonce = (long)reader["IdAnnonce"],
                    IdUtilisateur = (long)reader["IdUtilisateur"],
                    IdCategorie = (int)reader["IdCategorie"],
                    Titre = (string)reader["Titre"],
                    Prix = (decimal)reader["Prix"],
                    Localisation = reader["Localisation"] == DBNull.Value ? null : (string)reader["Localisation"],
                    Statut = (StatutAnnonce)(int)reader["Statut"],
                    DateCreation = (DateTime)reader["DateCreation"],
                    CategorieNom = reader["AdCategoryName"] == DBNull.Value ? "Sans catégorie" : (string)reader["AdCategoryName"],
                    MainImageUrl = reader["MainImageUrl"] == DBNull.Value ? null : (string)reader["MainImageUrl"],
                    AnnonceurNom = reader["AdSellerName"] == DBNull.Value ? null : (string)reader["AdSellerName"],
                    AnnonceurPhotoUrl = reader["AdSellerPhoto"] == DBNull.Value ? null : (string)reader["AdSellerPhoto"],
                    AnnonceurTelephone = reader["AdSellerPhone"] == DBNull.Value ? null : (string)reader["AdSellerPhone"],
                    Ville = reader["AdSellerVille"] == DBNull.Value ? null : (string)reader["AdSellerVille"]
                };
                items.Add(a);
            }
        }

        return (items, totalCount);
    }

    public async Task<(IReadOnlyList<Annonce> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, StatutAnnonce? statut = null, bool? estActif = null, long? idUtilisateur = null, string? keyword = null)
    {
        using var connection = (SqlConnection)_connectionFactory.CreateConnection();
        await connection.OpenAsync();

        var whereClauses = new List<string>();
        var parameters = new List<SqlParameter>();

        if (statut.HasValue)
        {
            whereClauses.Add("a.Statut = @Statut");
            parameters.Add(new SqlParameter("@Statut", (int)statut.Value));
        }
        if (estActif.HasValue)
        {
            whereClauses.Add("a.EstActive = @EstActive");
            parameters.Add(new SqlParameter("@EstActive", estActif.Value));
        }
        if (idUtilisateur.HasValue)
        {
            whereClauses.Add("a.IdUtilisateur = @IdUtilisateur");
            parameters.Add(new SqlParameter("@IdUtilisateur", idUtilisateur.Value));
        }
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            whereClauses.Add("(a.Titre LIKE @Keyword OR a.Titre LIKE @KeywordWord OR a.Description LIKE @Keyword OR a.Description LIKE @KeywordWord)");
            parameters.Add(new SqlParameter("@Keyword", $"{keyword}%"));
            parameters.Add(new SqlParameter("@KeywordWord", $"% {keyword}%"));
        }

        string whereSql = whereClauses.Count > 0 ? "WHERE " + string.Join(" AND ", whereClauses) : "";

        string countSql = $"SELECT COUNT(*) FROM Annonces a {whereSql}";
        using var countCmd = new SqlCommand(countSql, connection);
        foreach (var p in parameters) countCmd.Parameters.Add(new SqlParameter(p.ParameterName, p.Value));
        var totalCountObj = await countCmd.ExecuteScalarAsync();
        int totalCount = totalCountObj != null ? Convert.ToInt32(totalCountObj) : 0;

        string itemsSql = $@"
            SELECT 
                a.*, 
                ISNULL(c.Nom, 'Sans catégorie') AS AdCategoryName,
                ISNULL(u.Prenom, '') + ' ' + ISNULL(u.Nom, '') AS AdSellerName, 
                u.PhotoProfilUrl AS AdSellerPhoto,
                u.Telephone AS AdSellerPhone,
                u.Ville AS AdSellerVille,
                img.Url AS MainImageUrl
            FROM Annonces a
            LEFT JOIN Categories c ON a.IdCategorie = c.IdCategorie
            LEFT JOIN Utilisateurs u ON a.IdUtilisateur = u.IdUtilisateur
            OUTER APPLY (
                SELECT TOP 1 Url FROM ImagesAnnonce 
                WHERE IdAnnonce = a.IdAnnonce 
                ORDER BY EstPrincipale DESC, OrdreAffichage ASC
            ) img
            {whereSql}
            ORDER BY a.DateCreation DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

        using var itemsCmd = new SqlCommand(itemsSql, connection);
        foreach (var p in parameters) itemsCmd.Parameters.Add(new SqlParameter(p.ParameterName, p.Value));
        itemsCmd.Parameters.AddWithValue("@Offset", (pageNumber - 1) * pageSize);
        itemsCmd.Parameters.AddWithValue("@PageSize", pageSize);

        using var reader = await itemsCmd.ExecuteReaderAsync();
        var items = new List<Annonce>();
        while (await reader.ReadAsync())
        {
            items.Add(MapToAnnonce(reader));
        }

        return (items, totalCount);
    }

    public async Task<IReadOnlyList<ValeurAttributAnnonce>> GetValeursByAnnonceIdAsync(long idAnnonce)
    {
        using var connection = (SqlConnection)_connectionFactory.CreateConnection();
        const string sql = "SELECT * FROM ValeursAttributAnnonce WHERE IdAnnonce = @IdAnnonce";
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@IdAnnonce", idAnnonce);
        
        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();
        var list = new List<ValeurAttributAnnonce>();
        while (await reader.ReadAsync())
        {
            list.Add(new ValeurAttributAnnonce
            {
                IdValeurAttributAnnonce = (long)reader["IdValeurAttributAnnonce"],
                IdAnnonce = (long)reader["IdAnnonce"],
                IdAttributCategorie = (int)reader["IdAttributCategorie"],
                IdOptionAttributCategorie = reader["IdOptionAttributCategorie"] == DBNull.Value ? null : (int)reader["IdOptionAttributCategorie"],
                ValeurTexte = reader["ValeurTexte"] == DBNull.Value ? null : (string)reader["ValeurTexte"],
                ValeurNombre = reader["ValeurNombre"] == DBNull.Value ? null : (decimal)reader["ValeurNombre"],
                ValeurDate = reader["ValeurDate"] == DBNull.Value ? null : (DateTime)reader["ValeurDate"],
                ValeurBooleen = reader["ValeurBooleen"] == DBNull.Value ? null : (bool)reader["ValeurBooleen"]
            });
        }
        return list;
    }

    public async Task<IReadOnlyList<ImageAnnonce>> GetImagesByAnnonceIdAsync(long idAnnonce)
    {
        using var connection = (SqlConnection)_connectionFactory.CreateConnection();
        const string sql = "SELECT * FROM ImagesAnnonce WHERE IdAnnonce = @IdAnnonce ORDER BY OrdreAffichage ASC";
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@IdAnnonce", idAnnonce);
        
        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();
        var list = new List<ImageAnnonce>();
        while (await reader.ReadAsync())
        {
            list.Add(new ImageAnnonce
            {
                IdImageAnnonce = (long)reader["IdImageAnnonce"],
                IdAnnonce = (long)reader["IdAnnonce"],
                Url = (string)reader["Url"],
                OrdreAffichage = (int)reader["OrdreAffichage"],
                EstPrincipale = (bool)reader["EstPrincipale"],
                DateCreation = (DateTime)reader["DateCreation"]
            });
        }
        return list;
    }

    public async Task<bool> UpdateStatutAsync(long id, StatutAnnonce statut)
    {
        using var connection = (SqlConnection)_connectionFactory.CreateConnection();
        string sql = "UPDATE Annonces SET Statut = @Statut";
        if (statut == StatutAnnonce.PUBLIEE) sql += ", DatePublication = ISNULL(DatePublication, SYSUTCDATETIME())";
        sql += " WHERE IdAnnonce = @Id";
        
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Statut", (int)statut);
        command.Parameters.AddWithValue("@Id", id);
        
        await connection.OpenAsync();
        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<long> AddImageAsync(ImageAnnonce image)
    {
        using var connection = (SqlConnection)_connectionFactory.CreateConnection();
        const string sql = @"
            INSERT INTO ImagesAnnonce (IdAnnonce, Url, OrdreAffichage, EstPrincipale)
            VALUES (@IdAnnonce, @Url, @OrdreAffichage, @EstPrincipale);
            SELECT CAST(SCOPE_IDENTITY() as bigint);";
        
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@IdAnnonce", image.IdAnnonce);
        command.Parameters.AddWithValue("@Url", image.Url);
        command.Parameters.AddWithValue("@OrdreAffichage", image.OrdreAffichage);
        command.Parameters.AddWithValue("@EstPrincipale", image.EstPrincipale);
        
        await connection.OpenAsync();
        var idImgObj = await command.ExecuteScalarAsync();
        return idImgObj != null ? Convert.ToInt64(idImgObj) : 0;
    }

    public async Task<bool> DeleteImageAsync(long idImage)
    {
        using var connection = (SqlConnection)_connectionFactory.CreateConnection();
        const string sql = "DELETE FROM ImagesAnnonce WHERE IdImageAnnonce = @Id";
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", idImage);
        
        await connection.OpenAsync();
        return await command.ExecuteNonQueryAsync() > 0;
    }

    private static Annonce MapToAnnonce(SqlDataReader reader)
    {
        return new Annonce
        {
            IdAnnonce = (long)reader["IdAnnonce"],
            IdUtilisateur = (long)reader["IdUtilisateur"],
            IdCategorie = (int)reader["IdCategorie"],
            Titre = (string)reader["Titre"],
            Description = reader.HasColumn("Description") && reader["Description"] != DBNull.Value ? (string)reader["Description"] : "",
            Prix = (decimal)reader["Prix"],
            Localisation = reader["Localisation"] == DBNull.Value ? null : (string)reader["Localisation"],
            Statut = (StatutAnnonce)(int)reader["Statut"],
            DateCreation = (DateTime)reader["DateCreation"],
            DatePublication = reader.HasColumn("DatePublication") && reader["DatePublication"] != DBNull.Value ? (DateTime)reader["DatePublication"] : null,
            DateExpiration = reader.HasColumn("DateExpiration") && reader["DateExpiration"] != DBNull.Value ? (DateTime)reader["DateExpiration"] : null,
            EstActive = (bool)reader["EstActive"],
            CategorieNom = reader.HasColumn("AdCategoryName") && reader["AdCategoryName"] != DBNull.Value ? (string)reader["AdCategoryName"] : null,
            AnnonceurNom = reader.HasColumn("AdSellerName") && reader["AdSellerName"] != DBNull.Value ? (string)reader["AdSellerName"] : null,
            AnnonceurPhotoUrl = reader.HasColumn("AdSellerPhoto") && reader["AdSellerPhoto"] != DBNull.Value ? (string)reader["AdSellerPhoto"] : null,
            AnnonceurTelephone = reader.HasColumn("AdSellerPhone") && reader["AdSellerPhone"] != DBNull.Value ? (string)reader["AdSellerPhone"] : null,
            MainImageUrl = reader.HasColumn("MainImageUrl") && reader["MainImageUrl"] != DBNull.Value ? (string)reader["MainImageUrl"] : null,
            Ville = reader.HasColumn("AdSellerVille") && reader["AdSellerVille"] != DBNull.Value ? (string)reader["AdSellerVille"] : null
        };
    }
}

public static class SqlDataReaderExtensions
{
    public static bool HasColumn(this SqlDataReader reader, string columnName)
    {
        for (int i = 0; i < reader.FieldCount; i++)
        {
            if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }
}
