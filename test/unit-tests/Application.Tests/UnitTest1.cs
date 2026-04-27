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
    }
}
