using System.Configuration.Assemblies;
using GalaxyFootball.Application.Factories;
using GalaxyFootball.Domain.Entities;
using Xunit;

namespace Application.Tests
{
	public class LeagueMatchesTest
	{
		[Fact]
		public void CreateLeagueMatches_GeneratesMatchSchedule()
		{
			// Arrange
			var factory = new CalendarFactory(null);
			var param = GameParameters.GetInfo();
			var game = new Game
			{
				Year = 2026,
				NumberOfTeamsInLeague = 8,
				CurrentLeagueRound = 0,
				CurrentCupRound = 0,
				Day = 1,
				IsPaused = false,
				IsProcessing = false,
				IsLocked = false,
				DatabaseVersion = 1,
				GameVersion = "1.0"
			};

			int totalNumberOfClubs = 8;
			int nr_league_rounds = (game.NumberOfTeamsInLeague-1) * 2;
            int matches_per_round = game.NumberOfTeamsInLeague/2;
            int expected_matches = nr_league_rounds * matches_per_round;
            int home_matches_per_team = nr_league_rounds/2;
            int away_matches_per_team = nr_league_rounds/2;
			// Act
			var calendar = factory.CreateCalender(game, param, totalNumberOfClubs);
			Assert.NotNull(calendar);
			Assert.Equal(nr_league_rounds, calendar.Count(d => d.DayType==CalendarDayType.LeagueMatch));

            var league_id = Guid.NewGuid();

            var match_factory = new LeagueMatchFactory();
            var matches = match_factory.create_matches_for_league(league_id, calendar);

            Assert.NotNull(matches);
            //Assert.All( matches, m => m.CompetitionID == league_id);
            //Assert.All( matches, m => m.CompetitionID == league_id);
            Assert.Equal(expected_matches, matches.Count());
            Assert.Equal(home_matches_per_team, matches.Count(m => m.TeamHomeIndex == 1));
            Assert.Equal(home_matches_per_team, matches.Count(m => m.TeamHomeIndex == 2));
            Assert.Equal(home_matches_per_team, matches.Count(m => m.TeamHomeIndex == 3));
            Assert.Equal(home_matches_per_team, matches.Count(m => m.TeamHomeIndex == 4));
            Assert.Equal(home_matches_per_team, matches.Count(m => m.TeamHomeIndex == 5));
            Assert.Equal(home_matches_per_team, matches.Count(m => m.TeamHomeIndex == 6));
            Assert.Equal(home_matches_per_team, matches.Count(m => m.TeamHomeIndex == 7));
            Assert.Equal(home_matches_per_team, matches.Count(m => m.TeamHomeIndex == 8));
            Assert.Equal(away_matches_per_team, matches.Count(m => m.TeamAwayIndex == 1));
            Assert.Equal(away_matches_per_team, matches.Count(m => m.TeamAwayIndex == 2));
            Assert.Equal(away_matches_per_team, matches.Count(m => m.TeamAwayIndex == 3));
            Assert.Equal(away_matches_per_team, matches.Count(m => m.TeamAwayIndex == 4));
            Assert.Equal(away_matches_per_team, matches.Count(m => m.TeamAwayIndex == 5));
            Assert.Equal(away_matches_per_team, matches.Count(m => m.TeamAwayIndex == 6));
            Assert.Equal(away_matches_per_team, matches.Count(m => m.TeamAwayIndex == 7));
            Assert.Equal(away_matches_per_team, matches.Count(m => m.TeamAwayIndex == 8));
		}
	}
}
