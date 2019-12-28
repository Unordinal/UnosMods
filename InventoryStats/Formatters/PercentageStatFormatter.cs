namespace Unordinal.InventoryStats.Formatters
{
    public class PercentageStatFormatter : NumStatFormatter
    {
        public PercentageStatFormatter(string prefix = "", string suffix = "", string color = "", int places = 2, float effectiveMax = 0f) : base(prefix, suffix, color, places, effectiveMax) { }

        public override string Format(float value)
        {
            string valueStr = value.ToString("P", nfi);

            if (effectiveMax != 0f && value >= effectiveMax)
                valueStr += " [Effective Max]";

            return Format(valueStr);
        }
    }
}
