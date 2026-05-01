using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using api.Data.Connections;
using api.Interfaces.Annonces;
using api.Models;
using api.Models.Enums;

namespace api.Repositories.Annonces;

public class AnnonceRepository : IAnnonceRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public AnnonceRepository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<long> CreateAsync(Annonce annonce, List<ValeurAttributAnnonce> valeurs)
    {
        // This method is now replaced by the one with images, but keeping it for compatibility if needed.
        return await CreateAsync(annonce, valeurs, new List<ImageAnnonce>());
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
                OUTPUT INSERTED.IdAnnonce
                VALUES (@IdUtilisateur, @IdCategorie, @Titre, @Description, @Prix, @Localisation, @Statut, @DateCreation, @DatePublication, @EstActive)";

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

            var idAnnonce = (long)await cmdAnnonce.ExecuteScalarAsync();

            if (valeurs.Count > 0)
            {
                const string sqlValeur = @"
                    INSERT INTO ValeursAttributAnnonce (IdAnnonce, IdAttributCategorie, IdOptionAttributCategorie, ValeurTexte, ValeurNombre, ValeurDate, ValeurBooleen)
                    VALUES (@IdAnnonce, @IdAttributCategorie, @IdOptionAttributCategorie, @ValeurTexte, @ValeurNombre, @ValeurDate, @ValeurBooleen)";

                foreach (var v in valeurs)
                {
                    using var cmdValeur = new SqlCommand(sqlValeur, connection, transaction);
                    cmdValeur.Parameters.AddWithValue("@IdAnnonce", idAnnonce);
                    cmdValeur.Parameters.AddWithValue("@IdAttributCategorie", v.IdAttributCategorie);
                    cmdValeur.Parameters.AddWithValue("@IdOptionAttributCategorie", (object?)v.IdOptionAttributCategorie ?? DBNull.Value);
                    cmdValeur.Parameters.AddWithValue("@ValeurTexte", (object?)v.ValeurTexte ?? DBNull.Value);
                    cmdValeur.Parameters.AddWithValue("@ValeurNombre", (object?)v.ValeurNombre ?? DBNull.Value);
                    cmdValeur.Parameters.AddWithValue("@ValeurDate", (object?)v.ValeurDate ?? DBNull.Value);
                    cmdValeur.Parameters.AddWithValue("@ValeurBooleen", (object?)v.ValeurBooleen ?? DBNull.Value);
                    await cmdValeur.ExecuteNonQueryAsync();
                }
            }

            if (images.Count > 0)
            {
                const string sqlImage = @"
                    INSERT INTO ImagesAnnonce (IdAnnonce, Url, OrdreAffichage, EstPrincipale, DateCreation)
                    VALUES (@IdAnnonce, @Url, @OrdreAffichage, @EstPrincipale, @DateCreation)";

                foreach (var img in images)
                {
                    using var cmdImage = new SqlCommand(sqlImage, connection, transaction);
                    cmdImage.Parameters.AddWithValue("@IdAnnonce", idAnnonce);
                    cmdImage.Parameters.AddWithValue("@Url", img.Url);
                    cmdImage.Parameters.AddWithValue("@OrdreAffichage", img.OrdreAffichage);
                    cmdImage.Parameters.AddWithValue("@EstPrincipale", img.EstPrincipale);
                    cmdImage.Parameters.AddWithValue("@DateCreation", DateTime.UtcNow);
                    await cmdImage.ExecuteNonQueryAsync();
                }
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
            const string sqlAnnonce = @"
                UPDATE Annonces SET 
                    Titre = @Titre, 
                    Description = @Description, 
                    Prix = @Prix, 
                    Localisation = @Localisation,
                    DateExpiration = @DateExpiration
                WHERE IdAnnonce = @IdAnnonce";

            using var cmdAnnonce = new SqlCommand(sqlAnnonce, connection, transaction);
            cmdAnnonce.Parameters.AddWithValue("@Titre", annonce.Titre);
            cmdAnnonce.Parameters.AddWithValue("@Description", annonce.Description);
            cmdAnnonce.Parameters.AddWithValue("@Prix", annonce.Prix);
            cmdAnnonce.Parameters.AddWithValue("@Localisation", (object?)annonce.Localisation ?? DBNull.Value);
            cmdAnnonce.Parameters.AddWithValue("@DateExpiration", (object?)annonce.DateExpiration ?? DBNull.Value);
            cmdAnnonce.Parameters.AddWithValue("@IdAnnonce", annonce.IdAnnonce);

            var rowsAffected = await cmdAnnonce.ExecuteNonQueryAsync();
            if (rowsAffected == 0)
            {
                await transaction.RollbackAsync();
                return false;
            }

            const string sqlDelete = "DELETE FROM ValeursAttributAnnonce WHERE IdAnnonce = @IdAnnonce";
            using var cmdDelete = new SqlCommand(sqlDelete, connection, transaction);
            cmdDelete.Parameters.AddWithValue("@IdAnnonce", annonce.IdAnnonce);
            await cmdDelete.ExecuteNonQueryAsync();

            if (valeurs.Count > 0)
            {
                const string sqlValeur = @"
                    INSERT INTO ValeursAttributAnnonce (IdAnnonce, IdAttributCategorie, IdOptionAttributCategorie, ValeurTexte, ValeurNombre, ValeurDate, ValeurBooleen)
                    VALUES (@IdAnnonce, @IdAttributCategorie, @IdOptionAttributCategorie, @ValeurTexte, @ValeurNombre, @ValeurDate, @ValeurBooleen)";

                foreach (var v in valeurs)
                {
                    using var cmdValeur = new SqlCommand(sqlValeur, connection, transaction);
                    cmdValeur.Parameters.AddWithValue("@IdAnnonce", annonce.IdAnnonce);
                    cmdValeur.Parameters.AddWithValue("@IdAttributCategorie", v.IdAttributCategorie);
                    cmdValeur.Parameters.AddWithValue("@IdOptionAttributCategorie", (object?)v.IdOptionAttributCategorie ?? DBNull.Value);
                    cmdValeur.Parameters.AddWithValue("@ValeurTexte", (object?)v.ValeurTexte ?? DBNull.Value);
                    cmdValeur.Parameters.AddWithValue("@ValeurNombre", (object?)v.ValeurNombre ?? DBNull.Value);
                    cmdValeur.Parameters.AddWithValue("@ValeurDate", (object?)v.ValeurDate ?? DBNull.Value);
                    cmdValeur.Parameters.AddWithValue("@ValeurBooleen", (object?)v.ValeurBooleen ?? DBNull.Value);
                    await cmdValeur.ExecuteNonQueryAsync();
                }
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
        const string sql = "UPDATE Annonces SET Statut = 5, EstActive = 0 WHERE IdAnnonce = @Id";
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);
        
        await connection.OpenAsync();
        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<Annonce?> GetByIdAsync(long id)
    {
        using var connection = (SqlConnection)_connectionFactory.CreateConnection();
        const string sql = "SELECT * FROM Annonces WHERE IdAnnonce = @Id";
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

    public async Task<(IReadOnlyList<Annonce> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, StatutAnnonce? statut = null, bool? estActif = null, long? idUtilisateur = null)
    {
        using var connection = (SqlConnection)_connectionFactory.CreateConnection();
        await connection.OpenAsync();

        var whereClauses = new List<string>();
        var parameters = new List<SqlParameter>();

        if (statut.HasValue)
        {
            whereClauses.Add("Statut = @Statut");
            parameters.Add(new SqlParameter("@Statut", (int)statut.Value));
        }
        if (estActif.HasValue)
        {
            whereClauses.Add("EstActive = @EstActive");
            parameters.Add(new SqlParameter("@EstActive", estActif.Value));
        }
        if (idUtilisateur.HasValue)
        {
            whereClauses.Add("IdUtilisateur = @IdUtilisateur");
            parameters.Add(new SqlParameter("@IdUtilisateur", idUtilisateur.Value));
        }

        string whereSql = whereClauses.Count > 0 ? "WHERE " + string.Join(" AND ", whereClauses) : "";

        string countSql = $"SELECT COUNT(*) FROM Annonces {whereSql}";
        using var countCmd = new SqlCommand(countSql, connection);
        foreach (var p in parameters) countCmd.Parameters.Add(new SqlParameter(p.ParameterName, p.Value));
        int totalCount = (int)await countCmd.ExecuteScalarAsync();

        string itemsSql = $@"
            SELECT * FROM Annonces 
            {whereSql}
            ORDER BY DateCreation DESC
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

    private static Annonce MapToAnnonce(SqlDataReader reader)
    {
        return new Annonce
        {
            IdAnnonce = (long)reader["IdAnnonce"],
            IdUtilisateur = (long)reader["IdUtilisateur"],
            IdCategorie = (int)reader["IdCategorie"],
            Titre = (string)reader["Titre"],
            Description = (string)reader["Description"],
            Prix = (decimal)reader["Prix"],
            Localisation = reader["Localisation"] == DBNull.Value ? null : (string)reader["Localisation"],
            Statut = (StatutAnnonce)(int)reader["Statut"],
            DateCreation = (DateTime)reader["DateCreation"],
            DatePublication = reader["DatePublication"] == DBNull.Value ? null : (DateTime)reader["DatePublication"],
            DateExpiration = reader["DateExpiration"] == DBNull.Value ? null : (DateTime)reader["DateExpiration"],
            EstActive = (bool)reader["EstActive"]
        };
    }

    // Overload for the interface
    public async Task<long> CreateAsync(Annonce annonce, List<ValeurAttributAnnonce> valeurs, List<ImageAnnonce> images, IDbTransaction? transaction = null)
    {
        return await CreateAsync(annonce, valeurs, images);
    }
}
