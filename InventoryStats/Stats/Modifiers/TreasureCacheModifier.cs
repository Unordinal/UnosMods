using System.Linq;
using UnityEngine;
using RoR2;
using Unordinal.InventoryStats.Formatters;
using Unordinal.InventoryStats.Providers;
using static Unordinal.StyleCatalog;

namespace Unordinal.InventoryStats.Stats.Modifiers
{
    public class TreasureCacheModifier : StatModifier
    {
        public static PickupIndex TreasureCache { get; } = PickupCatalog.FindPickupIndex(ItemIndex.TreasureCache);

        public static TreasureCacheModifier Instance { get; } = new TreasureCacheModifier();

        private TreasureCacheModifier()
        {
            ModifyingIndices = new[] { TreasureCache };
            Formula = ModifierFormulas.TreasureCache;
            Formatter = new PercentageModifierFormatter(
                name: "Teammates' Keys",
                modifyingIndices: ModifyingIndices,
                color: StyleIndex.cIsUtility.ToHex());
        }

        public static float TotalRarity(float keyCount)
        {
            return 80 + (keyCount * 20) + Mathf.Pow(keyCount, 2);
        }

        public static float CommonRarity(float keyCount)
        {
            return 80 / TotalRarity(keyCount);
        }

        public static float UncommonRarity(float keyCount)
        {
            return (keyCount * 20) / TotalRarity(keyCount);
        }

        public static float LegendaryRarity(float keyCount)
        {
            return Mathf.Pow(keyCount, 2) / TotalRarity(keyCount);
        }

        public static int GetCount()
        {
            return ContextProvider.GetPickupCount(Instance.ModifyingIndices);
        }

        public static int GetTeamCount()
        {
            return ContextProvider.GetPickupCount(ContextProvider.GetAllPlayerBodies(), Instance.ModifyingIndices);
        }
        
        public static int GetOnlyTeamCount()
        {
            return ContextProvider.GetPickupCount(ContextProvider.GetAllPlayerBodiesExcept(), Instance.ModifyingIndices);
        }
    }
}
