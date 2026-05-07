using Amazon.S3;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using GalaxyFootball.Infrastructure.Cloudflare;

[ApiController]
[Route("api/logs")]
public class LogFileController : ControllerBase
{
    private readonly LogFiles m_logFiles;
    private readonly IConfiguration m_configuration;
    private readonly ILogger<LogFileController> m_logger;

    public LogFileController(IConfiguration configuration, ILogger<LogFileController> logger)
    {
        m_configuration = configuration;
        m_logger = logger;

        var access_key = m_configuration["CLOUDFLARE:ACCESS_KEY"];
        var secret_key = m_configuration["CLOUDFLARE:SECRET_KEY"];
        var service_url = m_configuration["CLOUDFLARE:S3_URL"];

        if (string.IsNullOrEmpty(access_key))
            throw new ArgumentException("CLOUDFLARE:ACCESS_KEY is missing from configuration.");
        if (string.IsNullOrEmpty(secret_key))
            throw new ArgumentException("CLOUDFLARE:SECRET_KEY is missing from configuration.");
        if (string.IsNullOrEmpty(service_url))
            throw new ArgumentException("CLOUDFLARE:S3_URL is missing from configuration.");

        m_logFiles = new LogFiles(access_key, secret_key, service_url);
    }


    [HttpGet]
    public async Task<IActionResult> ListLogs()
    {
        var bucketValidation = TryGetBucketName(out var bucketName);
        if (bucketValidation is not null)
        {
            return bucketValidation;
        }

        try
        {
            var files = await m_logFiles.ListLogs(bucketName!);
            return Ok(files);
        }
        catch (AmazonS3Exception ex) when (ex.Message.Contains("bucket name is not valid", StringComparison.OrdinalIgnoreCase))
        {
            m_logger.LogError(ex, "Cloudflare log bucket '{BucketName}' is invalid.", bucketName);
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Cloudflare log storage is misconfigured.",
                detail: "CLOUDFLARE:BUCKET_NAME is not a valid bucket name.");
        }
        catch (AmazonS3Exception ex)
        {
            m_logger.LogError(ex, "Failed to list logs from Cloudflare bucket '{BucketName}'.", bucketName);
            return Problem(
                statusCode: StatusCodes.Status502BadGateway,
                title: "Cloudflare log storage request failed.",
                detail: "The backend could not retrieve logs from Cloudflare storage.");
        }
    }

    [HttpGet("{key}")]
    public async Task<IActionResult> GetLog([FromRoute] string key)
    {
        var bucketValidation = TryGetBucketName(out var bucketName);
        if (bucketValidation is not null)
        {
            return bucketValidation;
        }

        try
        {
            var content = await m_logFiles.GetLog(bucketName!, key);
            return Content(content, "text/plain");
        }
        catch (AmazonS3Exception ex) when (ex.Message.Contains("bucket name is not valid", StringComparison.OrdinalIgnoreCase))
        {
            m_logger.LogError(ex, "Cloudflare log bucket '{BucketName}' is invalid.", bucketName);
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Cloudflare log storage is misconfigured.",
                detail: "CLOUDFLARE:BUCKET_NAME is not a valid bucket name.");
        }
        catch (AmazonS3Exception ex)
        {
            m_logger.LogError(ex, "Failed to fetch log '{LogKey}' from Cloudflare bucket '{BucketName}'.", key, bucketName);
            return Problem(
                statusCode: StatusCodes.Status502BadGateway,
                title: "Cloudflare log storage request failed.",
                detail: "The backend could not retrieve the requested log from Cloudflare storage.");
        }
    }

    private IActionResult? TryGetBucketName(out string? bucketName)
    {
        bucketName = m_configuration["CLOUDFLARE:BUCKET_NAME"];

        if (string.IsNullOrWhiteSpace(bucketName))
        {
            m_logger.LogError("CLOUDFLARE:BUCKET_NAME is missing from configuration.");
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Cloudflare log storage is misconfigured.",
                detail: "CLOUDFLARE:BUCKET_NAME is missing from configuration.");
        }

        if (string.Equals(bucketName, "DISABLED", StringComparison.OrdinalIgnoreCase))
        {
            return Problem(
                statusCode: StatusCodes.Status503ServiceUnavailable,
                title: "Cloudflare log storage is disabled.",
                detail: "Log browsing is disabled for this environment.");
        }

        return null;
    }
}
