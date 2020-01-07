using System;
using System.Collections.Generic;
using System.Text;

namespace PickupStatsAPI.Stats
{
    public class FloatStat : Stat
    {
        public float BaseValue { get; set; }
        public float StackValue { get; set; }
    }
}
