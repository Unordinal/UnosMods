using R2API.Networking.Interfaces;
using RoR2;
using UnityEngine.Networking;

namespace Unordinal.ToolbotEquipmentSwap.Networking
{
    public class EquipSwapMessage : INetMessage
    {
        public NetworkUser NetUser { get; set; }

        public void OnReceived()
        {
            Util.LogMessageReceived(this, false, NetUser.userName);

            if (NetworkServer.active)
            {
                Inventory inv = NetUser.GetCurrentBody()?.inventory;
                if (inv is null)
                {
                    ToolbotEquipmentSwap.Logger.LogDebug($"{NetUser.userName} did not have a valid inventory.");
                    return;
                }
                
                byte slotToSwitchTo = (byte)(inv.activeEquipmentSlot == 0 ? 1 : 0);
                inv.SetActiveEquipmentSlot(slotToSwitchTo);
                ToolbotEquipmentSwap.Logger.LogDebug($"Swapped {NetUser.userName}'s equipment slot to slot {slotToSwitchTo}.");
            }
        }
        public void Serialize(NetworkWriter writer)
        {
            Util.LogSerialization(this, false);

            writer.Write(NetUser.masterController.gameObject);
        }

        public void Deserialize(NetworkReader reader)
        {
            Util.LogSerialization(this, true);

            NetUser = reader.ReadGameObject().GetComponent<PlayerCharacterMasterController>().networkUser;
            Util.LogVariable(NetUser.userName, nameof(NetUser));
        }
    }
}
