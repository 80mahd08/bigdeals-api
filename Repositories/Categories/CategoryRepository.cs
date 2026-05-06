using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using api.Data.Connections;
using api.Interfaces.Categories;
using api.Models;
using api.Models.Enums;

namespace api.Repositories.Categories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public CategoryRepository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<Categorie>> GetAllAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "SELECT * FROM Categories WHERE EstActive = 1 ORDER BY OrdreAffichage";
        using var command = new SqlCommand(sql, (SqlConnection)connection);
        
        if (connection.State != ConnectionState.Open) await ((SqlConnection)connection).OpenAsync();
        using var reader = await command.ExecuteReaderAsync();
        
        var categories = new List<Categorie>();
        while (await reader.ReadAsync())
        {
            categories.Add(MapToCategorie(reader));
        }
        return categories;
    }

    public async Task<Categorie?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "SELECT * FROM Categories WHERE IdCategorie = @Id AND EstActive = 1";
        using var command = new SqlCommand(sql, (SqlConnection)connection);
        command.Parameters.AddWithValue("@Id", id);
        
        if (connection.State != ConnectionState.Open) await ((SqlConnection)connection).OpenAsync();
        using var reader = await command.ExecuteReaderAsync();
        
        if (await reader.ReadAsync())
        {
            return MapToCategorie(reader);
        }
        return null;
    }

    public async Task<IReadOnlyList<AttributCategorie>> GetAttributesByCategoryIdAsync(int idCategorie)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "SELECT * FROM AttributsCategorie WHERE IdCategorie = @IdCategorie AND EstActive = 1 ORDER BY OrdreAffichage";
        using var command = new SqlCommand(sql, (SqlConnection)connection);
        command.Parameters.AddWithValue("@IdCategorie", idCategorie);
        
        if (connection.State != ConnectionState.Open) await ((SqlConnection)connection).OpenAsync();
        using var reader = await command.ExecuteReaderAsync();
        
        var attributes = new List<AttributCategorie>();
        while (await reader.ReadAsync())
        {
            attributes.Add(MapToAttributCategorie(reader));
        }
        return attributes;
    }

    public async Task<IReadOnlyList<OptionAttributCategorie>> GetOptionsByAttributeIdAsync(int idAttributCategorie)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "SELECT * FROM OptionsAttributCategorie WHERE IdAttributCategorie = @IdAttributCategorie AND EstActive = 1 ORDER BY OrdreAffichage";
        using var command = new SqlCommand(sql, (SqlConnection)connection);
        command.Parameters.AddWithValue("@IdAttributCategorie", idAttributCategorie);
        
        if (connection.State != ConnectionState.Open) await ((SqlConnection)connection).OpenAsync();
        using var reader = await command.ExecuteReaderAsync();
        
        var options = new List<OptionAttributCategorie>();
        while (await reader.ReadAsync())
        {
            options.Add(MapToOptionAttributCategorie(reader));
        }
        return options;
    }

    private static Categorie MapToCategorie(SqlDataReader reader)
    {
        return new Categorie
        {
            IdCategorie = reader.GetInt32(reader.GetOrdinal("IdCategorie")),
            Nom = reader.GetString(reader.GetOrdinal("Nom")),
            Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
            IconKey = reader.IsDBNull(reader.GetOrdinal("IconKey")) ? null : reader.GetString(reader.GetOrdinal("IconKey")),
            OrdreAffichage = reader.GetInt32(reader.GetOrdinal("OrdreAffichage")),
            EstActive = reader.GetBoolean(reader.GetOrdinal("EstActive")),
            DateCreation = reader.GetDateTime(reader.GetOrdinal("DateCreation"))
        };
    }

    private static AttributCategorie MapToAttributCategorie(SqlDataReader reader)
    {
        return new AttributCategorie
        {
            IdAttributCategorie = reader.GetInt32(reader.GetOrdinal("IdAttributCategorie")),
            IdCategorie = reader.GetInt32(reader.GetOrdinal("IdCategorie")),
            Nom = reader.GetString(reader.GetOrdinal("Nom")),
            TypeDonnee = (TypeDonneeAttribut)reader.GetInt32(reader.GetOrdinal("TypeDonnee")),
            Obligatoire = reader.GetBoolean(reader.GetOrdinal("Obligatoire")),
            Filtrable = reader.GetBoolean(reader.GetOrdinal("Filtrable")),
            OrdreAffichage = reader.GetInt32(reader.GetOrdinal("OrdreAffichage")),
            Placeholder = reader.IsDBNull(reader.GetOrdinal("Placeholder")) ? null : reader.GetString(reader.GetOrdinal("Placeholder")),
            EstPlage = reader.GetBoolean(reader.GetOrdinal("EstPlage")),
            EstActive = reader.GetBoolean(reader.GetOrdinal("EstActive"))
        };
    }

    private static OptionAttributCategorie MapToOptionAttributCategorie(SqlDataReader reader)
    {
        return new OptionAttributCategorie
        {
            IdOptionAttributCategorie = reader.GetInt32(reader.GetOrdinal("IdOptionAttributCategorie")),
            IdAttributCategorie = reader.GetInt32(reader.GetOrdinal("IdAttributCategorie")),
            Valeur = reader.GetString(reader.GetOrdinal("Valeur")),
            OrdreAffichage = reader.GetInt32(reader.GetOrdinal("OrdreAffichage")),
            EstActive = reader.GetBoolean(reader.GetOrdinal("EstActive"))
        };
    }
}
