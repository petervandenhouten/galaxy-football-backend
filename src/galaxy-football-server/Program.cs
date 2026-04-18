using Serilog;

using Microsoft.Extensions.Configuration;

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
        .Build())
    .Enrich.FromLogContext()
    .CreateLogger();


var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();
builder.Services.AddControllers();
// Register external controllers from Cloudflare.Library
builder.Services.AddControllers()
    .AddApplicationPart(typeof(GalaxyFootball.Infrastructure.LogsController).Assembly);

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
builder.Configuration.AddEnvironmentVariables();

var app = builder.Build();

// Use CORS policy for testing with Admin pages of localhost dev machine
app.UseCors("AllowLocalhost");

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";

// use localhost in the browser
app.Urls.Add($"http://0.0.0.0:{port}");

app.UseHttpsRedirection();

app.MapControllers(); 

app.UseSerilogRequestLogging(); 

app.Run();

