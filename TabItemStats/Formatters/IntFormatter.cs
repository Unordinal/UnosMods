using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using UnityEngine;
using UnosMods.TabItemStats;
using static UnosUtilities.StyleCatalog;

namespace UnosMods.TabItemStats.Formatters
{
    public class IntFormatter : IStatFormatter
    {
        private readonly string prefix;
        private readonly string suffix;
        private readonly string color;
        private readonly StyleIndex style;

        public IntFormatter(string color = "", StyleIndex style = StyleIndex.None, string prefix = "", string suffix = "")
        {
            this.color = color;
            this.style = style;
            this.prefix = prefix;
            this.suffix = suffix;
            if (string.IsNullOrEmpty(color) && style == StyleIndex.None)
                this.color = StyleIndex.cIsHealing.GetColorHexString(true);
        }

        public string Format(float value)
        {
            string valueStr = $"{prefix}{Math.Round(value, 0)}{suffix}";
            if (style == StyleIndex.None)
                return valueStr.Color(color);
            else
                return valueStr.Style(style);
        }
    }
}
