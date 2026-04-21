using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;
using Serilog;
using GalaxyFootball.Infrastructure.Cloudflare;

// TODO move this class to infrastructore layer and add configuration for S3 client
public class LogUploaderService : BackgroundService
{
    private readonly IConfiguration m_configuration;
    private readonly ILogger<LogUploaderService> m_logger;
    private readonly FileUploader m_fileUploader;
    
    public LogUploaderService(IConfiguration configuration, ILogger<LogUploaderService> logger)
    {
        m_configuration = configuration;
        m_logger = logger;

        var access_key  = m_configuration["CLOUDFLARE:ACCESS_KEY"];
        var secret_key  = m_configuration["CLOUDFLARE:SECRET_KEY"];
        var service_url = m_configuration["CLOUDFLARE:S3_URL"];

        if (string.IsNullOrEmpty(access_key))
            throw new ArgumentException("CLOUDFLARE:ACCESS_KEY is missing from configuration.");
        if (string.IsNullOrEmpty(secret_key))
            throw new ArgumentException("CLOUDFLARE:SECRET_KEY is missing from configuration.");
        if (string.IsNullOrEmpty(service_url))
            throw new ArgumentException("CLOUDFLARE:S3_URL is missing from configuration.");

        m_fileUploader = new FileUploader(access_key, secret_key, service_url);
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            upload_logfiles();
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    private async void upload_logfiles()
    {
        // Serilog config:
        // for linux use
        // "%TMPDIR%/galaxyfootball/logs/log-.txt", "rollingInterval": "Day" } }
        // for window dev: also define %TMPDIR
        
        var tempPath    = Path.GetTempPath();
        if (!Directory.Exists(tempPath))
        {
            m_logger.LogError("Temp path does not exist: {tempPath}", tempPath);
        }
        var logsDir     = Path.Combine(tempPath, "galaxyfootball", "logs");
        if (!Directory.Exists(logsDir))
        {
            m_logger.LogError("Logs path does not exist: {logsDir}", logsDir);
        }
`
        // Rolling log file name, must the same as configured in Serilog, e.g. "log-.txt" with rollingInterval "Day" will generate log-20240610.txt for June 10, 2024
        var logFileName = Path.Combine(logsDir, $"log-{DateTime.Now:yyyyMMdd}.txt"); 
        if (!File.Exists(logFileName))
        {
            m_logger.LogError("Log file {logFileName} does not exist, skipping upload", logFileName);
            return;
        }

        // Copy the log file to a temp location to avoid file lock issues
        var tempUploadFile = Path.Combine(logsDir, $"log-{DateTime.Now:yyyy-MM-dd}.txt"); // Similar to rolling log file 
        try
        {
            File.Copy(logFileName, tempUploadFile, true);
        }
        catch (Exception ex)
        {
            m_logger.LogError(ex, "Failed to copy log file {logFileName} to temp location {tempUploadFile}", logFileName, tempUploadFile);
            return;
        }

        var bucket_name = m_configuration["CLOUDFLARE:BUCKET_NAME"];
        if (string.IsNullOrEmpty(bucket_name))
            throw new ArgumentException("CLOUDFLARE:BUCKET_NAME is missing from configuration.");

        m_logger.LogInformation("Uploading log file {logFileName} (copied to {tempUploadFile}) to CloudFlare S3 {bucket}",
                                logFileName, tempUploadFile, bucket_name);

        try
        {
            await m_fileUploader.UploadFile(bucket_name, tempUploadFile, "logs");
        }
        catch (Exception ex)
        {
            m_logger.LogError(ex, "Failed to upload log file {logFileName} (temp {tempUploadFile}) to CloudFlare S3", logFileName, tempUploadFile);
        }
        finally
        {
            // Clean up the temp file
            try { if (File.Exists(tempUploadFile)) File.Delete(tempUploadFile); } catch { }
        }
    }
}