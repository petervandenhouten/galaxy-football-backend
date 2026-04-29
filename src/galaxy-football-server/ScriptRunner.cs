using System.Threading.Tasks;
using GalaxyFootball.Application.Scripts;
using GalaxyFootball.Infrastructure.Database;
using Microsoft.Extensions.Logging;

/// <summary>
/// Responsible for running scripts derived from BaseScript.
/// Checks if the script can run, and provides a Serilog logger factory.
/// </summary>
public class ScriptRunner
{
    private readonly ApplicationDbContext m_db;
    private readonly ILoggerFactory m_loggerFactory;

    /// <summary>
    /// Initializes a new instance of ScriptRunner 
    /// </summary>
    public ScriptRunner(ApplicationDbContext db, ILoggerFactory loggerFactory)
    {
        m_db = db;
        m_loggerFactory = loggerFactory;
    }

    /// <summary>
    /// Runs the given script if it can run.
    /// </summary>
    /// <param name="script">The script to run.</param>
    public async Task RunScript(BaseScript script)
    {
        if (script == null) throw new ArgumentNullException(nameof(script));
        var logger = m_loggerFactory.CreateLogger(script.GetType());
        logger.LogInformation("Running script: {ScriptName}", script.GetType().Name);
        if (script.CanRun())
        {
            try
            {
                await script.Run();
                logger.LogInformation("Script {ScriptName} completed successfully.", script.GetType().Name);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Script {ScriptName} failed.", script.GetType().Name);
                throw;
            }
        }
        else
        {
            logger.LogWarning("Script {ScriptName} cannot run.", script.GetType().Name);
        }
    }

    /// <summary>
    /// Finds and instantiates a script derived from BaseScript by its class name using reflection.
    /// </summary>
    /// <param name="scriptClassName">The name of the script class to find (case-sensitive).</param>
    /// <returns>An instance of the script, or null if not found.</returns>
    protected BaseScript? CreateScriptByName(string scriptClassName)
    {
        var scriptType = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .FirstOrDefault(t => t.IsClass && !t.IsAbstract && typeof(BaseScript).IsAssignableFrom(t) && t.Name == scriptClassName);
        if (scriptType == null)
            return null;

        // Look for a constructor (ApplicationDbContext, ILoggerFactory)
        var ctor = scriptType.GetConstructor(new[] { typeof(ApplicationDbContext), typeof(ILoggerFactory) });
        if (ctor == null)
            throw new InvalidOperationException($"Script class '{scriptClassName}' does not have the required constructor.");

        return ctor.Invoke(new object[] { m_db, m_loggerFactory }) as BaseScript;
    }

    /// <summary>
    /// Runs a script by its class name, if found and can run.
    /// </summary>
    /// <param name="scriptClassName">The name of the script class to run.</param>
    public async Task RunScriptByName(string scriptClassName)
    {
        var script = CreateScriptByName(scriptClassName);
        if (script == null)
            throw new InvalidOperationException($"Script class '{scriptClassName}' not found.");
        await RunScript(script);
    }
}
