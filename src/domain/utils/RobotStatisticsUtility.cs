using GalaxyFootball.Domain.Entities;

namespace GalaxyFootball.Domain.Utils
{
    public class RobotStatisticsUtility
    {
        static public void Reset(RobotStatistics stats)
        {
            stats.Assists       = 0;
            stats.Fouls         = 0;
            stats.GamePlayed    = 0;
            stats.Goals         = 0;
            stats.Interceptions = 0;
            stats.RedCards      = 0;
            stats.YellowCards   = 0;
        }

        static public void Add(RobotStatistics stats, RobotStatistics stats_to_add)
        {
            stats.Assists       += stats_to_add.Assists;
            stats.Fouls         += stats_to_add.Fouls;
            stats.GamePlayed    += stats_to_add.GamePlayed;
            stats.Goals         += stats_to_add.Goals;
            stats.Interceptions += stats_to_add.Interceptions;
            stats.RedCards      += stats_to_add.RedCards;
            stats.YellowCards   += stats_to_add.YellowCards;
        }

    }
}
