using R2API.Networking;
using R2API.Networking.Interfaces;
using RoR2;

namespace Unordinal.ToolbotEquipmentSwap.Networking
{
    public class ClientConfigCommand : INetCommand
    {
        public void OnReceived()
        {
            Util.LogMessageReceived(this, true);

            new ClientConfigMessage
            {
                NetUser = LocalUserManager.GetFirstLocalUser().currentNetworkUser,
                SwapKey = PluginConfig.SwapKey,
                SwapOnRetool = PluginConfig.SwapOnRetool
            }.Send(NetworkDestination.Server);
        }
    }
}
