using System.Linq;
using UnityEngine;
using Unordinal.InventoryStats.Providers;

namespace Unordinal.InventoryStats.Stats.Modifiers
{
    public static class ModifierFormulas
    {
        public delegate float ModifierFormula(float stat);

        public static float Luck(float procChance)
        {
            return 1 - Mathf.Pow(1 - procChance, ContextProvider.GetPickupCount(LuckModifier.Instance.ModifyingIndices) + 1); // https://riskofrain2.fandom.com/wiki/57_Leaf_Clover
        }
        
        public static float TreasureCache(float stat)
        {
            return ContextProvider.GetAllPlayerBodiesExcept()
                .Sum(body => body.GetPickupCount(TreasureCacheModifier.Instance.ModifyingIndices)) + stat;
        }

        public static float DualBands(float combinedProc)
        {
            int hasFire = ContextProvider.GetLocalBody().GetPickupCount(DualBandsModifier.FireBand) > 0 ? 1 : 0;
            int hasIce = ContextProvider.GetLocalBody().GetPickupCount(DualBandsModifier.IceBand) > 0 ? 1 : 0;

            return 1 - Mathf.Pow(1 - combinedProc, hasFire + hasIce);
        }

        public static float TPHealingNova(float stat)
        {
            return ContextProvider.GetAllPlayerBodiesExcept()
                .Sum(body => body.GetPickupCount(TPHealingNovaModifier.Instance.ModifyingIndices)) + stat;
        }
    }
}
