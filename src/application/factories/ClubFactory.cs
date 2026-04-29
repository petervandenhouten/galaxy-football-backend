using System;
using GalaxyFootball.Domain.Entities;
using GalaxyFootball.Application.Utils;

namespace GalaxyFootball.Application.Factories
{
    public static class ClubFactory
    {
        private static readonly Random _random = new Random();
        private static readonly string[] Colors = new[]
        {
            "Red", "Blue", "Green", "Yellow", "Orange", "Purple", "Black", "White", "Gray", "Cyan", "Magenta", "Maroon", "Navy", "Teal", "Lime", "Olive", "Silver", "Gold"
        };

        public static Club CreateClub()
        {
            var planetName = new NameGenerator().GetPlanetName();

            var nameGen = new NameGenerator();
            var club = new Club
            {
                Id = Guid.NewGuid(),
                Name = nameGen.GetClubName(planetName),
                PlanetName = planetName, 
                FoundationYear = 1,
                PrimaryColor = GetRandomColor(),
                SecondaryColor = GetRandomColor(),
                TertiaryColor = GetRandomColor(),
                UniformStyle = $"Style{_random.Next(1, 10)}",
                LogoReference = $"logo_{_random.Next(1, 1000)}.png",
                Balance = 100000,
                LastIncome = 0,
                LastExpenses = 0,
                FanCount = 1000,
                FanHappiness = 0,
                FanExpectations = 0,
                ClubRankingPoints = 0,
                SeasonGamesWon = 0,
                SeasonGamesLost = 0,
                SeasonGamesDrawn = 0,
                SeasonGoalsScored = 0,
                SeasonGoalsConceded = 0,
                WinningStreak = 0,
                LosingStreak = 0,
                AllTimeGamesWon = 0,
                AllTimeGamesLost = 0,
                AllTimeGamesDrawn = 0,
                AllTimeGoalsScored = 0,
                AllTimeGoalsConceded = 0
            };
            return club;
        }

        private static string GetRandomColor()
        {
            return Colors[_random.Next(Colors.Length)];
        }
    }
}
