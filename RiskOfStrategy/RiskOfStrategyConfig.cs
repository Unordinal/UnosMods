using System;
using UnityEngine;
using BepInEx.Configuration;

namespace UnosMods.RiskOfStrategy
{
    public static class RiskOfStrategyConfig
    {
        //public static ConfigWrapper<string> TemplateConfigSetting;
        public static void Init(ConfigFile cfg)
        {
            /*TemplateConfigSetting = cfg.Wrap(
                "Settings",
                "TemplateConfig",
                "A template configuration setting.",
                "String value"
                );*/
        }

        public static KeyCode? GetKey(ConfigWrapper<string> param)
        {
            if (!Enum.TryParse(param.Value, out KeyCode result))
                return null;
            return result;
        }
    }
}
