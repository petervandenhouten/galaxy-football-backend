
using GalaxyFootball.Infrastructure.Database;
using Microsoft.Extensions.Logging;

namespace GalaxyFootball.Application.Scripts
{
    public abstract class BaseScript
    {
        protected readonly ApplicationDbContext m_db;
        protected readonly ILoggerFactory m_loggerFactory;

        public BaseScript(ApplicationDbContext db, ILoggerFactory loggerFactory)
        {
            m_db = db;
            m_loggerFactory = loggerFactory;
        }
        abstract public Task Run();
        abstract public bool CanRun();
        //abstract public Task RunAsync();
        //abstract public Task<bool> CanRunAsync();

        /// <summary>
        /// Runs another script, passing the current db and logger.
        /// </summary>
        /// <typeparam name="TScript">The script type to run.</typeparam>
        protected async Task RunScript<TScript>() where TScript : BaseScript
        {
            var scriptObj = Activator.CreateInstance(typeof(TScript), m_db, m_loggerFactory);
            if (scriptObj is not TScript script)
                throw new InvalidOperationException($"Could not create instance of {typeof(TScript).Name}.");
            if (script.CanRun())
            {
                await script.Run();
            }
        }

        /// <summary>
        /// Runs a script by its class name (string), passing the current db and logger.
        /// Returns false if the script doesn't exist (not yet implemented), true if run successfully or skipped.
        /// </summary>
        /// <param name="scriptName">The class name of the script to run (e.g., "StartNewGame").</param>
        /// <returns>True if script exists and was processed, false if script not found.</returns>
        public async Task<bool> RunScriptByName(string scriptName)
        {
            var ns = typeof(BaseScript).Namespace;
            var type = Type.GetType($"{ns}.{scriptName}");
            if (type == null)
            {
                var logger = m_loggerFactory.CreateLogger<BaseScript>();
                logger.LogWarning("Script type '{ScriptName}' not found in namespace '{Namespace}'. This script may not be implemented yet.", scriptName, ns);
                return false;
            }
            if (!typeof(BaseScript).IsAssignableFrom(type))
                throw new InvalidOperationException($"Type '{scriptName}' does not inherit from BaseScript.");
            var scriptObj = Activator.CreateInstance(type, m_db, m_loggerFactory);
            if (scriptObj is not BaseScript script)
                throw new InvalidOperationException($"Could not create instance of {scriptName}.");
            if (script.CanRun())
            {
                await script.Run();
            }
            return true;
        }

    }
}

/* TEMPLATE BEGIN

using GalaxyFootball.Infrastructure.Database;
using Microsoft.Extensions.Logging;

namespace GalaxyFootball.Application.Scripts
{
    public class <SCRIPTNAME> : BaseScript
    {
        private readonly ILogger<<SCRIPTNAME>> m_logger;
        public <SCRIPTNAME>(ApplicationDbContext db, ILoggerFactory loggerFactory)
            : base(db, loggerFactory)
        {
            m_logger = loggerFactory.CreateLogger<<SCRIPTNAME>>();
        }

        public override bool CanRun()
        {
            throw new NotImplementedException();
        }

        public override Task Run()
        {
            throw new NotImplementedException();
        }
    }
}

TEMPLATE END */