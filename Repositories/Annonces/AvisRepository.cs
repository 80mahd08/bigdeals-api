using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using api.Data.Connections;
using api.Interfaces.Annonces;
using api.Models;

namespace api.Repositories.Annonces;

public class AvisRepository : IAvisRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public AvisRepository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<(IEnumerable<Avis> Items, int TotalCount)> GetPagedByAnnonceIdAsync(long idAnnonce, int page, int pageSize)
    {
        using var connection = (SqlConnection)_connectionFactory.CreateConnection();
        
        // Count Query
        const string countSql = "SELECT COUNT(*) FROM Avis WHERE IdAnnonce = @IdAnnonce AND EstActif = 1";
        using var countCommand = new SqlCommand(countSql, connection);
        countCommand.Parameters.AddWithValue("@IdAnnonce", idAnnonce);
        
        // Data Query with OFFSET/FETCH
        const string dataSql = @"
            SELECT a.*, u.Nom, u.Prenom, u.PhotoProfilUrl, ann.Titre 
            FROM Avis a
            LEFT JOIN Utilisateurs u ON a.IdUtilisateur = u.IdUtilisateur
            LEFT JOIN Annonces ann ON a.IdAnnonce = ann.IdAnnonce
            WHERE a.IdAnnonce = @IdAnnonce AND a.EstActif = 1
            ORDER BY a.DateCreation DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            
        using var dataCommand = new SqlCommand(dataSql, connection);
        dataCommand.Parameters.AddWithValue("@IdAnnonce", idAnnonce);
        dataCommand.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);
        dataCommand.Parameters.AddWithValue("@PageSize", pageSize);
        
        await connection.OpenAsync();
        int totalCount = (int)(await countCommand.ExecuteScalarAsync() ?? 0);
        
        using var reader = await dataCommand.ExecuteReaderAsync();
        var items = new List<Avis>();
        while (await reader.ReadAsync())
        {
            items.Add(MapToAvis(reader));
        }
        return (items, totalCount);
    }

    public async Task<IEnumerable<Avis>> GetByAnnonceIdAsync(long idAnnonce)
    {
        using var connection = (SqlConnection)_connectionFactory.CreateConnection();
        const string sql = @"
            SELECT a.*, u.Nom, u.Prenom, u.PhotoProfilUrl, ann.Titre 
            FROM Avis a
            LEFT JOIN Utilisateurs u ON a.IdUtilisateur = u.IdUtilisateur
            LEFT JOIN Annonces ann ON a.IdAnnonce = ann.IdAnnonce
            WHERE a.IdAnnonce = @IdAnnonce AND a.EstActif = 1
            ORDER BY a.DateCreation DESC";
            
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@IdAnnonce", idAnnonce);
        
        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();
        var items = new List<Avis>();
        while (await reader.ReadAsync())
        {
            items.Add(MapToAvis(reader));
        }
        return items;
    }

    public async Task<IEnumerable<Avis>> GetByAnnonceurIdAsync(long idAnnonceur)
    {
        using var connection = (SqlConnection)_connectionFactory.CreateConnection();
        const string sql = @"
            SELECT a.*, u.Nom, u.Prenom, u.PhotoProfilUrl, ann.Titre 
            FROM Avis a
            LEFT JOIN Annonces ann ON a.IdAnnonce = ann.IdAnnonce
            LEFT JOIN Utilisateurs u ON a.IdUtilisateur = u.IdUtilisateur
            WHERE ann.IdUtilisateur = @IdAnnonceur AND a.EstActif = 1
            ORDER BY a.DateCreation DESC";
            
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@IdAnnonceur", idAnnonceur);
        
        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();
        var items = new List<Avis>();
        while (await reader.ReadAsync())
        {
            items.Add(MapToAvis(reader));
        }
        return items;
    }

    public async Task<Avis?> GetByIdAsync(long idAvis)
    {
        using var connection = (SqlConnection)_connectionFactory.CreateConnection();
        const string sql = @"
            SELECT a.*, u.Nom, u.Prenom, u.PhotoProfilUrl, ann.Titre 
            FROM Avis a
            JOIN Utilisateurs u ON a.IdUtilisateur = u.IdUtilisateur
            JOIN Annonces ann ON a.IdAnnonce = ann.IdAnnonce
            WHERE a.IdAvis = @IdAvis";
            
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@IdAvis", idAvis);
        
        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapToAvis(reader);
        }
        return null;
    }

    public async Task<Avis?> GetByUserAndAnnonceAsync(long idUtilisateur, long idAnnonce)
    {
        using var connection = (SqlConnection)_connectionFactory.CreateConnection();
        const string sql = @"
            SELECT a.*, u.Nom, u.Prenom, u.PhotoProfilUrl, ann.Titre 
            FROM Avis a
            JOIN Utilisateurs u ON a.IdUtilisateur = u.IdUtilisateur
            JOIN Annonces ann ON a.IdAnnonce = ann.IdAnnonce
            WHERE a.IdUtilisateur = @IdUtilisateur AND a.IdAnnonce = @IdAnnonce";
            
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@IdUtilisateur", idUtilisateur);
        command.Parameters.AddWithValue("@IdAnnonce", idAnnonce);
        
        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapToAvis(reader);
        }
        return null;
    }

    public async Task<long> CreateAsync(Avis avis)
    {
        using var connection = (SqlConnection)_connectionFactory.CreateConnection();
        const string sql = @"
            INSERT INTO Avis (IdAnnonce, IdUtilisateur, Note, Commentaire, DateCreation, EstActif)
            VALUES (@IdAnnonce, @IdUtilisateur, @Note, @Commentaire, @DateCreation, @EstActif);
            SELECT CAST(SCOPE_IDENTITY() as BIGINT);";
            
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@IdAnnonce", avis.IdAnnonce);
        command.Parameters.AddWithValue("@IdUtilisateur", avis.IdUtilisateur);
        command.Parameters.AddWithValue("@Note", avis.Note);
        command.Parameters.AddWithValue("@Commentaire", avis.Commentaire);
        command.Parameters.AddWithValue("@DateCreation", avis.DateCreation == default ? DateTime.UtcNow : avis.DateCreation);
        command.Parameters.AddWithValue("@EstActif", avis.EstActif);

        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();
        return result != null ? Convert.ToInt64(result) : 0;
    }

    public async Task<bool> UpdateAsync(Avis avis)
    {
        using var connection = (SqlConnection)_connectionFactory.CreateConnection();
        const string sql = @"
            UPDATE Avis 
            SET Commentaire = @Commentaire, Note = @Note, DateModification = @DateModification
            WHERE IdAvis = @IdAvis";
            
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Commentaire", avis.Commentaire);
        command.Parameters.AddWithValue("@Note", avis.Note);
        command.Parameters.AddWithValue("@DateModification", DateTime.UtcNow);
        command.Parameters.AddWithValue("@IdAvis", avis.IdAvis);

        await connection.OpenAsync();
        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> DeleteAsync(long idAvis)
    {
        using var connection = (SqlConnection)_connectionFactory.CreateConnection();
        const string sql = "DELETE FROM Avis WHERE IdAvis = @IdAvis";
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@IdAvis", idAvis);

        await connection.OpenAsync();
        return await command.ExecuteNonQueryAsync() > 0;
    }

    private static Avis MapToAvis(SqlDataReader reader)
    {
        return new Avis
        {
            IdAvis = (long)reader["IdAvis"],
            IdAnnonce = (long)reader["IdAnnonce"],
            IdUtilisateur = (long)reader["IdUtilisateur"],
            Note = (int)reader["Note"],
            Commentaire = (string)reader["Commentaire"],
            DateCreation = (DateTime)reader["DateCreation"],
            DateModification = reader["DateModification"] == DBNull.Value ? null : (DateTime?)reader["DateModification"],
            EstActif = (bool)reader["EstActif"],
            NomUtilisateur = reader["Nom"] == DBNull.Value ? null : (string)reader["Nom"],
            PrenomUtilisateur = reader["Prenom"] == DBNull.Value ? null : (string)reader["Prenom"],
            PhotoProfilUrl = reader["PhotoProfilUrl"] == DBNull.Value ? null : (string)reader["PhotoProfilUrl"],
            TitreAnnonce = reader["Titre"] == DBNull.Value ? string.Empty : (string)reader["Titre"]
        };
    }
}
