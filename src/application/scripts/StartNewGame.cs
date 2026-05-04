using GalaxyFootball.Application.Factories;
using GalaxyFootball.Infrastructure.Database;
using Microsoft.Extensions.Logging;
using GalaxyFootball.Domain.Entities;

namespace GalaxyFootball.Application.Scripts
{
    // An action initiated by an administrator to start a new GAME

    // - deletes alls tables, except user/player related
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

            setup_game_parameters();
            reset_game_date();

            // clean up the database
            delete_users_without_id();
            delete_players_without_id();

            // todo : delete all history (matches, league results, cup results, player stats, etc.) from previous game

            delete_all_clubs();
            delete_all_teams();
            delete_all_player_club_teams();
            delete_all_leagues();
            delete_all_autocoaches();
            delete_all_robots();

            // m_db.SaveChanges();

            int teams_per_league = m_db.Games.FirstOrDefault()?.NumberOfTeamsInLeague ?? 0;
            int create_nr_leagues = 1 + (m_db.Users.Count() / teams_per_league);
            m_logger.LogInformation("Number of leagues to create: {LeagueCount} (based on {UserCount} users and {TeamsPerLeague} teams per league)", create_nr_leagues, m_db.Users.Count(), teams_per_league);  
            
            int players_assigned_to_club = 0;

            for (int i = 0; i < create_nr_leagues; i++)
            {
                // todo: ask LeagueSystem for the first available level and number
                var (level, number) = LeagueFactory.GetFirstAvailableLevelAndNumber(m_db.Leagues);
                m_logger.LogInformation("Creating league at level {Level}:{Number}...", level, number);

                // Create an empty league
                var league = LeagueFactory.CreateLeague(level,number);
                m_db.Leagues.Add(league);
                
                // create the desired number of teams
                for ( int t=0 ; t<teams_per_league; t++)
                {
                    int competition_indexfor_team = t+1;

                    // assign players to first clubs in highest leagues created
                    if (players_assigned_to_club < m_db.Users.Count())
                    {
                        var user = m_db.Users.OrderBy(u => u.Id).Skip(players_assigned_to_club).FirstOrDefault();
                        if ( user == null || user.Id == Guid.Empty )
                        {
                            m_logger.LogWarning("User with empty ID found. Skipping user.");
                            throw new Exception("User with empty ID found. Cannot start a new game without valid users.");
                        }
                        var player = m_db.UserPlayers.FirstOrDefault(p => p.UserId == user.Id);
                        if ( player == null || player.PlayerId == Guid.Empty )
                        {
                            m_logger.LogWarning("Player with empty ID found for user {UserId}. ", user.Id);
                            throw new Exception("Player with empty ID found. Cannot start a new game without valid players.");
                        }
                        create_club_and_team(player.PlayerId, league.Id, competition_indexfor_team);
                        players_assigned_to_club++;
                    }
                    else
                    {
                        // create an autocoached team for remaining teams without players
                        var coach  = AutoCoachFactory.CreateAutoCoach();
                        m_db.AutoCoaches.Add(coach);
                        create_club_and_team(coach.Id, league.Id, competition_indexfor_team);
                    }
                }
                
                m_logger.LogInformation("Created league at level {Level}:{Number}. [Id={LeagueId}]", league.Level, league.Number, league.Id );
            }

            await m_db.SaveChangesAsync();

