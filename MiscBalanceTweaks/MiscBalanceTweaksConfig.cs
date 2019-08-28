using System;
using UnityEngine;
using BepInEx.Configuration;

namespace UnosMods.MiscBalanceTweaks
{
    public static class MiscBalanceTweaksConfig
    {
        private static ConfigWrapper<float> minSkillCooldown;
        public static float MinSkillCooldown { get => Mathf.Clamp(minSkillCooldown.Value, 0f, 2f); }
        public static void Init(ConfigFile cfg)
        {
            minSkillCooldown = cfg.Wrap(
                "BalanceTweaks",
                "MinSkillCooldown",
                "Sets the minimum skill cooldown. (Default: 0.1f, Vanilla: 0.5f)",
                0.1f
                );
        }

        public static KeyCode? GetKey(ConfigWrapper<string> param)
        {
            if (!Enum.TryParse(param.Value, out KeyCode result))
                return null;
            return result;
        }
    }
}
