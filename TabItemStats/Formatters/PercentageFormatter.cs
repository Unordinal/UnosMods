using System.Globalization;
using RoR2;
using UnityEngine;
using UnosUtilities;
using static UnosUtilities.StyleCatalog;

namespace UnosMods.TabItemStats.Formatters
{
    public class PercentageFormatter : IStatFormatter
    {
        private readonly uint places;
        private readonly float maxValue;
        private readonly string color;
        private readonly StyleIndex style;
        private readonly string prefix;
        private readonly string suffix;

        public static implicit operator IntFormatter(PercentageFormatter pf) => new IntFormatter(color: pf.color, style: pf.style, suffix: pf.suffix);
        public static implicit operator FloatFormatter(PercentageFormatter pf) => new FloatFormatter(color: pf.color, style: pf.style, suffix: pf.suffix, places: pf.places);

        public PercentageFormatter(string color = "", StyleIndex style = StyleIndex.None, string prefix = "", string suffix = "", uint places = 2, float maxValue = 0f)
        {
            this.color = color;
            this.style = style;
            this.prefix = prefix;
            this.suffix = suffix;
            this.places = places;
            this.maxValue = maxValue > 0 ? maxValue : float.MaxValue;
            if (string.IsNullOrEmpty(color) && style == StyleIndex.None)
                this.color = StyleIndex.cIsHealing.GetColorHexString(true);
        }

        public string Format(float value)
        {
            var effectiveMaxText = value >= maxValue ? " [Effective Max]".Color(ColorCatalog.ColorIndex.BossItem) : string.Empty;
            value = Mathf.Min(value, maxValue);

            NumberFormatInfo numInfo = new NumberFormatInfo { PercentPositivePattern = 1 };
            string valueStr = value.ToString($"P{places}", numInfo); // 0.3712 -> 37.12%
            valueStr = style == StyleIndex.None ? valueStr.Color(color) : valueStr.Style(style);
            return $"{prefix}{valueStr}{suffix}{effectiveMaxText}";
        }
    }
}