            await RunScript<StartNewSeason>();
        }

        private void create_club_and_team(Guid player_coach_Id, Guid leagueId, int team_index)
        {
            var club   = ClubFactory.CreateClub(); 
            while( m_db.Clubs.Any(c => c.Name == club.Name) ) // ensure unique club name
            {
                club = ClubFactory.CreateClub();
            }
            m_db.Clubs.Add(club);
            
            var team   = TeamFactory.CreateTeamForClub(club.Id);
            m_db.Teams.Add(team);

            create_robots_for_team(team.Id, 16);

            link_player_to_club_team        (player_coach_Id, club.Id, team.Id);
            link_team_to_league             (team.Id, leagueId, team_index);    
            link_results_of_team_to_league  (leagueId, team.Id);
        }

        private void create_robots_for_team(Guid teamId, int nr_of_robots)
        {
            var game = m_db.Games.FirstOrDefault();

            for (int i=0; i<nr_of_robots; i++)
            {
                var (robot, brain, body, battery, motor) = RobotFactory.CreateRobot(game?.Year ?? 1, game?.Day ?? 0, teamId);
                m_db.RobotBrains.Add(brain);
                m_db.RobotBodies.Add(body);
                m_db.RobotBatteries.Add(battery);
                m_db.RobotMotors.Add(motor);
                m_db.Robots.Add(robot);

                var team_robot = new TeamRobot
                {
                    TeamId = teamId,
                    RobotId = robot.Id
                };
                m_db.TeamRobots.Add(team_robot);
            }
            m_db.SaveChanges();
        }
        public override bool CanRun()
        {
            m_logger.LogInformation("Checking if a new game can be started...");
            m_logger.LogInformation("Games objects: {GameCount}", m_db.Games.Count());
            m_logger.LogInformation("Number of user registrations: {UserCount}", m_db.Users.Count());

            // Return false if no Game exists in the database, or when there are no Users, since a game cannot start without players.
            return m_db.Games.Count()==1 && m_db.Users.Any();
        }

        private void link_player_to_club_team(Guid playerId, Guid clubId, Guid teamId)
        {
            // Remove any existing PlayerClubTeam entries for this player
            var existingLinks = m_db.PlayerClubTeams.Where(pct => pct.PlayerId == playerId).ToList();
            if (existingLinks.Count > 0)
            {
                m_db.PlayerClubTeams.RemoveRange(existingLinks);
            }
            // Create a new PlayerClubTeam entry to link the player to the club and team
            var player_club_team = new PlayerClubTeam
            {
                PlayerId = playerId,
                ClubId = clubId,
                TeamId = teamId
            };
            m_db.PlayerClubTeams.Add(player_club_team);
            m_db.SaveChanges();
        }
        private void link_team_to_league(Guid teamId, Guid leagueId, int team_index)
        {
            // Create a new TeamCompetition entry to link the team to the league
            var team_competition = new TeamCompetition
            {
                TeamId        = teamId,
                CompetitionId = leagueId,
                TeamIndex     = team_index
            };
            m_db.TeamCompetitions.Add(team_competition);
            m_db.SaveChanges();
        }

        private void delete_all_leagues()
        {
            // Delete all existing leagues and related team competitions
            var all_leagues = m_db.Leagues.ToList();
            var all_leagues_entries = m_db.TeamCompetitions.ToList();
            var all_leagues_results = m_db.LeagueResults.ToList();
            m_db.Leagues.RemoveRange(all_leagues);
            m_db.TeamCompetitions.RemoveRange(all_leagues_entries);
            m_db.LeagueResults.RemoveRange(all_leagues_results);
            m_db.SaveChanges();
        }

        private void delete_all_player_club_teams()
        {
            // Delete all existing player-club-team relationships
            var all_player_club_teams = m_db.PlayerClubTeams.ToList();
            m_db.PlayerClubTeams.RemoveRange(all_player_club_teams);
            m_db.SaveChanges();
        }

        private void delete_all_autocoaches()
        {
            // Delete all existing autocoaches
            var all_autocoaches = m_db.AutoCoaches.ToList();
            m_db.AutoCoaches.RemoveRange(all_autocoaches);
            m_db.SaveChanges();
        }

        private void link_results_of_team_to_league(Guid leagueId, Guid teamId)
        {
            // Create a new LeagueResult entry to link the team to the league results
            var league_results = new LeagueResult
            {
                Id = Guid.NewGuid(),
                TeamId = teamId,
                CompetitionId = leagueId,
                HomePlayed = 0,
                HomeWins = 0,
                HomeDraws = 0,
                HomeLosses = 0,
                HomeGoalsFor = 0,
                HomeGoalsAgainst = 0,
                AwayPlayed = 0,
                AwayWins = 0,
                AwayDraws = 0,
                AwayLosses = 0,
                AwayGoalsFor = 0,
                AwayGoalsAgainst = 0,
                WinningStreak = 0,
                LosingStreak = 0,
                MatchResults = string.Empty
            };
            m_db.LeagueResults.Add(league_results);
            m_db.SaveChanges();
        }

        private void delete_users_without_id()
        {
            var users_without_id = m_db.Users.Where(u => u.Id == Guid.Empty).ToList();
            m_db.Users.RemoveRange(users_without_id);
            m_db.SaveChanges();
        }

        private void delete_players_without_id()
        {
            var players_without_id = m_db.Players.Where(p => p.Id == Guid.Empty).ToList();
            m_db.Players.RemoveRange(players_without_id);
            m_db.SaveChanges();
        }
        
        private void delete_all_clubs()
        {
            // Delete all existing clubs and related club-team relationships
            var all_clubs = m_db.Clubs.ToList();
            var all_club_teams = m_db.ClubTeams.ToList();
            var all_club_stadiums = m_db.ClubStadiums.ToList();
            var all_club_sponsors = m_db.ClubSponsors.ToList();
            var all_player_club_teams = m_db.PlayerClubTeams.ToList();
            var all_season_league_results = m_db.SeasonLeagueResults.ToList();
            var all_club_league_results = m_db.ClubLeagueResults.ToList();
            var all_club_cup_results = m_db.ClubCupResults.ToList();
            var all_season_cup_results = m_db.SeasonCupResults.ToList();
            var all_autocoach_club_teams = m_db.AutoCoachClubTeams.ToList();

            m_db.Clubs.RemoveRange(all_clubs);
            m_db.ClubTeams.RemoveRange(all_club_teams);
            m_db.ClubStadiums.RemoveRange(all_club_stadiums);
            m_db.ClubSponsors.RemoveRange(all_club_sponsors);
            m_db.PlayerClubTeams.RemoveRange(all_player_club_teams);
            m_db.SeasonLeagueResults.RemoveRange(all_season_league_results);
            m_db.ClubLeagueResults.RemoveRange(all_club_league_results);
            m_db.ClubCupResults.RemoveRange(all_club_cup_results);
            m_db.SeasonCupResults.RemoveRange(all_season_cup_results);
            m_db.AutoCoachClubTeams.RemoveRange(all_autocoach_club_teams);
            m_db.SaveChanges();
        }

        private void delete_all_teams()
        {
            // Delete all existing teams and related team-competition relationships
            var all_teams = m_db.Teams.ToList();
            var all_team_competitions = m_db.TeamCompetitions.ToList();
            m_db.Teams.RemoveRange(all_teams);
            m_db.TeamCompetitions.RemoveRange(all_team_competitions);
            m_db.SaveChanges();
        }
        // Store game parameters that could change because of a software update,
        // but should remain fixed during a season.
        private void setup_game_parameters()
        {
            var parameters = GameParameters.GetInfo();
            var game = m_db.Games.FirstOrDefault();
            if (game != null)
            {
                game.NumberOfTeamsInLeague = parameters.NumberOfTeamsInLeague;
                //game.GameVersion = VersionInfo.GetVersion(); // Set the desired game version
                m_db.SaveChanges();
            }
        }

        private void reset_game_date()
        {
            var game = m_db.Games.FirstOrDefault();
            if (game != null)
            {
                game.Year = 0; // Reset for the first year/season
                game.Day = 0;  // Reset for the first day
                game.CurrentLeagueRound = 0;
                game.CurrentCupRound = 0;
                m_db.SaveChanges();
            }   
        }

        private void delete_all_robots()
        {
            var all_robots = m_db.Robots.ToList();
            var all_robot_brains = m_db.RobotBrains.ToList();
            var all_robot_bodies = m_db.RobotBodies.ToList();
            var all_robot_batteries = m_db.RobotBatteries.ToList();
            var all_robot_motors = m_db.RobotMotors.ToList();

            m_db.Robots.RemoveRange(all_robots);
            m_db.RobotBrains.RemoveRange(all_robot_brains);
            m_db.RobotBodies.RemoveRange(all_robot_bodies);
            m_db.RobotBatteries.RemoveRange(all_robot_batteries);
            m_db.RobotMotors.RemoveRange(all_robot_motors);
            m_db.SaveChanges();
        }
    }
}
