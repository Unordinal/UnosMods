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
        public static float TotalRarity
        {
            get
            {
                int totalKeys = TeamTotalKeys();
                return 80 + (totalKeys * 20) + Mathf.Pow(totalKeys, 2);
            }
        }
        public static float CommonRarity => 80 / TotalRarity;
        public static float UncommonRarity => (TeamTotalKeys() * 20) / TotalRarity;
        public static float LegendaryRarity => Mathf.Pow(TeamTotalKeys(), 2) / TotalRarity;

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

        private static int TeamTotalKeys()
        {
            return ContextProvider.GetAllPlayerBodies().Sum(body => body?.GetPickupCount(TreasureCache) ?? 0);
        }
    }
}
