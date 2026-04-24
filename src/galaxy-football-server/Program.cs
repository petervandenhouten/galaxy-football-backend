using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using GalaxyFootball.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
        .Build())
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

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "yourIssuer",
            ValidAudience = "yourAudience",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("yourSuperSecretKey"))
        };
    });

builder.Services.AddAuthorization();

// Resolve logger and pass to DatabaseConnection
var databaseConnection = new DatabaseConnection(builder.Configuration);
var connectionString = databaseConnection.ConnectionString();
Log.Information("Using database connection string: {connectionString}", connectionString);
// Register DbContext after verifying connection    
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));

var app = builder.Build();

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
if ( databaseConnection.TestConnectionAsync(connectionString, logger).Result )
{
    Log.Information("Successfully connected to the database.");
}
else
{
    Log.Error("Failed to connect to the database. Please check your connection string and database server.");
    throw new InvalidOperationException("Database connection failed.");
}

app.Run();

