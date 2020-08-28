using BepInEx.Configuration;
using RoR2;
using System;
using System.Linq;

namespace AIBlacklister
{
    public static class AIBlacklisterConfig
    {
        private static readonly ItemIndex[] defaultItemBlacklist =
        {
            ItemIndex.Dagger, ItemIndex.FallBoots, ItemIndex.Feather, ItemIndex.Mushroom, ItemIndex.WardOnLevel, ItemIndex.StunChanceOnHit,
            ItemIndex.Firework, ItemIndex.TreasureCache, ItemIndex.BossDamageBonus, ItemIndex.HeadHunter, ItemIndex.KillEliteFrenzy,
            ItemIndex.ExecuteLowHealthElite, ItemIndex.TPHealingNova, ItemIndex.LunarUtilityReplacement, ItemIndex.Thorns, 
            ItemIndex.LunarPrimaryReplacement, ItemIndex.Squid, ItemIndex.FocusConvergence, ItemIndex.MonstersOnShrineUse,
            ItemIndex.ShockNearby
        };
        private static readonly EquipmentIndex[] defaultEquipBlacklist =
        {
            EquipmentIndex.Blackhole, EquipmentIndex.CommandMissile, EquipmentIndex.OrbitalLaser, EquipmentIndex.Lightning, EquipmentIndex.Scanner, 
            EquipmentIndex.Gateway
        };

        private static ItemIndex[] aiItemBlacklist;
        private static EquipmentIndex[] aiEquipBlacklist;

        public static ItemIndex[] AIItemBlacklist
        {
            get
            {
                if (AIItemBlacklistEntry is null) return null;

                if (aiItemBlacklist is null)
                {
                    string[] blacklistIndices = 
                        AIItemBlacklistEntry.Value
                        .Split(',')
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Select(x => x.Trim())
                        .Distinct().ToArray();

                    aiItemBlacklist = Array.ConvertAll(blacklistIndices, StringToItemIndex).Where(x => x != ItemIndex.None).ToArray();
                }
                return aiItemBlacklist;
            }
        }
        public static EquipmentIndex[] AIEquipBlacklist
        {
            get
            {
                if (AIEquipBlacklistEntry is null) return null;

                if (aiEquipBlacklist is null)
                {
                    string[] blacklistIndices =
                        AIEquipBlacklistEntry.Value
                        .Split(',')
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Select(x => x.Trim())
                        .Distinct().ToArray();

                    aiEquipBlacklist = Array.ConvertAll(blacklistIndices, StringToEquipmentIndex).Where(x => x != EquipmentIndex.None).ToArray();
                }
                return aiEquipBlacklist;
            }
        }

        public static ConfigEntry<string> AIItemBlacklistEntry { get; set; }
        public static ConfigEntry<string> AIEquipBlacklistEntry { get; set; }

        public static void Init(ConfigFile config)
        {
            AIItemBlacklistEntry = config.Bind(
                "AIBlacklister",
                "ItemBlacklist",
                string.Join(", ", defaultItemBlacklist),
                "The list of item indices that the AI cannot receive. The default values include the vanilla-blacklisted items.\n" +
                "See 'https://github.com/risk-of-thunder/R2Wiki/wiki/Item-&-Equipment-IDs-and-Names' for a list of valid IDs. (Use the code name.)");

            AIEquipBlacklistEntry = config.Bind(
                "AIBlacklister",
                "EquipmentBlacklist",
                string.Join(", ", defaultEquipBlacklist),
                "The list of equipment indices that the AI cannot receive (currently only the Scavenger). The default values include the vanilla-blacklisted items.\n" +
                "See 'https://github.com/risk-of-thunder/R2Wiki/wiki/Item-&-Equipment-IDs-and-Names' for a list of valid IDs. (Use the code name.)");
        }

        private static ItemIndex StringToItemIndex(string indexStr)
        {
            ItemIndex index = ItemCatalog.FindItemIndex(indexStr);
            if (index == ItemIndex.None)
                AIBlacklister.Logger.LogWarning($"{indexStr} is not a valid ItemIndex.");

            return index;
        }
        
        private static EquipmentIndex StringToEquipmentIndex(string indexStr)
        {
            EquipmentIndex index = EquipmentCatalog.FindEquipmentIndex(indexStr);
            if (index == EquipmentIndex.None)
                AIBlacklister.Logger.LogWarning($"{indexStr} is not a valid EquipmentIndex.");

            return index;
        }
    }
}
