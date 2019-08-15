using RoR2;

namespace UnosMods.TabItemStats
{
    public static class Extensions
    {
        public static string Color(this string str, ColorCatalog.ColorIndex color)
        {
            return str.Color(color.ToHex());
        }

        public static string Color(this string str, string color)
        {
            return $"<color={color}>{str}</color>";
        }

        public static string ToHex(this ColorCatalog.ColorIndex colorIndex, bool excludeSymbol = false)
        {
            return (excludeSymbol ? "" : "#") + ColorCatalog.GetColorHexString(colorIndex);
        }

        public static int ItemStacks(this CharacterBody body, ItemIndex item)
        {
            return body?.inventory?.GetItemCount(item) ?? 0;
        }
    }
}
