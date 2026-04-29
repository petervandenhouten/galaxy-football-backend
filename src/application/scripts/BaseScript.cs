
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
    }
}
