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
        public const string PluginVersion = "1.0.0";
        private const string ModRpcId = "UnosMods.DropItems";

        private static MiniRpcLib.Action.IRpcAction<DropItemsMessage> CmdDropItem;
        public Dictionary<Inventory, PickupIndex> LastPickedUpItem { get; set; } = new Dictionary<Inventory, PickupIndex>();

        private readonly KeyCode? dropItemKey;

        public DropItems()
        {
            var configDropItemKey = Config.Wrap("DropItems", "DropItemKey", "Key code for dropping the item that was last picked up. (Default: G)", KeyCode.G.ToString());
            dropItemKey = GetKey(configDropItemKey);
            if (dropItemKey == null)
                Logger.LogError($"Invalid keycode '{configDropItemKey}' specified for DropItemKey!");

            var miniRpc = MiniRpc.CreateInstance(ModRpcId);
            CmdDropItem = miniRpc.RegisterAction(Target.Server, (Action<NetworkUser, DropItemsMessage>)DoDropItem);
            On.RoR2.GenericPickupController.GrantItem += GenericPickupController_GrantItem;
            On.RoR2.GenericPickupController.GrantEquipment += GenericPickupController_GrantEquipment;
        }

        public void Update()
        {
            if (Run.instance && Stage.instance && dropItemKey != null && Input.GetKeyDown(dropItemKey.Value))
                DropLastItem();
        }

        private void GenericPickupController_GrantItem(On.RoR2.GenericPickupController.orig_GrantItem orig, GenericPickupController self, CharacterBody body, Inventory inventory)
        {
            orig(self, body, inventory);
            LastPickedUpItem[inventory] = self.pickupIndex;
        }

        private void GenericPickupController_GrantEquipment(On.RoR2.GenericPickupController.orig_GrantEquipment orig, GenericPickupController self, CharacterBody body, Inventory inventory)
        {
            orig(self, body, inventory);
            LastPickedUpItem[inventory] = new PickupIndex(inventory.GetEquipmentIndex());
        }

        private KeyCode? GetKey(ConfigWrapper<string> param)
        {
            if (!Enum.TryParse(param.Value, out KeyCode result))
                return null;
            return result;
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

            PickupIndex pickupIndex;
            if (LastPickedUpItem[inv].equipmentIndex != EquipmentIndex.None)
                pickupIndex = new PickupIndex(LastPickedUpItem[inv].equipmentIndex);
            else
                pickupIndex = new PickupIndex(LastPickedUpItem[inv].itemIndex);

            Logger.LogInfo($"Dropping item '{pickupIndex}' for host '{player.networkUser?.userName}'");
            LastPickedUpItem[inv] = PickupIndex.none;
            DropItem(player, pickupIndex);
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

            PickupIndex pickupIndex;
            if (LastPickedUpItem[inv].equipmentIndex != EquipmentIndex.None)
                pickupIndex = new PickupIndex(LastPickedUpItem[inv].equipmentIndex);
            else
                pickupIndex = new PickupIndex(LastPickedUpItem[inv].itemIndex);

            Logger.LogInfo($"Dropping item '{pickupIndex}' for client '{user.userName}'");
            LastPickedUpItem[inv] = PickupIndex.none;
            DropItem(user.masterController, pickupIndex);
        }

        public bool DropItem(PlayerCharacterMasterController player, PickupIndex pickupIndex)
        {
            if (!NetworkServer.active)
            {
                Debug.LogWarning("[Server] function 'System.Boolean UnosMods.DropItems::DropItem(RoR2.PlayerCharacterMasterController,RoR2.PickupIndex)' called on client");
                return false;
            }

            var inv = player.master?.inventory;
            if (inv == null)
                return false;
            
            Logger.LogDebug($"Dropping {pickupIndex} (equip: {pickupIndex.equipmentIndex}, item: {pickupIndex.itemIndex})");
            if (pickupIndex.equipmentIndex != EquipmentIndex.None)
            {
                if (inv.GetEquipmentIndex() != pickupIndex.equipmentIndex)
                    return false;
                inv.SetEquipmentIndex(EquipmentIndex.None);
            }
            else
            {
                if (inv.GetItemCount(pickupIndex.itemIndex) <= 0)
                    return false;
                inv.RemoveItem(pickupIndex.itemIndex, 1);
            }
            
            var transform = player.master.GetBody().coreTransform;
            PickupDropletController.CreatePickupDroplet(pickupIndex, 
                                                        transform.position, 
                                                        transform.up * 15f + transform.forward * 10f);
            return true;
        }
    }
}
