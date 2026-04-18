
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
[ApiController]
[Route("version")]
public class VersionController : ControllerBase
{
    private readonly ILogger<VersionController> _logger;
    private readonly IConfiguration _configuration;

    public record VersionRecord(string buildtime, string branchname, string version, string description);

    public VersionController(ILogger<VersionController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }



    [HttpGet]
    public ActionResult<VersionRecord> Get(int id)
    {
        // Use AssemblyInformationalVersionAttribute for version info
        string versionString = "unknown";
        string buildTime = GetBuildTime();
        string description = _configuration["Description"] ?? "No description available";
        string branch = GetGitBranchName();
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
        _logger.LogInformation("Processing request at {buildtime}. Version={version}, Description={description}", version.buildtime, version.version, version.description);
        return version; // Automatically wrapped in an OkObjectResult
    }

    private static string GetGitBranchName()
    {
        // Retrieve branch name from environment variable GIT_BRANCH
        return Environment.GetEnvironmentVariable("GIT_BRANCH") ?? "unknown";
    }

    private static string GetBuildTime()
    {
        return Assembly.GetExecutingAssembly()
            .GetCustomAttributes(typeof(AssemblyMetadataAttribute), false)
            .OfType<AssemblyMetadataAttribute>()
            .FirstOrDefault(a => a.Key == "BuildDateTime")?.Value ?? "unknown";
    }
}
