using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UnosUtilities
{
    public static class StyleCatalog
    {
        private static readonly Color32[] indexToColor32 = new Color32[13];
        private static readonly string[] indexToHexString = new string[13];
        static StyleCatalog()
        {
            indexToColor32[0] = new Color32(255, 255, 255, byte.MaxValue);                                  // None
            indexToColor32[1] = new Color32(229, 201, 98, byte.MaxValue);                                   // cIsDamage
            indexToColor32[2] = new Color32(229, 130, 98, byte.MaxValue);                                   // cIsHealth
            indexToColor32[3] = new Color32(156, 229, 98, byte.MaxValue);                                   // cIsHealing
            indexToColor32[4] = new Color32(149, 205, 229, byte.MaxValue);                                  // cIsUtility
            indexToColor32[5] = new Color32(254, 124, 124, byte.MaxValue);                                  // cDeath
            indexToColor32[6] = new Color32(239, 235, 28, byte.MaxValue);                                   // cShrine
            indexToColor32[7] = new Color32(135, 142, 165, byte.MaxValue);                                  // cStack
            indexToColor32[8] = new Color32(204, 211, 224, byte.MaxValue);                                  // cSub
            indexToColor32[9] = new Color32(137, 144, 167, byte.MaxValue);                                  // cMono
            indexToColor32[10] = new Color32(131, 148, 179, byte.MaxValue);                                 // cEvent
            indexToColor32[11] = new Color32(255, 201, 245, byte.MaxValue);                                 // cWorldEvent
            indexToColor32[12] = new Color32(250, 250, 204, byte.MaxValue);                                 // cUserSetting

            indexToHexString = indexToColor32.Select(x => Util.RGBToHex(x)).ToArray();

            /*for (var i = 0; i < Enum.GetNames(typeof(StyleIndex)).Length; i++)
                indexToHexString[i] = Util.RGBToHex(indexToColor32[i]);*/
        }

        public static Color32 GetColor(StyleIndex index)
        {
            if (index < StyleIndex.None || index > StyleIndex.cUserSetting)
                index = StyleIndex.None;
            return indexToColor32[(int)index];
        }

        public static string GetColorHexString(StyleIndex index)
        {
            if (index < StyleIndex.None || index > StyleIndex.cUserSetting)
                index = StyleIndex.None;
            return indexToHexString[(int)index];
        }

        static uint Size(this StyleIndex style)
        {
            return 0;
        }

        public enum StyleIndex
        {
            None,
            cIsDamage,
            cIsHealth,
            cIsHealing,
            cIsUtility,
            cDeath,
            cShrine,
            cStack,
            cSub,
            cMono,
            cEvent,
            cWorldEvent,
            cUserSetting
        }
    }
}
