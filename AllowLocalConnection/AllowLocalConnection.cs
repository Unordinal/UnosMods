using BepInEx;
using BepInEx.Logging;

namespace AllowLocalConnection
{

    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(R2API.R2API.PluginGUID)]
    public class AllowLocalConnection : BaseUnityPlugin
    {
        public const string PluginName = "AllowLocalConnection";
        public const string PluginVersion = "1.0.0";
        public const string PluginGUID = "com.Unordinal.AllowLocalConnection";

        public static new ManualLogSource Logger { get; private set; }

        public void Awake()
        {
            Logger = base.Logger;
        }

        public void OnEnable()
        {
            AddHooks();
        }

        public void OnDisable()
        {
            RemoveHooks();
        }

        public void AddHooks()
        {
            On.RoR2.Networking.GameNetworkManager.OnClientConnect += this.GameNetworkManager_OnClientConnect;
        }

        public void RemoveHooks()
        {
            On.RoR2.Networking.GameNetworkManager.OnClientConnect -= this.GameNetworkManager_OnClientConnect;
        }

        private void GameNetworkManager_OnClientConnect(On.RoR2.Networking.GameNetworkManager.orig_OnClientConnect orig, RoR2.Networking.GameNetworkManager self, UnityEngine.Networking.NetworkConnection conn)
        {
            // Do nothing.
        }
    }
}
