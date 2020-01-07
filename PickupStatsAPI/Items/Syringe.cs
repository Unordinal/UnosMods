using System;
using System.Collections.Generic;
using System.Text;
using PickupStatsAPI.Stats;

namespace PickupStatsAPI.Items
{
    public class Syringe : Item
    {
        public FloatStat AttackSpeed => (FloatStat)stats[0];

        public override void ApplyHook()
        {

        }
    }
}
