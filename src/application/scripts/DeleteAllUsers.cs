using GalaxyFootball.Infrastructure.Database;
using Microsoft.Extensions.Logging;

namespace GalaxyFootball.Application.Scripts
{
    public class DeleteAllUsers : BaseScript
    {
        private readonly ILogger<DeleteAllUsers> m_logger;
        public DeleteAllUsers(ApplicationDbContext db, ILoggerFactory loggerFactory)
            : base(db, loggerFactory)
        {

            m_logger = loggerFactory.CreateLogger<DeleteAllUsers>();

            //
        }

        public override async Task Run()
        {
            var users       = m_db.Users.ToList();
            var players     = m_db.Players.ToList();
            var userPlayers = m_db.UserPlayers.ToList();
            
            m_db.UserPlayers.RemoveRange(userPlayers);
            m_db.Users      .RemoveRange(users);
            m_db.Players    .RemoveRange(players);

            await m_db.SaveChangesAsync();
            m_logger.LogInformation("All users, all players, and all user-player associations have been deleted.");
        }

        public override bool CanRun()
        {
            return true;
        }
    }
}
