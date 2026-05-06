using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using api.Data.Connections;
using api.Common;
using api.Dtos.Favorites;
using api.Interfaces.Favorites;

namespace api.Repositories.Favorites;

public class FavoriteRepository : IFavoriteRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public FavoriteRepository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<bool> AddAsync(long userId, long annonceId)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            INSERT INTO Favoris (IdUtilisateur, IdAnnonce)
            VALUES (@IdUtilisateur, @IdAnnonce)";
            
        using var command = new SqlCommand(sql, (SqlConnection)connection);
        command.Parameters.AddWithValue("@IdUtilisateur", userId);
        command.Parameters.AddWithValue("@IdAnnonce", annonceId);

        if (connection.State != ConnectionState.Open) await ((SqlConnection)connection).OpenAsync();
        
        try
        {
            return await command.ExecuteNonQueryAsync() > 0;
        }
        catch (SqlException ex) when (ex.Number == 2627) // Primary key/Unique constraint violation
        {
            return false; // Already favorited
        }
    }

    public async Task<bool> RemoveAsync(long userId, long annonceId)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            DELETE FROM Favoris
            WHERE IdUtilisateur = @IdUtilisateur AND IdAnnonce = @IdAnnonce";
            
        using var command = new SqlCommand(sql, (SqlConnection)connection);
        command.Parameters.AddWithValue("@IdUtilisateur", userId);
        command.Parameters.AddWithValue("@IdAnnonce", annonceId);

        if (connection.State != ConnectionState.Open) await ((SqlConnection)connection).OpenAsync();
        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<PagedResponse<FavoriteDto>> GetPagedByUserIdAsync(long userId, int pageNumber, int pageSize)
    {
        using var connection = _connectionFactory.CreateConnection();
        if (connection.State != ConnectionState.Open) await ((SqlConnection)connection).OpenAsync();

        const string countSql = @"
            SELECT COUNT(*) 
            FROM Favoris f
            JOIN Annonces a ON f.IdAnnonce = a.IdAnnonce
            WHERE f.IdUtilisateur = @IdUtilisateur AND a.Statut = 1 AND a.EstActive = 1";
            
        using var countCmd = new SqlCommand(countSql, (SqlConnection)connection);
        countCmd.Parameters.AddWithValue("@IdUtilisateur", userId);
        var countResult = await countCmd.ExecuteScalarAsync();
        int totalCount = countResult != null ? Convert.ToInt32(countResult) : 0;

        const string itemsSql = @"
            SELECT 
                a.IdAnnonce, a.IdUtilisateur, a.IdCategorie, a.Titre, a.Prix, a.Localisation, f.DateCreation,
                c.Nom AS CategorieNom,
                u.Prenom + ' ' + u.Nom AS AnnonceurNom,
                u.PhotoProfilUrl AS AnnonceurPhotoUrl,
                u.Ville,
                (SELECT TOP 1 Url FROM ImagesAnnonce i WHERE i.IdAnnonce = a.IdAnnonce ORDER BY EstPrincipale DESC, OrdreAffichage ASC) AS MainImageUrl
            FROM Favoris f
            JOIN Annonces a ON f.IdAnnonce = a.IdAnnonce
            JOIN Categories c ON a.IdCategorie = c.IdCategorie
            JOIN Utilisateurs u ON a.IdUtilisateur = u.IdUtilisateur
            WHERE f.IdUtilisateur = @IdUtilisateur AND a.Statut = 1 AND a.EstActive = 1
            ORDER BY f.DateCreation DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            
        using var itemsCmd = new SqlCommand(itemsSql, (SqlConnection)connection);
        itemsCmd.Parameters.AddWithValue("@IdUtilisateur", userId);
        itemsCmd.Parameters.AddWithValue("@Offset", (pageNumber - 1) * pageSize);
        itemsCmd.Parameters.AddWithValue("@PageSize", pageSize);

        var items = new List<FavoriteDto>();
        using var reader = await itemsCmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            items.Add(new FavoriteDto
            {
                IdAnnonce = (long)reader["IdAnnonce"],
                IdUtilisateur = (long)reader["IdUtilisateur"],
                IdCategorie = (int)reader["IdCategorie"],
                CategorieNom = (string)reader["CategorieNom"],
                AnnonceurNom = (string)reader["AnnonceurNom"],
                AnnonceurPhotoUrl = reader["AnnonceurPhotoUrl"] == DBNull.Value ? null : (string)reader["AnnonceurPhotoUrl"],
                Titre = (string)reader["Titre"],
                Prix = (decimal)reader["Prix"],
                Localisation = (string)reader["Localisation"],
                Ville = reader["Ville"] == DBNull.Value ? null : (string)reader["Ville"],
                DateCreation = (DateTime)reader["DateCreation"],
                MainImageUrl = reader["MainImageUrl"] == DBNull.Value ? null : (string)reader["MainImageUrl"]
            });
        }

        return new PagedResponse<FavoriteDto>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<bool> IsFavoritedAsync(long userId, long annonceId)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "SELECT 1 FROM Favoris WHERE IdUtilisateur = @IdUtilisateur AND IdAnnonce = @IdAnnonce";
        
        using var command = new SqlCommand(sql, (SqlConnection)connection);
        command.Parameters.AddWithValue("@IdUtilisateur", userId);
        command.Parameters.AddWithValue("@IdAnnonce", annonceId);

        if (connection.State != ConnectionState.Open) await ((SqlConnection)connection).OpenAsync();
        return await command.ExecuteScalarAsync() != null;
    }

    public async Task<IReadOnlyList<long>> GetIdsByUserIdAsync(long userId)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "SELECT IdAnnonce FROM Favoris WHERE IdUtilisateur = @IdUtilisateur";
        
        using var command = new SqlCommand(sql, (SqlConnection)connection);
        command.Parameters.AddWithValue("@IdUtilisateur", userId);

        if (connection.State != ConnectionState.Open) await ((SqlConnection)connection).OpenAsync();
        using var reader = await command.ExecuteReaderAsync();
        
        var ids = new List<long>();
        while (await reader.ReadAsync())
        {
            ids.Add((long)reader["IdAnnonce"]);
        }
        return ids;
    }
}
