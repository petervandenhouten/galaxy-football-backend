using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using GalaxyFootball.Infrastructure.Cloudflare;

[ApiController]
[Route("api/logs")]
public class LogsController : ControllerBase
{
    private readonly LogFiles m_logFiles;
    
    private readonly IConfiguration m_configuration;

    public LogsController(IConfiguration configuration)
    {
        m_configuration = configuration;

        var access_key = m_configuration["CLOUDFLARE:ACCESS_KEY"];
        var secret_key = m_configuration["CLOUDFLARE:SECRET_KEY"];
        var service_url = m_configuration["CLOUDFLARE:S3_URL"];

        m_logFiles = new LogFiles(access_key, secret_key, service_url);
    }


    [HttpGet]
    public async Task<IActionResult> ListLogs()
    {
        // Both functions should be async:
        // The controller action (e.g., ListLogs in LogsController) must be async so it can await the asynchronous operation and not block the ASP.NET request thread.
        // The library function that actually sends the request to Cloudflare should also be async, since it performs I/O-bound work (network call).

        var bucket_name = m_configuration["CLOUDFLARE:BUCKET_NAME"];
        var files = await m_logFiles.ListLogs(bucket_name);
        return Ok(files);
    }

    [HttpGet("{key}")]
    public async Task<IActionResult> GetLog([FromRoute] string key)
    {
        var bucket_name = m_configuration["CLOUDFLARE:BUCKET_NAME"];
        var content = await m_logFiles.GetLog(bucket_name, key);
        return Content(content, "text/plain");
    }
}
