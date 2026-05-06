using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using api.Data.Connections;
using System.Threading.Tasks;

namespace api.Controllers;

[Route("api/debug/fix-user")]
[ApiController]
public class FixUserController : ControllerBase
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public FixUserController(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    [HttpGet]
    public async Task<IActionResult> FixUser()
    {
        using var connection = (SqlConnection)_connectionFactory.CreateConnection();
        await connection.OpenAsync();
        
        // Ensure Prenom is Mahdi and Nom is Amari
        const string sql = "UPDATE Utilisateurs SET Prenom = 'Mahdi', Nom = 'Amari' WHERE Email = 'mahdi1@gmail.com'";
        using var cmd = new SqlCommand(sql, connection);
        int rows = await cmd.ExecuteNonQueryAsync();
        
        return Ok(new { Message = $"Updated {rows} user(s).", Prenom = "Mahdi", Nom = "Amari" });
    }
}
