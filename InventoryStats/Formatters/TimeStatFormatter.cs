using System.Globalization;

namespace Unordinal.InventoryStats.Formatters
{
    public class TimeStatFormatter : NumStatFormatter
    {
        public TimeStatFormatter(string prefix = "", string suffix = "s", string color = "", int places = 0, float effectiveMax = 0) : base(prefix, suffix, color, places, effectiveMax) { }

        public override string Format(float value)
        {
            string valueStr = value.ToString(nfi);

            if (effectiveMax != 0f && value >= effectiveMax)
                valueStr += " [Effective Max]";

            return Format(valueStr);
        }
    }
}
