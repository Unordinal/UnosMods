using RoR2;
using System.Linq;
using System.Text;
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

        /// <summary>
        /// Removes all leading and trailing whitespace characters or specified characters from the current <see cref="StringBuilder"/> object.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> to trim.</param>
        /// <param name="trimChars">The characters to trim off.</param>
        public static void Trim(this StringBuilder sb, params char[] trimChars)
        {
            sb.TrimStart(trimChars);
            sb.TrimEnd(trimChars);
        }

        /// <summary>
        /// Removes all leading whitespace characters or specified characters from the current <see cref="StringBuilder"/> object.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> to trim.</param>
        /// <param name="trimChars">The characters to trim off.</param>
        public static StringBuilder TrimStart(this StringBuilder sb, params char[] trimChars)
        {
            if (sb is null || sb.Length == 0) return sb;
            bool validTrimChars = !(trimChars is null || trimChars.Length == 0);

            int toRemove = 0;
            for (int i = 0; i < sb.Length; i++)
            {
                bool shouldRemove = (!validTrimChars && char.IsWhiteSpace(sb[i])) || trimChars.Contains(sb[i]); // if no trimChars given and char is whitespace OR char is in trimChars
                if (!shouldRemove)
                    break;

                toRemove++;
            }
            
            if (toRemove > 0)
                sb.Remove(0, toRemove);

            return sb;
        }

        /// <summary>
        /// Removes all trailing whitespace characters or specified characters from the current <see cref="StringBuilder"/> object.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> to trim.</param>
        /// <param name="trimChars">The characters to trim off.</param>
        public static StringBuilder TrimEnd(this StringBuilder sb, params char[] trimChars)
        {
            if (sb is null || sb.Length == 0) return sb;
            bool validTrimChars = !(trimChars is null || trimChars.Length == 0);

            int toRemove = 0;
            int i = sb.Length - 1;
            for (; i >= 0; i--)
            {
                bool shouldRemove = (!validTrimChars && char.IsWhiteSpace(sb[i])) || trimChars.Contains(sb[i]); // if no trimChars given and char is whitespace OR char is in trimChars
                if (!shouldRemove)
                    break;

                toRemove++;
            }

            if (toRemove > 0)
                sb.Remove(i + 1, toRemove);

            return sb;
        }
    }
}
