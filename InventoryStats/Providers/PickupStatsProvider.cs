using System.Collections.Generic;
using System.Linq;
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

            if (pickupStats is null) return "Not Implemented!".Colorize(ColorIndex.Error.ToHex()); // If we didn't find the index in the lists it's probably a new item we haven't added yet.

            string fullStatText = "</color>"; // Removes extraneous styling.
            foreach (var stat in pickupStats) // Loops through all the stats in the chosen pickup.
            {
                bool shouldSkipStat =                                               // If all of these are true, we'll skip the current stat.
                    InventoryStats.HideNonStackingStats                             // - User preference is to hide non-stacking stats.
                    && !stat.Stacks                                                 // - The stat doesn't stack.
                    && !ContextProvider.PlayerHasModifyingItems(stat.Modifiers);    // - Player doesn't have any items that modify the stat (ex: clover modifying a proc. chance that doesn't stack should still show)
                
                if (shouldSkipStat) // If the current stat doesn't stack and the user preference is to hide these, skip it.
                    continue;
                
                if (stat.Formula != null)
                {
                    fullStatText += $"<align=left>{stat.Text}"; // Stat name.
                    
                    // Adds the current stat value with stacks + the stat's modifiers together to get the final stat value.
                    // (eg: 2 items at 10% per stack = 20%, then has 1 clover so add 16% to get total of 36%)
                    float statValue = stat.Formula(count) + stat.GetSubStats(count).Sum(); 
                    string statValueStr = stat.Format(statValue);

                    // Takes the modifiers (clover, lepton, etc) and gets and appends their values to the stat value string.
                    //var subStatsValues = stat.GetSubStats(count).Zip(stat.Modifiers, Tuple.Create);
                    statValueStr += stat.FormatSubStats(count);
                    
                    fullStatText += $": {statValueStr}";
                }
                else // if the stat doesn't have a formula (only text), simply append that text and color it with a special color.
                {
                    fullStatText += $"<align=left>{stat.Text.Colorize(ColorNote)}";
                }

                fullStatText += "\n";
            }
            // Trims any extraneous whitespace characters - we don't want a hanging newline taking up space and this was cleaner than checking whether or not to add a newline (especially with the hide non-stacking logic)
            fullStatText = fullStatText.Trim();

            if (pickupDef.itemIndex != ItemIndex.None)
                fullStatText += $"<br></style><align=right>({count} stack{(count > 1 ? "s" : "")})"; // Appends the item stack count to the end and aligns it to the right (appending an 's' for plural count if needed)

            return fullStatText;
        }
    }
}
