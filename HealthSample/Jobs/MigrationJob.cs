
using Dapper;
using HealthSample.Controllers;
using System.Data.SqlClient;

namespace HealthSample.Jobs;

public class MigrationJob: BackgroundService
{
    private readonly ILogger _logger;
    private readonly IConfiguration _configuration;

    public MigrationJob(
        ILogger<MigrationJob> logger,
        IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var connectionString = _configuration.GetConnectionString("SqlServerConnection");
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        string commandText = """
            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Todos')
            BEGIN
                CREATE TABLE Todos (
                    Id int PRIMARY KEY,
                    TaskName nvarchar(100),
                    Completion bit
                )
            END
            """;
        await connection.ExecuteAsync(commandText);
    }
}
