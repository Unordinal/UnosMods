using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace Unordinal.BuffIconTooltips
{
    public static class Util
    {
        public enum Style
        {
            None,
            Health,
            Healing,
            Utility,
            Damage,
            Death,
            Shrine,
            Artifact,
            Stack,
            Sub,
            Mono,
            Event,
            WorldEvent,
            KeywordName,
            UserSetting
        }

        // Sprites later? ex: <sprite name=\"TP\" tint=1>
        public static Dictionary<Style, string> StyleToString { get; } = new Dictionary<Style, string>
        {
            { Style.Health, "cIsHealth" },
            { Style.Healing, "cIsHealing" },
            { Style.Utility, "cIsUtility" },
            { Style.Damage, "cIsDamage" },
            { Style.Death, "cDeath" },
            { Style.Shrine, "cShrine" },
            { Style.Artifact, "cArtifact" },
            { Style.Stack, "cStack" },
            { Style.Sub, "cSub" },
            { Style.Mono, "cMono" },
            { Style.Event, "cEvent" },
            { Style.WorldEvent, "cWorldEvent" },
            { Style.KeywordName, "cKeywordName" },
            { Style.UserSetting, "cUserSetting" },
        };

        public static string Stylize(this string text, Style style)
        {
            string styleCode = GetStyleCodeFromStyle(style);
            return $"<style={styleCode}>{text}</style>";
        }

        public static string Colorize(this string text, Color color)
        {
            string colorCode = "#" + ColorUtility.ToHtmlStringRGB(color);
            return $"<color={colorCode}>{text}</color>";

        }

        public static string DamageColorize(this string text, DamageColorIndex damageColorIndex)
        {
            string colorCode = GetDamageColorCode(damageColorIndex);
            return $"<color={colorCode}>{text}</color>";

        }

        public static string GetStyleCodeFromStyle(Style style)
        {
            StyleToString.TryGetValue(style, out string styleCode);
            return styleCode;
        }

        public static string GetDamageColorCode(DamageColorIndex damageColorIndex)
        {
            return "#" + ColorUtility.ToHtmlStringRGB(DamageColor.FindColor(damageColorIndex));
        }
    }
}
