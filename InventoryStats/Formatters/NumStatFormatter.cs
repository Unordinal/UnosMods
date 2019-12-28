using System.Globalization;

namespace Unordinal.InventoryStats.Formatters
{
    public class NumStatFormatter : StatFormatter
    {
        protected readonly NumberFormatInfo nfi;
        protected readonly int places;
        protected readonly float effectiveMax;

        public NumStatFormatter(string prefix = "", string suffix = "", string color = "", int places = 0, float effectiveMax = 0) : base(null, prefix, suffix, color)
        {
            this.places = places;
            this.effectiveMax = effectiveMax;
            this.nfi = GetNFI();
        }

        public override string Format(float value)
        {
            string valueStr = suffix == "%" 
                ? value.ToString("P", nfi) 
                : value.ToString(nfi);

            if (effectiveMax != 0f && value >= effectiveMax)
                valueStr += " [Effective Max]";

            return Format(valueStr);
        }

        public virtual NumberFormatInfo GetNFI()
        {
            NumberFormatInfo nfi = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
            nfi.PercentPositivePattern = 1;
            nfi.PercentNegativePattern = 1;
            if (places > 0)
            {
                nfi.PercentDecimalDigits = places;
                nfi.NumberDecimalDigits = places;
            }

            return nfi;
        }
    }
}
