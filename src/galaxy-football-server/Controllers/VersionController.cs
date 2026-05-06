using Microsoft.AspNetCore.Mvc;
using GalaxyFootball.Infrastructure.Git;

[ApiController]
[Route("version")]
public class VersionController : ControllerBase
{
    private readonly ILogger<VersionController> m_logger;
    private readonly IConfiguration m_configuration;

    public record VersionRecord
    (
        string assembly_version,
        string description,
        string game_version,
        string database_version,
        string asp_environment,
        string git_branchname,
        string git_commit,
        string buildtime
);

    public VersionController(ILogger<VersionController> logger, IConfiguration configuration)
    {
        m_logger = logger;
        m_configuration = configuration;
    }

    [HttpGet]
    public ActionResult<VersionRecord> Get()
    {
        // Use AssemblyInformationalVersionAttribute for version info
        string versionString   = VersionInfo.GetVersion();
        string buildTime       = VersionInfo.GetBuildTime();
        string description     = m_configuration["Description"] ?? "No description available";
        string asp_environment = BuildInfo.GetAspNetCoreEnvironment();
        string gameVersion     = GameParameters.GetInfo().Version;
        string databaseVersion = GameParameters.GetInfo().DatabaseVersion.ToString();
        var gitInfo            = GitInfo.GitRepositoryInfo.ReadFrom(AppContext.BaseDirectory);

        var version = new VersionRecord
        (
            versionString, description, gameVersion, databaseVersion, asp_environment, gitInfo.Branch ?? string.Empty, gitInfo.Commit ?? string.Empty, buildTime
        );

        m_logger.LogInformation("Processing version request. Version={version}, Description={description}, Env={env}, Branch={git}",
                                version.assembly_version, version.description, version.asp_environment, version.git_branchname);
                                
        return version; // Automatically wrapped in an OkObjectResult
    }
}
