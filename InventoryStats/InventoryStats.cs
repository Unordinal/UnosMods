#define DEBUG
using System;
using RoR2;
using RoR2.UI;
using BepInEx;
using BepInEx.Configuration;
using R2API.Utils;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using Unordinal.InventoryStats.Providers;
using Unordinal.InventoryStats.Stats;

namespace Unordinal.InventoryStats
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(R2API.R2API.PluginGUID)]
    public class InventoryStats : BaseUnityPlugin
    {
        public const string PluginName = "InventoryStats";
        public const string PluginVersion = "1.0.0";
        public const string PluginGUID = "com.unordinal.inventorystats";
        internal static new BepInEx.Logging.ManualLogSource Logger { get; private set; }

        public static ConfigEntry<bool> EntryHideNonStackingStats { get; private set; }
        public static bool HideNonStackingStats => EntryHideNonStackingStats.Value;

        internal void Awake()
        {
            Logger = base.Logger;

            EntryHideNonStackingStats = Config.Bind("InventoryStats", "HideNonStackingStats", false, "If true, stats that don't stack will be hidden unless they're modified by something like 57 Leaf Clover.");

            RegisterHooks();
        }

        private void RegisterHooks()
        {
            On.RoR2.PickupCatalog.Init += PickupCatalog_Init;

            On.RoR2.UI.ItemIcon.SetItemIndex += ItemIcon_SetItemIndex;
            IL.RoR2.UI.EquipmentIcon.SetDisplayData += EquipmentIcon_SetDisplayData;
            //On.RoR2.UI.EquipmentIcon.SetDisplayData += EquipmentIcon_SetDisplayData;
            //On.RoR2.UI.ItemInventoryDisplay.UpdateDisplay += ItemInventoryDisplay_UpdateDisplay;
        }

        private void UnregisterHooks()
        {
            On.RoR2.PickupCatalog.Init -= PickupCatalog_Init;

            On.RoR2.UI.ItemIcon.SetItemIndex -= ItemIcon_SetItemIndex;
            IL.RoR2.UI.EquipmentIcon.SetDisplayData -= EquipmentIcon_SetDisplayData;
            //On.RoR2.UI.EquipmentIcon.SetDisplayData -= EquipmentIcon_SetDisplayData;
        }

        public void UpdateStats()
        {
            Logger.LogInfo("Updating inventory stats.");
            PickupStatsDefinitions.UpdateItemStatDefs();
            PickupStatsDefinitions.UpdateEquipmentStatDefs();
        }

        public void UpdatePickupTooltip(TooltipProvider tooltip, PickupIndex pickupIdx, int itemCount = 1)
        {
            PickupDef pickupDef = PickupCatalog.GetPickupDef(pickupIdx);

            string descToken;
            if (pickupDef.itemIndex != ItemIndex.None)
            {
                descToken = ItemCatalog.GetItemDef(pickupDef.itemIndex).descriptionToken;
            }
            else if (pickupDef.equipmentIndex != EquipmentIndex.None)
            {
                descToken = EquipmentCatalog.GetEquipmentDef(pickupDef.equipmentIndex).descriptionToken;
            }
            else return;

            string pickupDesc = Language.IsTokenInvalid(descToken) ? Language.GetString(pickupDef.nameToken) : Language.GetString(descToken);
            pickupDesc += "\n\n" + PickupStatsProvider.GetStatsForPickup(pickupIdx, itemCount);

            tooltip.overrideBodyText = pickupDesc;
        }

        public void UpdatePickupTooltip(ItemIcon icon, ItemIndex itemIdx, int itemCount)
        {
            PickupIndex pickupIdx = PickupCatalog.FindPickupIndex(itemIdx);

            if (pickupIdx != PickupIndex.none)
                UpdatePickupTooltip(icon.tooltipProvider, pickupIdx, itemCount);
        }
        
        public void UpdatePickupTooltip(EquipmentIcon icon, EquipmentIndex equipIdx)
        {
            PickupIndex pickupIdx = PickupCatalog.FindPickupIndex(equipIdx);

            if (pickupIdx != PickupIndex.none)
                UpdatePickupTooltip(icon.tooltipProvider, pickupIdx);
        }

        private void PickupCatalog_Init(On.RoR2.PickupCatalog.orig_Init orig)
        {
            orig();
            UpdateStats();
        }

        private void ItemIcon_SetItemIndex(On.RoR2.UI.ItemIcon.orig_SetItemIndex orig, ItemIcon self, ItemIndex newItemIndex, int newItemCount)
        {
            orig(self, newItemIndex, newItemCount);
            UpdatePickupTooltip(self, newItemIndex, newItemCount);
        }

        private void EquipmentIcon_SetDisplayData(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.After,
                x => x.MatchLdarg(0),
                x => x.MatchLdarg(1),
                x => x.MatchStfld(out _),
                x => x.MatchRet()
                );
            c.Index -= 1;
            Logger.LogDebug(c);
            c.Emit(OpCodes.Ldarg, 0);
            Logger.LogDebug(c);
            c.Emit(OpCodes.Ldarg, 1);
            Logger.LogDebug(c);
            c.Emit(OpCodes.Ldfld, typeof(EquipmentIcon).GetNestedTypeCached("DisplayData").GetFieldCached("equipmentDef"));
            Logger.LogDebug(c);
            c.EmitDelegate<Action<EquipmentIcon, EquipmentDef>>((icon, equipDef) =>
            {
                if (icon.tooltipProvider != null && equipDef != null)
                {
                    UpdatePickupTooltip(icon, equipDef.equipmentIndex);
                }
            });
        }

        // This 'On' hook is just broken no matter what you do - not sure when or if it'll be fixed.
        /*private void EquipmentIcon_SetDisplayData(On.RoR2.UI.EquipmentIcon.orig_SetDisplayData orig, EquipmentIcon self, object newDisplayData)
        {
            orig(self, (ValueType)newDisplayData);
            try
            {
                EquipmentDef equipDef = newDisplayData.GetFieldValue<EquipmentDef>("equipmentDef");
                EquipmentIndex equipIdx = equipDef.equipmentIndex;

                UpdatePickupTooltip(self, equipIdx);
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
            }
        }*/

        private void ItemInventoryDisplay_UpdateDisplay(On.RoR2.UI.ItemInventoryDisplay.orig_UpdateDisplay orig, ItemInventoryDisplay self)
        {
            orig(self);
        }
    }
}
