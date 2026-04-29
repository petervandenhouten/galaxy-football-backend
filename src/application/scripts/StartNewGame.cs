using GalaxyFootball.Infrastructure.Database;
using Microsoft.Extensions.Logging;

namespace GalaxyFootball.Application.Scripts
{
    // An action initiated by an administrator to start a new GAME

    // - users accounts are not changed
    // - user-players relationships are not changed
    // - player get a new team
    // - new leagues are created with new clubs and new robots
    // - all players (users) get a team
    // - remaining teams get an autocoach
    // - all history is deleted
    // - game starts at day 1
    // - calculate the numbers of days, weeks, league rounds, cup rounds of one season 
    // - run the start of season script

    public class StartNewGame : BaseScript
    {
        private readonly ILogger<StartNewGame> m_logger;
        public StartNewGame(ApplicationDbContext db, ILoggerFactory loggerFactory)
            : base(db, loggerFactory)
        {
            m_logger = loggerFactory.CreateLogger<StartNewGame>();
        }

        public override async Task Run()
        {
            // Implement the logic to start a new game
            m_logger.LogInformation("Starting a new game...");

            if ( m_db.Users.Count() == m_db.Players.Count() &&
                 m_db.Users.Count() == m_db.UserPlayers.Count() )
            {
                m_logger.LogInformation("Found {UserCount} users and {PlayerCount} players in the database. A new game can be started.", m_db.Users.Count(), m_db.Players.Count());
            }
            else
            {
                m_logger.LogWarning("No sufficient players related to the users in the database. A new game cannot be started without players.");
                return;
            }

            int teams_per_league = m_db.Games.FirstOrDefault()?.NumberOfTeamsInLeague ?? 0;
            int create_nr_leagues = 1 + (m_db.Users.Count() / teams_per_league);
            m_logger.LogInformation("Number of leagues to create: {LeagueCount} (based on {UserCount} users and {TeamsPerLeague} teams per league)", create_nr_leagues, m_db.Users.Count(), teams_per_league);  
            

            await RunScript<StartNewSeason>();
        }

        public override bool CanRun()
        {
            m_logger.LogInformation("Checking if a new game can be started...");
            m_logger.LogInformation("Games objects: {GameCount}", m_db.Games.Count());
            m_logger.LogInformation("Number of user registrations: {UserCount}", m_db.Users.Count());

            // Return false if no Game exists in the database, or when there are no Users, since a game cannot start without players.
            return m_db.Games.Count()==1 && m_db.Users.Any();
        }
    }
}
