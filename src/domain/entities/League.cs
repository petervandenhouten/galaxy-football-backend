using System;
using System.Collections.Generic;

namespace GalaxyFootball.Domain.Entities
{
    /// <summary>
    /// Represents a competition tier. A league has a level and a number. It is linked to a number of teams.
    /// </summary>
    public class League
    {
        /// <summary>
        /// Unique identifier for the league.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The tier/level of the league (e.g., 1 for top division).
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// The number of the league within its level (e.g., 1, 2, 3 for parallel leagues at the same level).
        /// </summary>
        public int Number { get; set; }
    }
}
