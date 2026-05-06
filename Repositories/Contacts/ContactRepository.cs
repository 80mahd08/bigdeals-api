using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using api.Data.Connections;
using api.Common;
using api.Dtos.Contacts;
using api.Interfaces.Contacts;
using api.Models;
using api.Models.Enums;

namespace api.Repositories.Contacts;

public class ContactRepository : IContactRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public ContactRepository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<long> CreateAsync(ContactAnnonceur contact)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            INSERT INTO ContactsAnnonceur (IdUtilisateur, IdAnnonce, IdAnnonceur, TypeContact)
            OUTPUT INSERTED.IdContactAnnonceur
            VALUES (@IdUtilisateur, @IdAnnonce, @IdAnnonceur, @TypeContact)";
            
        using var command = new SqlCommand(sql, (SqlConnection)connection);
        command.Parameters.AddWithValue("@IdUtilisateur", contact.IdUtilisateur.HasValue ? (object)contact.IdUtilisateur.Value : DBNull.Value);
        command.Parameters.AddWithValue("@IdAnnonce", contact.IdAnnonce);
        command.Parameters.AddWithValue("@IdAnnonceur", contact.IdAnnonceur);
        command.Parameters.AddWithValue("@TypeContact", (int)contact.TypeContact);

        if (connection.State != ConnectionState.Open) await ((SqlConnection)connection).OpenAsync();
        var result = await command.ExecuteScalarAsync();
        return result != null ? Convert.ToInt64(result) : 0;
    }

    public async Task<PagedResponse<ContactResponseDto>> GetPagedByUserIdAsync(long userId, int pageNumber, int pageSize)
    {
        using var connection = _connectionFactory.CreateConnection();
        if (connection.State != ConnectionState.Open) await ((SqlConnection)connection).OpenAsync();

        const string countSql = "SELECT COUNT(*) FROM ContactsAnnonceur WHERE IdUtilisateur = @IdUtilisateur";
        using var countCmd = new SqlCommand(countSql, (SqlConnection)connection);
        countCmd.Parameters.AddWithValue("@IdUtilisateur", userId);
        var countResult = await countCmd.ExecuteScalarAsync();
        int totalCount = countResult != null ? Convert.ToInt32(countResult) : 0;

        const string itemsSql = @"
            SELECT 
                c.IdContactAnnonceur, c.IdAnnonce, c.IdAnnonceur, c.IdUtilisateur, c.TypeContact, c.DateContact,
                a.Titre AS TitreAnnonce,
                u.Nom AS NomAnnonceur, u.Prenom AS PrenomAnnonceur,
                init.Nom AS NomUtilisateur, init.Prenom AS PrenomUtilisateur
            FROM ContactsAnnonceur c
            JOIN Annonces a ON c.IdAnnonce = a.IdAnnonce
            JOIN Utilisateurs u ON c.IdAnnonceur = u.IdUtilisateur
            LEFT JOIN Utilisateurs init ON c.IdUtilisateur = init.IdUtilisateur
            WHERE c.IdUtilisateur = @IdUtilisateur
            ORDER BY c.DateContact DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            
        using var itemsCmd = new SqlCommand(itemsSql, (SqlConnection)connection);
        itemsCmd.Parameters.AddWithValue("@IdUtilisateur", userId);
        itemsCmd.Parameters.AddWithValue("@Offset", (pageNumber - 1) * pageSize);
        itemsCmd.Parameters.AddWithValue("@PageSize", pageSize);

        var items = await ReadContactsAsync(itemsCmd);
        return new PagedResponse<ContactResponseDto>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<PagedResponse<ContactResponseDto>> GetPagedByAdvertiserIdAsync(long advertiserId, int pageNumber, int pageSize)
    {
        using var connection = _connectionFactory.CreateConnection();
        if (connection.State != ConnectionState.Open) await ((SqlConnection)connection).OpenAsync();

        const string countSql = "SELECT COUNT(*) FROM ContactsAnnonceur WHERE IdAnnonceur = @IdAnnonceur";
        using var countCmd = new SqlCommand(countSql, (SqlConnection)connection);
        countCmd.Parameters.AddWithValue("@IdAnnonceur", advertiserId);
        var countResult = await countCmd.ExecuteScalarAsync();
        int totalCount = countResult != null ? Convert.ToInt32(countResult) : 0;

        const string itemsSql = @"
            SELECT 
                c.IdContactAnnonceur, c.IdAnnonce, c.IdAnnonceur, c.IdUtilisateur, c.TypeContact, c.DateContact,
                a.Titre AS TitreAnnonce,
                u.Nom AS NomAnnonceur, u.Prenom AS PrenomAnnonceur,
                init.Nom AS NomUtilisateur, init.Prenom AS PrenomUtilisateur
            FROM ContactsAnnonceur c
            JOIN Annonces a ON c.IdAnnonce = a.IdAnnonce
            JOIN Utilisateurs u ON c.IdAnnonceur = u.IdUtilisateur
            LEFT JOIN Utilisateurs init ON c.IdUtilisateur = init.IdUtilisateur
            WHERE c.IdAnnonceur = @IdAnnonceur
            ORDER BY c.DateContact DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            
        using var itemsCmd = new SqlCommand(itemsSql, (SqlConnection)connection);
        itemsCmd.Parameters.AddWithValue("@IdAnnonceur", advertiserId);
        itemsCmd.Parameters.AddWithValue("@Offset", (pageNumber - 1) * pageSize);
        itemsCmd.Parameters.AddWithValue("@PageSize", pageSize);

        var items = await ReadContactsAsync(itemsCmd);
        return new PagedResponse<ContactResponseDto>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<PagedResponse<ContactResponseDto>> GetAdminPagedAsync(int pageNumber, int pageSize)
    {
        using var connection = _connectionFactory.CreateConnection();
        if (connection.State != ConnectionState.Open) await ((SqlConnection)connection).OpenAsync();

        const string countSql = "SELECT COUNT(*) FROM ContactsAnnonceur";
        using var countCmd = new SqlCommand(countSql, (SqlConnection)connection);
        var countResult = await countCmd.ExecuteScalarAsync();
        int totalCount = countResult != null ? Convert.ToInt32(countResult) : 0;

        const string itemsSql = @"
            SELECT 
                c.IdContactAnnonceur, c.IdAnnonce, c.IdAnnonceur, c.IdUtilisateur, c.TypeContact, c.DateContact,
                a.Titre AS TitreAnnonce,
                u.Nom AS NomAnnonceur, u.Prenom AS PrenomAnnonceur,
                init.Nom AS NomUtilisateur, init.Prenom AS PrenomUtilisateur
            FROM ContactsAnnonceur c
            JOIN Annonces a ON c.IdAnnonce = a.IdAnnonce
            JOIN Utilisateurs u ON c.IdAnnonceur = u.IdUtilisateur
            LEFT JOIN Utilisateurs init ON c.IdUtilisateur = init.IdUtilisateur
            ORDER BY c.DateContact DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            
        using var itemsCmd = new SqlCommand(itemsSql, (SqlConnection)connection);
        itemsCmd.Parameters.AddWithValue("@Offset", (pageNumber - 1) * pageSize);
        itemsCmd.Parameters.AddWithValue("@PageSize", pageSize);

        var items = await ReadContactsAsync(itemsCmd);
        return new PagedResponse<ContactResponseDto>(items, totalCount, pageNumber, pageSize);
    }

    private async Task<List<ContactResponseDto>> ReadContactsAsync(SqlCommand command)
    {
        var items = new List<ContactResponseDto>();
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            items.Add(new ContactResponseDto
            {
                IdContactAnnonceur = (long)reader["IdContactAnnonceur"],
                IdAnnonce = (long)reader["IdAnnonce"],
                IdAnnonceur = (long)reader["IdAnnonceur"],
                IdUtilisateur = reader["IdUtilisateur"] == DBNull.Value ? null : (long?)reader["IdUtilisateur"],
                TypeContact = (TypeContact)(int)reader["TypeContact"],
                DateContact = (DateTime)reader["DateContact"],
                TitreAnnonce = (string)reader["TitreAnnonce"],
                NomAnnonceur = (string)reader["NomAnnonceur"],
                PrenomAnnonceur = (string)reader["PrenomAnnonceur"],
                NomUtilisateur = reader["NomUtilisateur"] == DBNull.Value ? null : (string)reader["NomUtilisateur"],
                PrenomUtilisateur = reader["PrenomUtilisateur"] == DBNull.Value ? null : (string)reader["PrenomUtilisateur"]
            });
        }
        return items;
    }
}
