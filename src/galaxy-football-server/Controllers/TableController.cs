using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Data;
using System.Text.Json;

// This controller is for debugging purposes only, to allow us to easily inspect the database tables and their contents
[ApiController]
[Route("api/[controller]")]
public class TableController : ControllerBase
{
    private readonly IConfiguration m_config;

    public TableController(IConfiguration config)
    {
        m_config = config;
    }

    private string GetConnectionString()
    {
        var databaseConnection = new DatabaseConnection(m_config);
        return databaseConnection.ConnectionString();
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("list")]
    public async Task<IActionResult> GetTables()
    {
        var tables = new List<string>();
        using var conn = new NpgsqlConnection(GetConnectionString());
        await conn.OpenAsync();
        using var cmd = new NpgsqlCommand(
            "SELECT table_name FROM information_schema.tables WHERE table_schema = 'public';", conn);
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            tables.Add(reader.GetString(0));
        }
        return Ok(tables);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("{tableName}")]
    public async Task<IActionResult> GetTableRows(string tableName)
    {
        var rows = new List<Dictionary<string, object>>();
        using var conn = new NpgsqlConnection(GetConnectionString());
        await conn.OpenAsync();
        using var cmd = new NpgsqlCommand($"SELECT * FROM \"{tableName}\" LIMIT 100;", conn);
        using var reader = await cmd.ExecuteReaderAsync();
        var schema = reader.GetColumnSchema();
        while (await reader.ReadAsync())
        {
            var row = new Dictionary<string, object>();
            foreach (var col in schema)
            {
                row[col.ColumnName] = reader[col.ColumnName];
            }
            rows.Add(row);
        }
        return Ok(rows);
    }
}