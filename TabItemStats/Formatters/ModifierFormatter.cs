using RoR2;
using System.Globalization;

namespace UnosMods.TabItemStats.Formatters
{
    public class ModifierFormatter : IStatFormatter
    {
        private readonly string modifierName;
        private readonly ItemIndex modifierIndex;
        private readonly string color;

        public ModifierFormatter(string modifierName = "", ItemIndex modifierIndex = ItemIndex.None, string color = "")
        {
            this.modifierName = modifierName;
            this.modifierIndex = modifierIndex;
            this.color = color;
        }

        public string Format(float value)
        {
            string color = this.color;
            if (string.IsNullOrEmpty(color))
                color = value >= 0 ? ColorCatalog.ColorIndex.Tier2Item.ToHex() : ColorCatalog.ColorIndex.Tier3Item.ToHex();
            NumberFormatInfo numInfo = new NumberFormatInfo { PercentPositivePattern = 1 };
            var valueStr = value.ToString("P2", numInfo).Color(color);
            var stack = string.Empty;
            if (modifierIndex != ItemIndex.None)
                stack = $" ({ContextProvider.ItemStacks(modifierIndex)})";
            return $"\t{modifierName}{stack}: {valueStr}";
        }
    }
}
