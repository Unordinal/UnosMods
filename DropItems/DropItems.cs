using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Networking;

using RoR2;
using BepInEx;
using BepInEx.Configuration;
using MiniRpcLib;

namespace UnosMods.ToolbotEquipmentSwap
{
    [BepInDependency(R2API.R2API.PluginGUID)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public sealed class DropItems : BaseUnityPlugin
    {
        public const string PluginName = "Drop Items";
        public const string PluginGUID = "com.unordinal.dropitems";
        public const string PluginVersion = "1.0.3";
        private const string ModRpcId = "UnosMods.DropItems";

        private static MiniRpcLib.Action.IRpcAction<DropItemsMessage> CmdDropItem;
        public Dictionary<Inventory, PickupIndex> LastPickedUpItem { get; set; } = new Dictionary<Inventory, PickupIndex>();

        private readonly KeyCode? dropItemKey;

        public DropItems()
        {
            var configDropItemKey = Config.Bind("DropItems", "DropItemKey", KeyCode.G.ToString(), "Key code for dropping the item that was last picked up.");
            dropItemKey = GetKey(configDropItemKey);
            if (dropItemKey == null)
                Logger.LogError($"Invalid keycode '{configDropItemKey}' specified for DropItemKey!");

            var miniRpc = MiniRpc.CreateInstance(ModRpcId);
            CmdDropItem = miniRpc.RegisterAction(Target.Server, (Action<NetworkUser, DropItemsMessage>)DoDropItem);
            On.RoR2.GenericPickupController.GrantItem += GenericPickupController_GrantItem;
            On.RoR2.GenericPickupController.GrantEquipment += GenericPickupController_GrantEquipment;
        }

        // FixedUpdate causes input loss.
        public void Update()
        {
            if (Run.instance && Stage.instance && dropItemKey != null && Input.GetKeyDown(dropItemKey.Value))
                DropLastItem();
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

        public void DropLastItem()
        {
            if (!NetworkServer.active)
            {
                CmdDropItem.Invoke(new DropItemsMessage());
                return;
            }

            var player = PlayerCharacterMasterController.instances[0];
            var inv = player.master.inventory;

            if (inv == null || !LastPickedUpItem.ContainsKey(inv) || LastPickedUpItem[inv] == PickupIndex.none || !inv.itemAcquisitionOrder.Any())
            {
                Logger.LogWarning($"Received item drop request from host '{player.networkUser?.userName}' but they cannot drop an item right now.");
                return;
            }

            PickupDef pickupDef = PickupCatalog.GetPickupDef(LastPickedUpItem[inv]);

            Logger.LogInfo($"Dropping item '{pickupDef.internalName}' for host '{player.networkUser?.userName}'");
            LastPickedUpItem[inv] = PickupIndex.none;
            DropItem(player, pickupDef);
        }

        public void DoDropItem(NetworkUser user, MessageBase message)
        {
            Logger.LogDebug($"Attempting item drop for client '{user.userName}'");
            var master = user.master;
            var body = master?.GetBody(); // If master is not null, invoke GetBody() on it (https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/member-access-operators#null-conditional-operators--and-)
            if (body == null) // If master and body are null
            {
                Logger.LogError($"Network User '{user.userName}' has no body.");
                return;
            }
            
            var inv = master.inventory;
            if (inv == null || !LastPickedUpItem.ContainsKey(inv) || LastPickedUpItem[inv] == PickupIndex.none || !inv.itemAcquisitionOrder.Any())
            {
                Logger.LogWarning($"Received item drop request from client '{user.userName}' but they cannot drop an item right now.");
                return;
            }

            PickupDef pickupDef = PickupCatalog.GetPickupDef(LastPickedUpItem[inv]);

            Logger.LogInfo($"Dropping item '{pickupDef.internalName}' for client '{user.userName}'");
            LastPickedUpItem[inv] = PickupIndex.none;
            DropItem(user.masterController, pickupDef);
        }

        public bool DropItem(PlayerCharacterMasterController player, PickupDef pickupDef)
        {
            if (!NetworkServer.active)
            {
                Debug.LogWarning("[Server] function 'System.Boolean UnosMods.DropItems::DropItem(RoR2.PlayerCharacterMasterController,RoR2.PickupIndex)' called on client");
                return false;
            }

            var inv = player.master?.inventory;
            if (inv == null)
                return false;
            
            Logger.LogDebug($"Dropping {Language.GetString(pickupDef.internalName)} (equip: {pickupDef.equipmentIndex}, item: {pickupDef.itemIndex})");
            if (pickupDef.equipmentIndex != EquipmentIndex.None)
            {
                if (inv.GetEquipmentIndex() != pickupDef.equipmentIndex)
                    return false;
                inv.SetEquipmentIndex(EquipmentIndex.None);
            }
            else
            {
                if (inv.GetItemCount(pickupDef.itemIndex) <= 0)
                    return false;
                inv.RemoveItem(pickupDef.itemIndex, 1);
            }
            
            var transform = player.master.GetBody().coreTransform;
            PickupDropletController.CreatePickupDroplet(pickupDef.pickupIndex, 
                                                        transform.position, 
                                                        transform.up * 15f + transform.forward * 10f);
            return true;
        }
    }
}
