using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Networking;
using RoR2;
using BepInEx;
using BepInEx.Logging;
using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace AIBlacklister
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(R2API.R2API.PluginGUID)]
    public class PluginEntry : BaseUnityPlugin
    {
        public const string PluginName = "AI Blacklister";
        public const string PluginGUID = "com.unordinal.aiblacklister";
        public const string PluginVersion = "1.0.0";
        public static new ManualLogSource Logger { get; private set; }

        public void Awake()
        {
            Logger = base.Logger;
            PluginConfig.Init(Config);
            SetHooks();
        }

        public void OnDestroy()
        {
            UnsetHooks();
        }

        public static void SetHooks()
        {
            On.RoR2.ItemCatalog.Init += ItemCatalog_Init;
            IL.RoR2.ScavengerItemGranter.Start += ScavengerItemGranter_Start;
        }

        public static void UnsetHooks()
        {
            On.RoR2.ItemCatalog.Init -= ItemCatalog_Init;
            IL.RoR2.ScavengerItemGranter.Start -= ScavengerItemGranter_Start;
        }

        public static void UpdateAIItemBlacklist()
        {
            if (PluginConfig.AIItemBlacklist is null)
            {
                Logger.LogError("AI item blacklist was not initialized correctly! Aborting.");
                return;
            }

            for (ItemIndex i = 0; (int)i < ItemCatalog.itemCount; i++)
            {
                ItemDef itemDef = ItemCatalog.GetItemDef(i);
                if (itemDef is null)
                {
                    Logger.LogWarning($"ItemDef for {i} was null - skipping.");
                    continue;
                }

                if (itemDef.DoesNotContainTag(ItemTag.AIBlacklist) && PluginConfig.AIItemBlacklist.Contains(itemDef.itemIndex))
                {
                    itemDef.tags = itemDef.tags.Add(ItemTag.AIBlacklist);
                    Logger.LogInfo($"Added item '{i}' to AI blacklist.");
                }
                else if (itemDef.ContainsTag(ItemTag.AIBlacklist) && !PluginConfig.AIItemBlacklist.Contains(itemDef.itemIndex))
                {
                    itemDef.tags = itemDef.tags.Remove(ItemTag.AIBlacklist);
                    Logger.LogInfo($"Removed item '{i}' from AI blacklist.");
                }
            }
        }

        public static IEnumerable<PickupIndex> EquipmentToPickupIndices(IEnumerable<EquipmentIndex> equipIndices)
        {
            foreach (var index in equipIndices)
            {
                yield return PickupCatalog.FindPickupIndex(index);
            }
        }

        private static EquipmentIndex GetRandomNonBlacklistEquipment()
        {
            IEnumerable<PickupIndex> blacklistEquips = EquipmentToPickupIndices(PluginConfig.AIEquipBlacklist);
            List<PickupIndex> equipsExceptBlacklist = Run.instance?.availableEquipmentDropList.Except(blacklistEquips).ToList();

            if (equipsExceptBlacklist is null)
            {
                Logger.LogError("Run equipment excepting blacklist was null!");
                return EquipmentIndex.None;
            }

            PickupDef randomEquip = PickupCatalog.GetPickupDef(equipsExceptBlacklist[UnityEngine.Random.Range(0, equipsExceptBlacklist.Count)]);
            return randomEquip.equipmentIndex;
        }

        private static void GiveRandomNonBlacklistEquipment(Inventory inv)
        {
            Logger.LogDebug("Giving random non-blacklisted equipment to inventory.");

            EquipmentIndex equip = GetRandomNonBlacklistEquipment();
            if (equip != EquipmentIndex.None)
                inv.SetEquipmentIndex(equip);
            else
                Logger.LogWarning("Could not get random equipment index.");
        }

        private static void ItemCatalog_Init(On.RoR2.ItemCatalog.orig_Init orig)
        {
            orig();
            UpdateAIItemBlacklist();
        }

        private static void ScavengerItemGranter_Start(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            bool success =
                c.TryGotoNext(
                    i => i.MatchLdloc(0),
                    i => i.MatchCallvirt<Inventory>("GiveRandomEquipment")
                );

            if (success)
            {
                c.RemoveRange(2);
                c.Emit(OpCodes.Ldloc_0);
                c.EmitDelegate<Action<Inventory>>((inv) =>
                {
                    if (NetworkServer.active)
                    {
                        GiveRandomNonBlacklistEquipment(inv);
                    }
                });
            }
            else
            {
                Logger.LogError($"Unable to apply IL hook to RoR2.ScavengerItemGranter.Start. Equipment blacklist will not be in effect.");
                return;
            }
        }
    }
}
