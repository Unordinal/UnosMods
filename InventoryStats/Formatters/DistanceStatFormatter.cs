namespace Unordinal.InventoryStats.Formatters
{
    public class DistanceStatFormatter : NumStatFormatter
    {
        public DistanceStatFormatter(string prefix = "", string suffix = "m", string color = "", int places = 0, float maxValue = 0) : base(prefix, suffix, color, places, maxValue) { }

        public override string Format(float value)
        {
            string valueStr = value.ToString(nfi);

            if (effectiveMax != 0f && value >= effectiveMax)
                valueStr += " [Effective Max]";

            return Format(valueStr);
        }
    }
}
