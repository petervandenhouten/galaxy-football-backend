using GalaxyFootball.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace GalaxyFootball.Application.Factories
{
    public class CalendarFactory
    {
        private readonly ILogger<CalendarFactory>? m_logger;
        // Move to global config
        const int MaxDay = 7;
        const int MaxWeek = 52;
        const int MatchDay = 7;
        const int CupDay = 3;

        public CalendarFactory(ILoggerFactory? loggerFactory)
        {
            if (loggerFactory is not null)
            {
                m_logger = loggerFactory.CreateLogger<CalendarFactory>();
            }
        }

        public List<Calendar> CreateCalender(Game game, GameParameters parameters, int totalNumberOfClubs)
        {
            var calendar = new List<Calendar>();
            
            // create new year
            int numberOfLeagueRounds = (game.NumberOfTeamsInLeague - 1) * 2; 
            int numberOfCupRounds = CalculateNumberOfCupRounds(totalNumberOfClubs);

            int remainingLeagueRounds    = numberOfLeagueRounds;
            int remainingCupRounds       = numberOfCupRounds;
            int remainingFriendlyMatches = parameters.FriendlyMatches;
            int leagueRound     = 1;
            int cupRound        = 1;
            int dayIndex        = 1;

            int specialDays = 2; // FastestPlayerEvent, PenaltyCupEvent
            int preseasonDays = parameters.PreSeasonDays+1; // draft event
            int totalDays = numberOfLeagueRounds + numberOfCupRounds + parameters.FriendlyMatches + specialDays;

            bool penalty_event_planned        = false;
            bool fastest_player_event_planned = false;

            for (int i=0; i<preseasonDays; i++)
            {
                var entry = new Calendar 
                { 
                    DayIndex = dayIndex,
                    CompetitionId = 0,
                    CompetitionRound = 0,
                };

                if ( i == parameters.PreSeasonDays/2)
                {
                    entry.DayType = CalendarDayType.DraftEvent;
                }
                else
                {
                    entry.DayType = CalendarDayType.Preseason;
                }
                calendar.Add(entry);
                dayIndex++;
            }

            int j=0;
            while(j<totalDays)
            {
                var entry = new Calendar
                { 
                    DayIndex = dayIndex,
                    CompetitionId    = 0,
                    CompetitionRound = 0,
                };

                if ( !fastest_player_event_planned && dayIndex >= totalDays * 3/5)
                {
                    entry.DayType = CalendarDayType.FastestPlayerEvent;
                    fastest_player_event_planned = true;
                }
                else if ( !penalty_event_planned && dayIndex >= totalDays * 4/5 )
                {
                    entry.DayType = CalendarDayType.PenaltyCupEvent;
                    penalty_event_planned = true;
                }
                else if ( remainingFriendlyMatches > 0 )
                {
                    if ( remainingFriendlyMatches == 3 )
                    {
                        entry.DayType = CalendarDayType.FriendlyMatch;
                        remainingFriendlyMatches--;
                    }
                    else if ( remainingFriendlyMatches == 2 )
                    {
                        entry.DayType = CalendarDayType.FriendlyMatch;
                        remainingFriendlyMatches--;
                    }
                    else if ( remainingFriendlyMatches == 1 )
                    {
                        entry.DayType = CalendarDayType.FriendlyMatch;
                        remainingFriendlyMatches--;
                    }
                }
                else
                {
                    if ( remainingLeagueRounds > 0 && remainingLeagueRounds > remainingCupRounds )
                    {
                        entry.DayType = CalendarDayType.LeagueMatch;
                        entry.CompetitionRound = leagueRound++;
                        remainingLeagueRounds--;
                    }
                    else if ( remainingCupRounds > 0)
                    {
                        entry.DayType = CalendarDayType.CupMatch;
                        entry.CompetitionRound = cupRound++;
                        remainingCupRounds--;
                    }
                }

                calendar.Add(entry);
                dayIndex++;

                for(int i=0; i<parameters.DaysBetweenMatches; i++)
                {
                    var breakEntry = new Calendar
                    {
                        DayIndex = dayIndex,
                        CompetitionId = 0,
                        CompetitionRound = 0,
                        DayType = CalendarDayType.Idle
                    };
                    calendar.Add(breakEntry);
                    dayIndex++;
                }
                j++;
            }

            for (int i=0; i<parameters.AfterSeasonDays; i++)
            {
                var entry = new Calendar
                {
                    DayIndex = dayIndex,
                    CompetitionId = 0,
                    CompetitionRound = 0,
                    DayType = CalendarDayType.AfterSeason
                };
                calendar.Add(entry);
                dayIndex++;
            }

            m_logger?.LogInformation("Calendar created for year={0}. [Number of days={1}, LeagueRounds={2}, CupRounds={3}]",
                                     game.Year, totalDays, numberOfLeagueRounds, numberOfCupRounds);

            return calendar;
        }

        private int CalculateNumberOfCupRounds(int totalNumberOfClubs)
        {
            if (totalNumberOfClubs > 0)
            {
                int nbTeamsInFirstFullRound = (int)Math.Pow(2, (int)Math.Floor(Math.Log(totalNumberOfClubs) / Math.Log(2)));
                int numberOfRounds          = (int)Math.Floor(Math.Log(nbTeamsInFirstFullRound) / Math.Log(2));
                int nbRemainingClub         = totalNumberOfClubs - nbTeamsInFirstFullRound;

                if (nbRemainingClub >= 2) numberOfRounds++;

                return numberOfRounds;
            }
            return 0;
        }
    }
}