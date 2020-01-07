using System;
using System.Collections.Generic;
using System.Text;

namespace PickupStatsAPI.Stats
{
    public static class StatFormulas
    {
        public delegate object StatFormula(int itemCount, float stackValue, float baseValue);
    }
}
