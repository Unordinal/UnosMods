using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RoR2;

namespace Unordinal.ItemCountDisplay
{
    public static class Utils
    {
        public static Dictionary<ItemTier, int> GetTierCounts(Inventory inv)
        {
            var tierMap = new Dictionary<ItemTier, int>();
            foreach (var tier in Enum.GetValues(typeof(ItemTier)).Cast<ItemTier>())
            {
                tierMap.Add(tier, inv.GetTotalItemCountOfTier(tier));
            }
            return tierMap;
        }

        public static string TierToHexString(ItemTier tier)
        {
            switch (tier)
            {
                case ItemTier.Tier1:
                    return ColorCatalog.GetColorHexString(ColorCatalog.ColorIndex.Tier1Item);
                case ItemTier.Tier2:
                    return ColorCatalog.GetColorHexString(ColorCatalog.ColorIndex.Tier2Item);
                case ItemTier.Tier3:
                    return ColorCatalog.GetColorHexString(ColorCatalog.ColorIndex.Tier3Item);
                case ItemTier.Lunar:
                    return ColorCatalog.GetColorHexString(ColorCatalog.ColorIndex.LunarItem);
                case ItemTier.Boss:
                    return ColorCatalog.GetColorHexString(ColorCatalog.ColorIndex.BossItem);
                case ItemTier.NoTier:
                    return ColorCatalog.GetColorHexString(ColorCatalog.ColorIndex.Unaffordable);
                default:
                    return "FFF";
            }
        }

        public static string Colorize(this string str, string colorHex)
        {
            return $"<color=#{colorHex}>{str}</color>";
        }
    }
}
