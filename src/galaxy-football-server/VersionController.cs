
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using GalaxyFootball.Infrastructure.Git;

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
        string versionString = "unknown";
        string buildTime = GetBuildTime();
        string description = m_configuration["Description"] ?? "No description available";
        string branch = BuildInfo.GetGitBranchName();

        try
        {
            var infoVersion = Assembly.GetExecutingAssembly()
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "unknown";
            // Only display the short version (e.g., 1.2.3)
            if (!string.IsNullOrEmpty(infoVersion))
            {
                // Split on '+' (for SemVer) or '-' (for pre-release) and take the first part
                var shortVersion = infoVersion.Split('+', '-')[0];
                versionString = shortVersion;
            }
            else
            {
                versionString = "unknown";
            }
        }
        catch { }

        var version = new VersionRecord(buildTime, branch, versionString, description);
        m_logger.LogInformation("Processing request at {buildtime}. Version={version}, Description={description}", version.buildtime, version.version, version.description);
        return version; // Automatically wrapped in an OkObjectResult
    }
    private static string GetBuildTime()
    {
        return Assembly.GetExecutingAssembly()
            .GetCustomAttributes(typeof(AssemblyMetadataAttribute), false)
            .OfType<AssemblyMetadataAttribute>()
            .FirstOrDefault(a => a.Key == "BuildDateTime")?.Value ?? "unknown";
    }
   
}
