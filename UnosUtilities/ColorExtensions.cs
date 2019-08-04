using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;

namespace UnosUtilities
{
    public static class ColorExtensions
    {
        /// <summary>
        /// Takes a <c>Color32</c> and turns it into a hex color.
        /// </summary>
        /// <param name="rgb">The <c>Color32</c> to convert.</param>
        /// <returns>Returns a string containing a hexadecimal color (ex: #FF0000)</returns>
        public static string RGBToHex(this Color32 rgb)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0:X2}{1:X2}{2:X2}", rgb.r, rgb.g, rgb.b);
        }

        /// <summary>
        /// Takes a hex color and turns it into a <c>Color32</c>.
        /// </summary>
        /// <param name="hex">The hexadecimal color (ex: #FF0000) to convert.</param>
        /// <returns>Returns <c>UnityEngine.Color32</c></returns>
        public static Color32 HexToRGB(this string hex)
        {
            System.Drawing.Color parsedHex = System.Drawing.Color.FromArgb(int.Parse(hex.Replace("#", ""), NumberStyles.AllowHexSpecifier));
            return new Color32(parsedHex.R, parsedHex.G, parsedHex.B, 255);
        }

        /// <summary>
        /// Takes a hex color and splits it into a string[3]: RR, GG, BB
        /// </summary>
        /// <param name="hex">A hexadecimal color (ex: #FF0000)</param>
        /// <returns>Returns string[3]: RR, GG, BB</returns>
        public static string[] SplitHexColor(this string hex)
        {
            // Strip num. sign, we don't need it
            hex = hex.Replace("#", "");

            // Validate hex color - match 3 or 6 digits or characters A-F, case insensitive
            Regex rex = new Regex(@"^(?:[\da-f]{3}){1,2}$", RegexOptions.IgnoreCase);
            if (!rex.IsMatch(hex))
            {
                Debug.LogError("Hex Color passed to SplitHexColor is not valid.");
                return null;
            }

            // initialize array length
            string[] hexRGB = new string[3];
            // Take RGB from hex color
            if (hex.Length == 6)
            {
                hexRGB[0] = hex.Substring(0, 2);
                hexRGB[1] = hex.Substring(2, 2);
                hexRGB[2] = hex.Substring(4, 2);
            }
            else // Add implied hex (ex: FC3 -> FFCC33)
            {
                hexRGB[0] = hex.Substring(0, 1) + hex.Substring(0, 1);
                hexRGB[1] = hex.Substring(1, 1) + hex.Substring(1, 1);
                hexRGB[2] = hex.Substring(2, 1) + hex.Substring(2, 1);
            }

            return hexRGB;
        }
    }
}
