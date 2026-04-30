using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("version")]
public class VersionController : ControllerBase
{
    private readonly ILogger<VersionController> m_logger;
    private readonly IConfiguration m_configuration;

    public record VersionRecord(string buildtime, string branchname, string version, string description);

    public VersionController(ILogger<VersionController> logger, IConfiguration configuration)
    {
        m_logger = logger;
        m_configuration = configuration;
    }

    [HttpGet]
    public ActionResult<VersionRecord> Get()
    {
        // Use AssemblyInformationalVersionAttribute for version info
        string versionString = VersionInfo.GetVersion();
        string buildTime     = VersionInfo.GetBuildTime();
        string description   = m_configuration["Description"] ?? "No description available";
        string branch        = BuildInfo.GetGitBranchName();

        var version = new VersionRecord(buildTime, branch, versionString, description);
        m_logger.LogInformation("Processing request at {buildtime}. Version={version}, Description={description}", version.buildtime, version.version, version.description);
        return version; // Automatically wrapped in an OkObjectResult
    }
}
