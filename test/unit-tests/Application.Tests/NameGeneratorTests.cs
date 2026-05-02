using GalaxyFootball.Application.Utils;
using Xunit;

namespace Application.Tests
{
    public class NameGeneratorTests
    {
        [Fact]
        public void Generates_NonEmpty_Name_And_ClubName()
        {
            var generator = new NameGenerator();
            var name = generator.GetName();
            var club = generator.GetClubName();

            Assert.False(string.IsNullOrWhiteSpace(name));
            Assert.False(string.IsNullOrWhiteSpace(club));
        }

        [Fact]
        public void Generates_ClubName_Based_On_Planet()
        {
            var generator = new NameGenerator();
            var planet = generator.GetPlanetName();
            var club   = generator.GetClubName(planet);

            Assert.False(string.IsNullOrWhiteSpace(planet));
            Assert.False(string.IsNullOrWhiteSpace(club));
            Assert.Contains(planet,club);
        }

    }
}
