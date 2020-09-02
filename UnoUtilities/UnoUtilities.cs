using BepInEx;
using BepInEx.Logging;
using R2API.Networking;
using R2API.Utils;
using System.Collections.Generic;
using UnityEngine;
using Unordinal.UnoUtilities.Networking;

namespace Unordinal.UnoUtilities
{
    [BepInDependency(R2API.R2API.PluginGUID)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [NetworkCompatibility(CompatibilityLevel.NoNeedForSync)]
    public class UnoUtilities : BaseUnityPlugin
    {
        public const string PluginGUID = "Unordinal.UnoUtilities";
        public const string PluginName = "UnoUtilities";
        public const string PluginVersion = "1.0.0";
        public static new ManualLogSource Logger { get; private set; }


        // Called when the script instance is initialized. Only called once during the lifetime of the script.
        internal void Awake()
        {
            Logger = base.Logger;
            Logger.LogError("This should not be loaded as a plugin! WHATTERUDOIN?");
            Destroy(this);
        }

        /// <summary>
        /// Gets the currently loaded plugins. Dictionary keys are the mods' GUIDs.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, PluginInfo> GetLoadedPlugins()
        {
            return BepInEx.Bootstrap.Chainloader.PluginInfos;
        }

        public static BaseUnityPlugin GetPluginInstance(string pluginGUID)
        {
            BaseUnityPlugin instance = null;

            var loadedPlugins = GetLoadedPlugins();
            if (loadedPlugins.ContainsKey(pluginGUID))
            {
                instance = loadedPlugins[pluginGUID].Instance;
            }

            return instance;
        }
    }
}
