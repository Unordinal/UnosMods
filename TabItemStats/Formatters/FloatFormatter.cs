using RoR2;
using static UnosUtilities.StyleCatalog;

namespace UnosMods.TabItemStats.Formatters
{
    public class FloatFormatter : IStatFormatter
    {
        private readonly string suffix;
        private readonly string color;
        private readonly StyleIndex style;
        private readonly uint places;

        public FloatFormatter(string color = "", StyleIndex style = StyleIndex.None, string suffix = "", uint places = 1)
        {
            this.color = color;
            this.style = style;
            this.suffix = suffix;
            this.places = places;
            if (string.IsNullOrEmpty(color) && style == StyleIndex.None)
                this.style = StyleIndex.cIsHealing;
        }

        public string Format(float value)
        {
            var valueStr = value.ToString($"f{places}");
            var returnStr = $"{valueStr}{suffix}";
            if (style == StyleIndex.None)
                return returnStr.Color(color);
            else
                return returnStr.Style(style);

        }
    }
}
