using System;
using RoR2;
using BepInEx;
using R2API.Utils;
using UnityEngine;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using RoR2.UI;
using BepInEx.Logging;

namespace UnosMods.TabItemStats
{
    [BepInDependency(R2API.R2API.PluginGUID)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class TabItemStats : BaseUnityPlugin
    {
        public const string PluginName = "TabItemStats";
        public const string PluginVersion = "1.0.1";
        public const string PluginGUID = "com.unordinal.itemtabstats";
        internal new static ManualLogSource Logger { get; } = new ManualLogSource(PluginName);

        internal void Awake()
        {
            On.RoR2.UI.ItemInventoryDisplay.UpdateDisplay += ItemInventoryDisplay_UpdateDisplay;
            On.RoR2.UI.ItemIcon.SetItemIndex += ItemIcon_SetItemIndex;
            IL.RoR2.UI.EquipmentIcon.SetDisplayData += EquipmentIcon_SetDisplayData;
            UpdateInventoryStats();
        }

        private void ItemInventoryDisplay_UpdateDisplay(On.RoR2.UI.ItemInventoryDisplay.orig_UpdateDisplay orig, ItemInventoryDisplay self)
        {
            orig(self);
            UpdateInventoryStats();
        }

        private void UpdateInventoryStats()
        {
            Logger.LogDebug("Updating inventory stats.");
            PickupStatProvider.UpdateItemDefs();
            PickupStatProvider.UpdateEquipmentDefs();
        }

        private void ItemIcon_SetItemIndex(On.RoR2.UI.ItemIcon.orig_SetItemIndex orig, RoR2.UI.ItemIcon self, ItemIndex newItemIndex, int newItemCount)
        {
            orig(self, newItemIndex, newItemCount);
            var itemDef = ItemCatalog.GetItemDef(newItemIndex);
            if (self.tooltipProvider != null && itemDef != null)
            {
                var itemDescription = !Language.IsTokenInvalid(itemDef.descriptionToken) ? Language.GetString(itemDef.descriptionToken) : Language.GetString(itemDef.pickupToken);
                itemDescription += "\n\n" + PickupStatProvider.ProvideItemStats(newItemIndex, newItemCount);
                self.tooltipProvider.overrideBodyText = itemDescription;
            }
        }

        private void EquipmentIcon_SetDisplayData(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(
                x => x.MatchLdarg(0),
                x => x.MatchLdarg(1),
                x => x.MatchStfld(out _),
                x => x.MatchRet()
                );
            c.Index += 3;
            Debug.Log(c);
            c.Emit(OpCodes.Ldarg, 0);
            Debug.Log(c);
            c.Emit(OpCodes.Ldarg, 1);
            Debug.Log(c);
            c.Emit(OpCodes.Ldfld, typeof(EquipmentIcon).GetNestedTypeCached("DisplayData").GetFieldCached("equipmentDef"));
            Debug.Log(c);
            c.EmitDelegate<Action<EquipmentIcon, EquipmentDef>>((icon, equipDef) =>
            {
                if (icon.tooltipProvider != null && equipDef != null)
                {
                    var equipDesc = !Language.IsTokenInvalid(equipDef.descriptionToken) ? Language.GetString(equipDef.descriptionToken) : Language.GetString(equipDef.pickupToken);
                    equipDesc += "\n\n" + PickupStatProvider.ProvideEquipmentStats(equipDef.equipmentIndex);
                    icon.tooltipProvider.overrideBodyText = equipDesc;
                }
            });
        }
    }
}
