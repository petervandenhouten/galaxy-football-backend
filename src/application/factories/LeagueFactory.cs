using GalaxyFootball.Domain.Entities;

namespace GalaxyFootball.Application.Factories
{
    /// <summary>
    /// Factory for creating League objects with teams assigned.
    /// </summary>
    public class LeagueFactory
    {
        /// <summary>
        /// Instantiates a League object, creates the specified number of teams, and assigns them to the league.
        /// </summary>
        /// <param name="numberOfTeams">Number of teams to create for the league.</param>
        /// <returns>Tuple containing the League and the list of created Teams.</returns>
        static public League CreateLeague(int level = 1, int number = 1)
        {
            var league = new League{ Id = Guid.NewGuid(), Level = level, Number = number };
            return league;
        }

        /// <summary>
        /// Returns the first available league level and number based on existing leagues.
        /// </summary>
        /// <param name="existingLeagues">A collection of existing leagues.</param>
        /// <returns>Tuple (level, number) for the next available league.</returns>
        public static (int level, int number) GetFirstAvailableLevelAndNumber(IEnumerable<League>? existingLeagues = null)
        {
            if (existingLeagues == null || !existingLeagues.Any())
                return (1, 1);

            // Find the highest level
            int maxLevel = existingLeagues.Max(l => l.Level);
            var leaguesAtMaxLevel = existingLeagues.Where(l => l.Level == maxLevel);
            var maxNumberOfLeagues = GetMaxNumberOfLeaguesOnLevel(maxLevel);

            // If the highest level is not full, return the next number on that level       
            if (leaguesAtMaxLevel.Count() < maxNumberOfLeagues)
            {
                int nextNumber = leaguesAtMaxLevel.Any() ? leaguesAtMaxLevel.Max(l => l.Number) + 1 : 1;
                return (maxLevel, nextNumber);
            }
            // Otherwise, move to the next level
            return (maxLevel + 1, 1);
        }

        public static int GetMaxNumberOfLeaguesOnLevel(int div_level)
        {
            int h = (int)Math.Pow(2, (double)(div_level - 1));
            return h;
        }

    }
}
