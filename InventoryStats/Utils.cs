using RoR2;
using static RoR2.ColorCatalog;

namespace Unordinal.InventoryStats
{
    public static class Utils
    {
        public static string Colorize(this string str, string color)
        {
            return string.IsNullOrWhiteSpace(color) ? string.Empty : $"<color={color}>{str}</color>";
        }

        public static string ToHex(this ColorIndex colorIndex, bool withSymbol = true)
        {
            return (withSymbol ? "#" : "") + GetColorHexString(colorIndex);
        }
    }
}
