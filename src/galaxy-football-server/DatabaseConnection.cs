using System.Threading.Tasks;
using Npgsql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

public class DatabaseConnection
{
    private readonly IConfiguration m_configuration;
    public DatabaseConnection(IConfiguration configuration)
    {
        m_configuration = configuration;
    }

    public string ConnectionString()
    {
        var dbName = m_configuration["GALAXY_FOOTBALL_DB_NAME"];
        var dbUser = m_configuration["GALAXY_FOOTBALL_DB_USER"];
        var dbPassword = m_configuration["GALAXY_FOOTBALL_DB_PASSWORD"];
        var dbHost = m_configuration["GALAXY_FOOTBALL_DB_URL"];
        var dbPort = m_configuration["GALAXY_FOOTBALL_DB_PORT"] ?? "5432";

        if (string.IsNullOrWhiteSpace(dbName))
            throw new InvalidOperationException("Environment variable GALAXY_FOOTBALL_DB_NAME is not set.");
        if (string.IsNullOrWhiteSpace(dbUser))
            throw new InvalidOperationException("Environment variable GALAXY_FOOTBALL_DB_USER is not set.");
        if (string.IsNullOrWhiteSpace(dbPassword))
            throw new InvalidOperationException("Environment variable GALAXY_FOOTBALL_DB_PASSWORD is not set.");
        if (string.IsNullOrWhiteSpace(dbHost))
            throw new InvalidOperationException("Environment variable GALAXY_FOOTBALL_DB_URL is not set.");

        var appName = "galaxy-football-server";
        var connectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword};SSL Mode=Require;Trust Server Certificate=true;Application Name={appName}";
        return connectionString;
    }

    public async Task<bool> TestConnectionAsync(string connectionString, ILogger<DatabaseConnection> logger)
    {
        try
        {
            logger.LogInformation("Testing database connection with database");
            await using var conn = new NpgsqlConnection(connectionString);
            try
            {
                await conn.OpenAsync();
                await conn.CloseAsync();
                return true;
            }
            catch (Npgsql.NpgsqlException ex)
            {
                logger.LogError(ex, "NpgsqlException: {Message}, Code: {Code}", ex.Message, ex.ErrorCode);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception: {Message}", ex.Message);
            }
        } 
        catch (Exception ex)
        {
            logger.LogError(ex, "Outer Exception: {Message}", ex.Message);
        }
        return false;
    }
}