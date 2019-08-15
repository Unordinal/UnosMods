using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RoR2;
using UnityEngine;
using UnosMods.TabItemStats.AbstractModifiers;
using UnosUtilities;

namespace UnosMods.TabItemStats
{
    public static partial class PickupStatProvider
    {
        private static string PositiveColor { get { return StyleCatalog.GetColorHexString(StyleCatalog.StyleIndex.cIsHealing, true); } }
        private static string NeutralColor { get { return StyleCatalog.GetColorHexString(StyleCatalog.StyleIndex.cIsDamage, true); } }
        private static string NegativeColor { get { return StyleCatalog.GetColorHexString(StyleCatalog.StyleIndex.cIsHealth, true); } }
        private static string UtilityColor { get { return StyleCatalog.GetColorHexString(StyleCatalog.StyleIndex.cIsUtility, true); } }
        private static string ShrineColor { get { return StyleCatalog.GetColorHexString(StyleCatalog.StyleIndex.cShrine, true); } }
        private static string CooldownColor { get { return ColorCatalog.ColorIndex.Equipment.ToHex(); } }
        private static string DoesNotStackColor { get { return "#959595"; } }

        public static string ProvideItemStats(ItemIndex index, int count = 1)
        {
            var itemStatList = itemDefs.ContainsKey(index) ? itemDefs[index] : null;
            if (itemStatList == null)
                return "Not implemented".Color(ColorCatalog.ColorIndex.Error.ToHex());

            var fullStatText = "</color>"; // Remove extraneous style info
            if (itemStatList != null)
            {
                foreach (var stat in itemStatList)
                {
                    var statValue = stat.GetInitialStat(count) + stat.GetSubStats(count).Sum();
                    var statValueStr = stat.Format(statValue);

                    var subStats = stat.GetSubStats(count).Zip(stat.StatModifiers, Tuple.Create);
                    statValueStr += stat.FormatSubStats(count);
                    /*foreach (var subStat in subStats)
                    {
                        if (Math.Round(subStat.Item1, 3) > 0)
                            statValueStr += "\n" + subStat.Item2;
                    }*/

                    /*.Aggregate("", (s, tuple) =>
                    {
                        if (Math.Round(tuple.Item1, 3) > 0)
                            return "\n" + tuple.Item2.Format(tuple.Item1);
                        return "";
                    });*/

                    if (itemStatList.IndexOf(stat) == itemStatList.Count - 1)
                        fullStatText += $"<align=left>{stat.StatText}: {statValueStr}";
                    else
                        fullStatText += $"{stat.StatText}: {statValueStr}\n";
                }
                fullStatText += $"<br><align=right>({count} stacks)";
            }

            return fullStatText;
        }
        public static string ProvideEquipmentStats(EquipmentIndex index)
        {
            var equipmentStatList = equipmentDefs.ContainsKey(index) ? equipmentDefs[index] : null;

            if (equipmentStatList == null)
                return "Not implemented".Color(ColorCatalog.ColorIndex.Unaffordable.ToHex());

            var fullStatText = string.Empty;
            if (equipmentStatList != null)
            {
                foreach (var stat in equipmentStatList)
                {
                    var statValue = stat.GetInitialStat() + stat.GetSubStats().Sum();
                    var statValueStr = stat.Format(statValue);

                    statValueStr += stat.GetSubStats()
                        .Zip(stat.StatModifiers, Tuple.Create)
                        .Aggregate("", (s, tuple) =>
                        {
                            if (Math.Round(tuple.Item1, 3) > 0)
                                return "\n" + tuple.Item2.Format(tuple.Item1);
                            return "";
                        });

                    if (equipmentStatList.IndexOf(stat) == equipmentStatList.Count - 1)
                        fullStatText += $"<align=left>{stat.StatText}: {statValueStr}";
                    else
                        fullStatText += $"{stat.StatText}: {statValueStr}\n";
                }
            }

            return fullStatText;
        }
    }
}
