using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RoR2;
using Unordinal.InventoryStats.Stats;
using static RoR2.ColorCatalog;
using static Unordinal.InventoryStats.Stats.PickupStatsDefinitions;

namespace Unordinal.InventoryStats.Providers
{
    public static class PickupStatsProvider
    {
        public static string GetStatsForPickup(PickupIndex pickup, int count = 1)
        {
            PickupDef pickupDef = PickupCatalog.GetPickupDef(pickup);
            List<Stat> pickupStats = null;

            if (pickupDef.itemIndex != ItemIndex.None)
            {
                ItemIndex index = pickupDef.itemIndex;
                pickupStats = ItemStatDefs.ContainsKey(index) ? ItemStatDefs[index] : null;
            }
            else if (pickupDef.equipmentIndex != EquipmentIndex.None)
            {
                EquipmentIndex index = pickupDef.equipmentIndex;
                pickupStats = EquipmentStatDefs.ContainsKey(index) ? EquipmentStatDefs[index] : null;
            }

            if (pickupStats is null) return "Not Implemented!".Colorize(ColorIndex.Error.ToHex()); // If we didn't find the index in the lists it's probably a new item we haven't implemented yet.

            StringBuilder statTextBuilder = new StringBuilder("</color>"); // Removes extraneous styling.
            foreach (var stat in pickupStats)
            {
                bool shouldSkipStat =                                               // If all of these are true, we'll skip the current stat.
                    InventoryStats.HideNonStackingStats                             // - User preference is to hide non-stacking stats.
                    && !stat.Stacks                                                 // - The stat doesn't stack.
                    && !ContextProvider.PlayerHasModifyingItems(stat.Modifiers);    // - Player doesn't have any items that modify the stat (ex: clover modifying a proc. chance that doesn't stack should still show)
                
                if (shouldSkipStat) // If all of the above are true, skip the current stat.
                    continue;
                
                if (stat.Formula != null)
                {
                    statTextBuilder.Append($"<align=left>{stat.Text}"); // Stat name.
                    
                    // Gets the current stat's value.
                    float baseStatValue = stat.Formula(count);
                    float finalStatValue = baseStatValue;

                    // Takes the modifiers (clover, lepton, etc) and gets and adds their values to the final stat value and formats them for later adding to the final stat text.
                    StringBuilder modTextBuilder = new StringBuilder();
                    foreach (var mod in stat.Modifiers)
                    {
                        if (mod.Formula is null) continue;

                        float modValue = mod.Formula(baseStatValue);
                        float modDiff = modValue - baseStatValue;
                        if (Math.Round(modDiff, 3) > 0)
                        {
                            finalStatValue += modDiff;
                            modTextBuilder.AppendLine(mod.Format(modDiff));
                        }
                    }
                    
                    statTextBuilder.Append($": {stat.Format(finalStatValue)}");
                    statTextBuilder.Append($"{modTextBuilder.ToString().TrimEnd()}");
                }
                else // if the stat doesn't have a formula (only text), simply append that text and color it with a special color.
                {
                    statTextBuilder.Append($"<align=left>{stat.Text.Colorize(ColorNote)}");
                }

                statTextBuilder.AppendLine();
            }
            // Trims extraneous whitespace characters - we don't want a hanging newline taking up space and this was cleaner than checking whether or not to add a newline (especially with the hide non-stacking logic)
            statTextBuilder.Trim();

            if (pickupDef.itemIndex != ItemIndex.None)
                statTextBuilder.Append($"<br></style><align=right>({count} stack{(count > 1 ? "s" : "")})"); // Appends the item stack count to the end and aligns it to the right (appending an 's' for plural count if needed)

            return statTextBuilder.ToString();
        }
    }
}
