using System;
using System.Collections.Generic;
using System.Text;
using static PickupStatsAPI.Stats.StatFormulas;

namespace PickupStatsAPI.Stats
{
    public abstract class Stat
    {
        public StatFormula Formula { get; set; }
    }
}
