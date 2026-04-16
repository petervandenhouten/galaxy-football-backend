using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("version")]
public class VersionController : ControllerBase
{
    private readonly ILogger<VersionController> _logger;

    public record VersionRecord(DateTime time, string version);

    public VersionController(ILogger<VersionController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public ActionResult<VersionRecord> Get(int id)
    {
        var version = new VersionRecord(DateTime.Now, "0.0.1");
        if (version == null) return NotFound();

        _logger.LogInformation("Processing request at {time}. Version={version}", version.time, version.version);

        return version; // Automatically wrapped in an OkObjectResult
    }
}
