using R2API.Networking.Interfaces;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace Unordinal.ToolbotEquipmentSwap.Networking
{
    public class ClientConfigRequest : INetRequest<ClientConfigRequest, ClientConfigRequestReply>
    {
        public ClientConfigRequestReply OnRequestReceived()
        {
            ToolbotEquipmentSwap.Logger.LogDebug($"[Received '{nameof(ClientConfigRequest)}' from server, sending reply.]");

            return new ClientConfigRequestReply
            {
                NetUser = LocalUserManager.GetFirstLocalUser().currentNetworkUser,
                SwapKey = PluginConfig.SwapKey,
                SwapOnRetool = PluginConfig.SwapOnRetool
            };
        }

        public void Serialize(NetworkWriter writer) { }

        public void Deserialize(NetworkReader reader) { }
    }

    public class ClientConfigRequestReply : INetRequestReply<ClientConfigRequest, ClientConfigRequestReply>
    {
        public NetworkUser NetUser { get; set; }
        public KeyCode SwapKey { get; set; }
        public bool SwapOnRetool { get; set; }

        public void OnReplyReceived()
        {
            ToolbotEquipmentSwap.Logger.LogDebug($"[Received '{nameof(ClientConfigRequestReply)}' from client '{NetUser.userName}'.]");

            ToolbotEquipmentSwap.ClientsSettings[NetUser] = (SwapKey, SwapOnRetool);
        }

        public void Serialize(NetworkWriter writer)
        {
            ToolbotEquipmentSwap.Logger.LogDebug($"[Serializing '{nameof(ClientConfigRequestReply)}']");

            writer.Write(NetUser.gameObject);
            writer.Write(SwapKey.ToString());
            writer.Write(SwapOnRetool);
        }

        public void Deserialize(NetworkReader reader)
        {
            ToolbotEquipmentSwap.Logger.LogDebug($"[Deserializing '{nameof(ClientConfigRequestReply)}']");

            NetUser = reader.ReadGameObject().GetComponent<NetworkUser>();
            ToolbotEquipmentSwap.Logger.LogDebug($"[{nameof(NetUser)}: {NetUser.userName}]");

            SwapKey = PluginConfig.ParseKeyCode(reader.ReadString());
            ToolbotEquipmentSwap.Logger.LogDebug($"[{nameof(SwapKey)}: {SwapKey}]");

            SwapOnRetool = reader.ReadBoolean();
            ToolbotEquipmentSwap.Logger.LogDebug($"[{nameof(SwapOnRetool)}: {SwapOnRetool}]");
        }
    }
}
