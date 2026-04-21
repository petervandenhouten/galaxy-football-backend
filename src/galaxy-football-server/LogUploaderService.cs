using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;
using Amazon.S3;
using Amazon.S3.Transfer;
using Serilog;

// TODO move this class to infrastructore layer and add configuration for S3 client
public class LogUploaderService : BackgroundService
{
    private readonly IConfiguration m_configuration;
    private readonly ILogger<LogUploaderService> m_logger;
    
    public LogUploaderService(IConfiguration configuration, ILogger<LogUploaderService> logger)
    {
        m_configuration = configuration;
        m_logger = logger;
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
        // "%TMP%/GalaxyFootball/logs/log-.txt" with rolling interval of 1 day
        var tempPath    = Path.GetTempPath();
        var logsDir     = Path.Combine(tempPath, "GalaxyFootball", "logs");

        // Log the contents of the logsDir for debugging
        try
        {
            if (Directory.Exists(logsDir))
            {
                var files = Directory.GetFiles(logsDir);
                m_logger.LogInformation("Contents of logsDir ({logsDir}): {Files}", logsDir, string.Join(", ", files));
            }
            else
            {
                m_logger.LogWarning("logsDir does not exist: {logsDir}", logsDir);
            }
        }
        catch (Exception ex)
        {
            m_logger.LogError(ex, "Failed to enumerate files in logsDir: {logsDir}", logsDir);
        }
        
        var logFileName = Path.Combine(logsDir, $"log-{DateTime.Now:yyyyMMdd}.txt"); // Rolling log file name
        if (!File.Exists(logFileName))
        {
            m_logger.LogInformation("Log file {logFileName} does not exist, skipping upload", logFileName);
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

        var access_key  = m_configuration["CLOUDFLARE:ACCESS_KEY"];
        var secret_key  = m_configuration["CLOUDFLARE:SECRET_KEY"];
        var bucket_name = m_configuration["CLOUDFLARE:BUCKET_NAME"];
        var service_url = m_configuration["CLOUDFLARE:S3_URL"];

        m_logger.LogInformation("Uploading log file {logFileName} (copied to {tempUploadFile}) to CloudFlare S3 {bucket}",
                                logFileName, tempUploadFile, bucket_name);

        // TODO Use Cloudflare library

        var config = new AmazonS3Config
        {
            ServiceURL = service_url,
            ForcePathStyle = false // using the default virtual-hosted style for CloudFlare S3
        };

        try
        {
            var client = new AmazonS3Client(access_key, secret_key, config);
            var fileTransferUtility = new TransferUtility(client);
            var uploadRequest = new TransferUtilityUploadRequest
            {
                BucketName = bucket_name,
                Key = "logs/" + Path.GetFileName(tempUploadFile), // Store in "logs/" folder in the bucket
                FilePath = tempUploadFile,
                ContentType = "text/plain",
                DisablePayloadSigning = true, // Critical for Cloudflare R2
            };
            await fileTransferUtility.UploadAsync(uploadRequest);
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