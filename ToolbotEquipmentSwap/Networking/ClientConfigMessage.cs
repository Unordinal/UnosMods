using R2API.Networking.Interfaces;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace Unordinal.ToolbotEquipmentSwap.Networking
{
    public class ClientConfigMessage : INetMessage
    {
        public NetworkUser NetUser { get; set; }
        public KeyCode SwapKey { get; set; }
        public bool SwapOnRetool { get; set; }

        public void OnReceived()
        {
            Util.LogMessageReceived(this, false, NetUser.userName);

            ToolbotEquipmentSwap.ClientsSettings[NetUser] = (SwapKey, SwapOnRetool);
        }

        public void Serialize(NetworkWriter writer)
        {
            Util.LogSerialization(this, false);

            writer.Write(NetUser.gameObject);
            writer.Write(SwapKey.ToString());
            writer.Write(SwapOnRetool);
        }

        public void Deserialize(NetworkReader reader)
        {
            Util.LogSerialization(this, true);

            NetUser = reader.ReadGameObject().GetComponent<NetworkUser>();
            Util.LogVariable(NetUser.userName, nameof(NetUser));

            SwapKey = PluginConfig.ParseKeyCode(reader.ReadString());
            Util.LogVariable(SwapKey, nameof(SwapKey));

            SwapOnRetool = reader.ReadBoolean();
            Util.LogVariable(SwapOnRetool, nameof(SwapOnRetool));
        }
    }
}
