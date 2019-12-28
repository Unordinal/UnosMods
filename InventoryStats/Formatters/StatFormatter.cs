using Unordinal.InventoryStats.Stats;

namespace Unordinal.InventoryStats.Formatters
{
    public class StatFormatter
    {
        protected readonly string text;
        protected readonly string prefix;
        protected readonly string suffix;
        protected string color;

        public string Color
        {
            get => color;
            set => color = value;
        }

        public StatFormatter(string text = "", string prefix = "", string suffix = "", string color = "")
        {
            this.text = text;
            this.prefix = prefix;
            this.suffix = suffix;
            this.color = color;
        }

        public virtual string Format(string value)
        {
            string actualColor = string.IsNullOrWhiteSpace(color) ? PickupStatsDefinitions.ColorPositive : color;
            return $"{prefix}{value}{suffix}".Trim().Colorize(actualColor);
        }

        public virtual string Format(float value)
        {
            return Format(value.ToString());
        }
    }
}
