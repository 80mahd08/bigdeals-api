using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using api.Data.Connections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace api.Controllers;

[Route("api/debug/users")]
[ApiController]
public class DebugUsersController : ControllerBase
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public DebugUsersController(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var users = new List<object>();
        using var connection = (SqlConnection)_connectionFactory.CreateConnection();
        await connection.OpenAsync();
        using var cmd = new SqlCommand("SELECT IdUtilisateur, Prenom, Nom, Email FROM Utilisateurs", connection);
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            users.Add(new
            {
                Id = reader.GetInt64(0),
                Prenom = reader.GetString(1),
                Nom = reader.GetString(2),
                Email = reader.GetString(3)
            });
        }
        return Ok(users);
    }
}
