using GalaxyFootball.Domain.Entities;

namespace GalaxyFootball.Application.Factories
{
    /// <summary>
    /// Factory for creating AutoCoach objects.
    /// </summary>
    public class AutoCoachFactory
    {
        /// <summary>
        /// Instantiates an AutoCoach object.
        /// </summary>
        /// <returns>The created AutoCoach object.</returns>
        static public AutoCoach CreateAutoCoach()
        {
            var nameGen = new GalaxyFootball.Application.Utils.NameGenerator();
            var autoCoach = new AutoCoach
            {
                Id = Guid.NewGuid(),
                FirstName = nameGen.GetFirstName(),
                LastName = nameGen.GetLastName(),
                PreferredLineupPickStrategy = GetRandomEnum<PreferredLineupPickStrategy>(),
                PreferredLineUpFormation = GetRandomEnum<PreferredLineUpFormation>(),
                LineupFormationSelectionMethod = GetRandomEnum<LineupFormationSelectionMethod>(),
                LineupPickStrategySelectionMethod = GetRandomEnum<LineupPickStrategySelectionMethod>()
            };
            return autoCoach;
        }

        private static TEnum GetRandomEnum<TEnum>() where TEnum : Enum
        {
            var values = Enum.GetValues(typeof(TEnum));
            if (values.Length == 0)
                throw new InvalidOperationException($"Enum '{typeof(TEnum).Name}' has no values.");
            var value = values.GetValue(new Random().Next(values.Length));
            if (value is null)
                throw new InvalidOperationException($"Failed to get a random value for enum '{typeof(TEnum).Name}'.");
            return (TEnum)value;
        }
    }
}
