using GalaxyFootball.Application.Interfaces;

namespace GalaxyFootball.Application.MatchEngine
{
    public class SimpleMatchEngine : IMatchEngine
    {
        private MatchEngineTeam? m_homeTeam;
        private MatchEngineTeam? m_awayTeam;
        private int m_duration;
        private bool m_extraTimeDuration;
        private bool m_penaltyShootoutRounds;

        public void SetMatchInput(MatchEngineTeam home, MatchEngineTeam away)
        {
            m_homeTeam = home;
            m_awayTeam = away;
        }

        public void SetMatchRules(int duration, bool extraTimeDuration, bool penaltyShootoutRounds)
        {
            m_duration = duration;
            m_extraTimeDuration = extraTimeDuration;
            m_penaltyShootoutRounds = penaltyShootoutRounds;
        }

        public MatchEngineOutput SimulateMatch()
        {
            if ( m_awayTeam is null || m_homeTeam is null)
            {
                throw new InvalidOperationException("Match teams are not set.");
            }
            
            // Placeholder logic for simulating a match
            var random = new Random();
            return new MatchEngineOutput
            {
                HomeScore = random.Next(0, 5),
                AwayScore = random.Next(0, 5)
            };
        }
    }
}