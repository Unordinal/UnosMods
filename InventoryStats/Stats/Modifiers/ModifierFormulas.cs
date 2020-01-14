using System.Linq;
using UnityEngine;
using Unordinal.InventoryStats.Providers;

namespace Unordinal.InventoryStats.Stats.Modifiers
{
    public static class ModifierFormulas
    {
        public delegate float ModifierFormula(float stat);

        public static float Luck(float baseChance)
        {
            return 1 - Mathf.Pow(1 - baseChance, LuckModifier.GetCount() + 1); // https://riskofrain2.fandom.com/wiki/57_Leaf_Clover
        }
        
        public static float TreasureCache(float count)
        {
            return TreasureCacheModifier.GetOnlyTeamCount() + count;
        }

        public static float DualBands(float combinedProc)
        {
            bool hasFire = ContextProvider.GetPickupCount(DualBandsModifier.FireBand) > 0;
            bool hasIce = ContextProvider.GetPickupCount(DualBandsModifier.IceBand) > 0;

            return (hasFire && hasIce) ? 1 - Mathf.Pow(1 - combinedProc, 2) : 0;
        }

        public static float TPHealingNova(float count)
        {
            return TPHealingNovaModifier.GetOnlyTeamCount() + count;
        }
    }
}
