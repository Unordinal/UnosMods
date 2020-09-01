using BepInEx.Configuration;
using System;
using UnityEngine;

namespace Unordinal.ToolbotEquipmentSwap
{
    public static class PluginConfig
    {
        private static ConfigEntry<string> swapKey;
        private static ConfigEntry<bool> swapOnRetool;

        public static KeyCode SwapKey => ParseKeyCode(swapKey.Value);
        public static bool SwapOnRetool => swapOnRetool.Value;

        public static void Initialize(ConfigFile config)
        {
            swapKey = config.Bind(nameof(ToolbotEquipmentSwap), "SwapKey", KeyCode.X.ToString(),
                "The key used to swap equipment when playing as MUL-T.\n" +
                "Valid key codes can be found here (under the Properties header): https://docs.unity3d.com/ScriptReference/KeyCode.html");
            swapOnRetool = config.Bind(nameof(ToolbotEquipmentSwap), "RetoolSwapsEquipment", false,
                "If false, stops your equipment from swapping when swapping weapons as MUL-T.");
        }

        public static KeyCode ParseKeyCode(string keyCodeStr)
        {
            bool valid = Enum.TryParse(keyCodeStr, out KeyCode keyCode);
            if (!valid) ToolbotEquipmentSwap.Logger.LogWarning($"'{keyCodeStr}' is not a valid key code!");

            return keyCode;
        }
    }
}
