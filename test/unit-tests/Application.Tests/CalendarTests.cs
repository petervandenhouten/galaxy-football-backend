using GalaxyFootball.Application.Factories;
using GalaxyFootball.Domain.Entities;
using Xunit;

namespace Application.Tests
{
	public class CalendarTests
	{
		[Fact]
		public void CreateCalender_GeneratesExpectedDays()
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

			int totalNumberOfClubs = 16;
			int nr_league_rounds = (game.NumberOfTeamsInLeague-1) * 2;
			int nr_cup_rounds    = 4; // 16,8,4,2
			// Act
			var calendar = factory.CreateCalender(game, param, totalNumberOfClubs);

			// Assert
			Assert.NotNull(calendar);
			Assert.True(calendar.Count > 0);
			Assert.Equal(1, calendar[0].DayIndex); // First day index should be 1
			Assert.Contains(calendar, d => d.DayType == CalendarDayType.LeagueMatch);
			Assert.Contains(calendar, d => d.DayType == CalendarDayType.CupMatch);
			Assert.Contains(calendar, d => d.DayType == CalendarDayType.Preseason);
			Assert.Contains(calendar, d => d.DayType == CalendarDayType.AfterSeason);
			Assert.Contains(calendar, d => d.DayType == CalendarDayType.DraftEvent);
			Assert.Contains(calendar, d => d.DayType == CalendarDayType.FastestPlayerEvent);
			Assert.Contains(calendar, d => d.DayType == CalendarDayType.PenaltyCupEvent);

			Assert.Equal(nr_league_rounds, calendar.Count(d => d.DayType==CalendarDayType.LeagueMatch));
			Assert.Equal(nr_cup_rounds, calendar.Count(d => d.DayType==CalendarDayType.CupMatch));
			Assert.Equal(param.FriendlyMatches, calendar.Count(d => d.DayType==CalendarDayType.FriendlyMatch));
			Assert.Equal(param.PreSeasonDays, calendar.Count(d => d.DayType==CalendarDayType.Preseason));
			Assert.Equal(param.AfterSeasonDays, calendar.Count(d => d.DayType==CalendarDayType.AfterSeason));
			Assert.Equal(1, calendar.Count(d => d.DayType==CalendarDayType.DraftEvent));
			Assert.Equal(1, calendar.Count(d => d.DayType==CalendarDayType.FastestPlayerEvent));
			Assert.Equal(1, calendar.Count(d => d.DayType==CalendarDayType.PenaltyCupEvent));
		}
	}
}
