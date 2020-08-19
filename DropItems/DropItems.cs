using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using R2API.Networking;
using R2API.Networking.Interfaces;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using Unordinal.DropItems.Networking;

namespace Unordinal.DropItems
{
    [BepInDependency(R2API.R2API.PluginGUID)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public sealed class DropItems : BaseUnityPlugin
    {
        public const string PluginName = "Drop Items";
        public const string PluginGUID = "Unordinal.DropItems";
        public const string PluginVersion = "1.0.4";

        public static new ManualLogSource Logger { get; private set; }

        public static Dictionary<Inventory, PickupIndex> LastPickedUpItem { get; set; } = new Dictionary<Inventory, PickupIndex>();

        private KeyCode? dropItemKey;

        internal void Awake()
        {
            Logger = base.Logger;

            var configDropItemKey = Config.Bind("DropItems", "DropItemKey", KeyCode.G.ToString(), "Key code for dropping the item that was last picked up.");
            dropItemKey = GetKey(configDropItemKey);
            if (dropItemKey is null) Logger.LogError($"Invalid keycode '{configDropItemKey}' specified for DropItemKey!");

            On.RoR2.GenericPickupController.GrantItem += GenericPickupController_GrantItem;
            On.RoR2.GenericPickupController.GrantEquipment += GenericPickupController_GrantEquipment;
        }

        internal void Start()
        {
            NetworkingAPI.RegisterMessageType<DropLastItemMessage>();
        }

        internal void Update()
        {
            var localMaster = LocalUserManager.GetFirstLocalUser().cachedMaster;
            if (Run.instance && Stage.instance && localMaster?.GetBody() != null && !localMaster.lostBodyToDeath)
            {
                if (dropItemKey != null && Input.GetKeyDown(dropItemKey.Value))
                {
                    new DropLastItemMessage(localMaster).Send(NetworkDestination.Server);
                }
            }
        }

        public static void DropLastItem(CharacterMaster charMaster)
        {
            if (NetworkServer.active)
            {
                var inv = charMaster.inventory;
                var pcmc = charMaster.playerCharacterMasterController;
                string requesterName = pcmc.GetDisplayName();
                if (LastPickedUpItem.ContainsKey(inv) && LastPickedUpItem[inv] != PickupIndex.none && inv.itemAcquisitionOrder.Any())
                {
                    PickupDef pickupDef = PickupCatalog.GetPickupDef(LastPickedUpItem[inv]);
                    LastPickedUpItem[inv] = PickupIndex.none;

                    if (pickupDef.equipmentIndex != EquipmentIndex.None)
                    {
                        if (inv.GetEquipmentIndex() != pickupDef.equipmentIndex)
                        {
                            Logger.LogWarning($"'{requesterName}' tried to drop an equipment item they don't have! ({pickupDef.equipmentIndex})");
                            return;
                        }
                        inv.SetEquipmentIndex(EquipmentIndex.None);
                    }
                    else
                    {
                        if (inv.GetItemCount(pickupDef.itemIndex) <= 0)
                        {
                            Logger.LogWarning($"'{requesterName}' tried to drop an item they don't have! ({pickupDef.itemIndex})");
                            return;
                        }
                        inv.RemoveItem(pickupDef.itemIndex, 1);
                    }

                    var transform = charMaster.GetBody()?.coreTransform;
                    if (transform != null)
                    {
                        PickupDropletController.CreatePickupDroplet(pickupDef.pickupIndex,
                                                                    transform.position,
                                                                    transform.up * 15f + transform.forward * 10f);
                    }

                    Logger.LogInfo($"'{requesterName}' dropped '{Language.GetString(pickupDef.nameToken)}'");
                }
                else
                {
                    Logger.LogDebug($"'{requesterName}' tried to drop an item but is not allowed.");
                }
            }
            else
            {
                Logger.LogWarning("DropLastItem called on client!");
            }
        }

        private void GenericPickupController_GrantItem(On.RoR2.GenericPickupController.orig_GrantItem orig, GenericPickupController self, CharacterBody body, Inventory inventory)
        {
            orig(self, body, inventory);
            var pickupDef = PickupCatalog.GetPickupDef(self.pickupIndex);
            if (pickupDef != null)
            {
                var itemDef = ItemCatalog.GetItemDef(pickupDef.itemIndex);
                if (itemDef != null && itemDef.tier != ItemTier.NoTier)
                {
                    LastPickedUpItem[inventory] = pickupDef.pickupIndex;
                }
            }
        }

        private void GenericPickupController_GrantEquipment(On.RoR2.GenericPickupController.orig_GrantEquipment orig, GenericPickupController self, CharacterBody body, Inventory inventory)
        {
            orig(self, body, inventory);
            LastPickedUpItem[inventory] = PickupCatalog.FindPickupIndex(inventory.GetEquipmentIndex());
        }

        private KeyCode? GetKey(ConfigEntry<string> param)
        {
            return Enum.TryParse(param.Value, out KeyCode result) ? (KeyCode?)result : null;
        }
    }
}
