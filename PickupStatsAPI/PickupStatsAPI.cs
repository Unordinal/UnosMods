using BepInEx;
using R2API;

namespace PickupStatsAPI
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(R2API.R2API.PluginGUID)]
    public class PickupStatsAPI : BaseUnityPlugin
    {
        public const string PluginName = "Pickup Stats API";
        public const string PluginVersion = "0.0.1";
        public const string PluginGUID = "com.unordinal.pickupstatsapi";

        internal void Awake()
        {
            
        }
    }
}
