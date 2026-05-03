namespace GalaxyFootball.Application.Interfaces
{
    public class MatchEngineTeam
    {
        public string Name = string.Empty;

        //public List<Player> Players = new List<Player>();
    }

    public class MatchEngineOutput
    {
        public int HomeScore;
        public int AwayScore;

        //public List<Player> Players = new List<Player>();
    }

    /// <summary>
    /// Interface for match engine operations.
    /// </summary>
    public interface IMatchEngine
    {

        void SetMatchInput(MatchEngineTeam home, MatchEngineTeam awaw);
        void SetMatchRules(int duration, bool extraTimeDuration, bool penaltyShootoutRounds);
        MatchEngineOutput SimulateMatch();

    }
}
