using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using api.Data.Connections;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace api.Controllers;

[Route("api/debug")]
[ApiController]
public class DebugController : ControllerBase
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public DebugController(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    [HttpGet("attributes")]
    public async Task<IActionResult> GetAttributes()
    {
        using var connection = (SqlConnection)_connectionFactory.CreateConnection();
        await connection.OpenAsync();
        
        using var cmdCount = new SqlCommand("SELECT COUNT(*) FROM AttributsCategorie", connection);
        var count = await cmdCount.ExecuteScalarAsync();
        
        var results = new List<object>();

        const string sql = @"
            SELECT c.Nom as CatNom, a.IdAttributCategorie, a.Nom, a.TypeDonnee 
            FROM AttributsCategorie a 
            JOIN Categories c ON a.IdCategorie = c.IdCategorie";
            
        using var cmd = new SqlCommand(sql, connection);
        using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                results.Add(new {
                    Category = reader.GetString(0),
                    Id = reader.GetInt32(1),
                    Name = reader.GetString(2),
                    Type = reader.GetInt32(3)
                });
            }
        }
        return Ok(new { Count = count, Items = results });
    }
}
