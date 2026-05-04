using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using GalaxyFootball.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
        .Build())
    .Filter.ByExcluding(logEvent =>
        logEvent.Level < LogEventLevel.Warning &&
        logEvent.RenderMessage().Contains("System.Collections.Generic", StringComparison.Ordinal))
    .Enrich.FromLogContext()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddControllers();
// Register external controllers from Cloudflare.Library
//builder.Services.AddControllers()
//    .AddApplicationPart(typeof(GalaxyFootball.Infrastructure.LogsController).Assembly);

// Enable CORS for localhost origins
builder.Services.AddCors(options =>
// TODO: For production, restrict origins instead of AllowAnyOrigin() for security.
{
    options.AddPolicy("AllowLocalhost",
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod()
    );
});

builder.Services.AddHostedService<LogUploaderService>();

builder.Services.AddScoped<JobService>();
builder.Services.AddScoped<ScriptRunner>();

var key = builder.Configuration["GALAXY_FOOTBALL_JWT_KEY"];
if (string.IsNullOrEmpty(key))
{
    Log.Error("GALAXY_FOOTBALL_JWT_KEY is missing from configuration. JWT authentication will not work.");
}
else
{
    Console.WriteLine($"PROGRAM JWT KEY: {key}");

    var IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "galaxy-football-backend",
                ValidAudience = "galaxy-football-clients",
                IssuerSigningKey = IssuerSigningKey
            };
        });
}

builder.Services.AddAuthorization();

// Resolve logger and pass to DatabaseConnection
var databaseConnection = new DatabaseConnection(builder.Configuration);
var connectionString = databaseConnection.ConnectionString();
Log.Information("Using database connection string: {connectionString}", connectionString);
// Register DbContext after verifying connection    
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));

var app = builder.Build();

var game_params = GameParameters.GetInfo();

// On startup, check and reset database if needed
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
    var game = db.Games.FirstOrDefault();
    if (game != null && game.DatabaseVersion < game_params.DatabaseVersion)
    {
        // Optionally, you can drop and recreate the database, or just migrate and update the version
        // db.Database.EnsureDeleted();
        // db.Database.Migrate();
        game.DatabaseVersion = game_params.DatabaseVersion;
        db.SaveChanges();
        Log.Information($"Database upgraded to version {game_params.DatabaseVersion}.");
    }
    else if (game == null)
    {
        // Seed initial game state if missing
        db.Games.Add(new GalaxyFootball.Domain.Entities.Game {
            Id = Guid.NewGuid(),
            IsPaused = false,
            IsLocked = false,
            Year = 1,
            Day = 0,
            IsProcessing = false,
            DatabaseVersion = game_params.DatabaseVersion
        });
        db.SaveChanges();
        Log.Information($"Database seeded with initial game state version {game_params.DatabaseVersion}.");
    }
}

app.UseAuthentication();
app.UseAuthorization();

// Use CORS policy for testing with Admin pages of localhost dev machine
app.UseCors("AllowLocalhost");

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";

// use localhost in the browser
app.Urls.Add($"http://0.0.0.0:{port}");

app.UseHttpsRedirection();

app.MapControllers(); 

app.UseSerilogRequestLogging(); 

var logger = app.Services.GetRequiredService<Microsoft.Extensions.Logging.ILogger<DatabaseConnection>>();
if ( databaseConnection.TestConnectionAsync(connectionString, logger).GetAwaiter().GetResult() )
{
    Log.Information("Successfully connected to the database.");
}
else
{
    Log.Error("Failed to connect to the database. Please check your connection string and database server.");
}

app.Run();

