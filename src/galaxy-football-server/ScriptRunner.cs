using GalaxyFootball.Application.Scripts;
using GalaxyFootball.Infrastructure.Database;

/// <summary>
/// Responsible for running scripts derived from BaseScript.
/// Checks if the script can run, and provides a Serilog logger factory.
/// </summary>
public class ScriptRunner
{
    private readonly ApplicationDbContext m_db;
    private readonly ILogger<ScriptRunner> m_logger;

    /// <summary>
    /// Initializes a new instance of ScriptRunner 
    /// </summary>
    public ScriptRunner(ApplicationDbContext db, ILogger<ScriptRunner> logger)
    {
        m_db = db;
        m_logger = logger;
    }

    /// <summary>
    /// Runs the given script if it can run.
    /// </summary>
    /// <param name="script">The script to run.</param>
    /// <param name="loggerName">The logger name to use for this script instance.</param>
    public void RunScript(BaseScript script, string loggerName)
    {
        if (script == null) throw new ArgumentNullException(nameof(script));
        
        if (script.CanRun())
        {
            m_logger.LogInformation("Running script: {ScriptName}", script.GetType().Name);
            try
            {
                script.Run();
                m_logger.LogInformation("Script {ScriptName} completed successfully.", script.GetType().Name);
            }
            catch (Exception ex)
            {
                m_logger.LogError(ex, "Script {ScriptName} failed.", script.GetType().Name);
                throw;
            }
        }
        else
        {
            m_logger.LogWarning("Script {ScriptName} cannot run.", script.GetType().Name);
        }
    }

    /// <summary>
    /// Finds and instantiates a script derived from BaseScript by its class name using reflection.
    /// </summary>
    /// <param name="scriptClassName">The name of the script class to find (case-sensitive).</param>
    /// <returns>An instance of the script, or null if not found.</returns>
    public BaseScript? CreateScriptByName(string scriptClassName)
    {
        var scriptType = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .FirstOrDefault(t => t.IsClass && !t.IsAbstract && typeof(BaseScript).IsAssignableFrom(t) && t.Name == scriptClassName);
        if (scriptType == null)
            return null;
        return Activator.CreateInstance(scriptType) as BaseScript;
    }

    /// <summary>
    /// Runs a script by its class name, if found and can run.
    /// </summary>
    /// <param name="scriptClassName">The name of the script class to run.</param>
    /// <param name="loggerName">The logger name to use for this script instance.</param>
    public void RunScriptByName(string scriptClassName, string loggerName)
    {
        var script = CreateScriptByName(scriptClassName);
        if (script == null)
            throw new InvalidOperationException($"Script class '{scriptClassName}' not found.");
        RunScript(script, loggerName);
    }
}
