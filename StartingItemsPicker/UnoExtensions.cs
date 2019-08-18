using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;

namespace UnosMods.StartingItemsPicker
{
    public static class UnoExtensions
    {
        public static ItemIndex ReadItemIndex(this NetworkReader reader)
        {
            return (ItemIndex)reader.ReadUInt16();
        }

        public static void WriteItemIndex(this NetworkWriter writer, ItemIndex item)
        {
            writer.Write((ushort)item);
        }

        public static byte[] ShortToByteArr(this short s)
        {
            byte[] byteArr = new byte[2];
            byteArr[0] = (byte)(s >> 8);
            byteArr[1] = (byte) s;
            return byteArr;
        }

        public static short ByteArrToShort(this byte[] b)
        {
            return (short)((b[0] << 8) + b[1]);
        }

        public static byte[] IntToByteArr(this int i)
        {
            byte[] byteArr = new byte[4];
            byteArr[0] = (byte)(i >> 24);
            byteArr[1] = (byte)(i >> 16);
            byteArr[2] = (byte)(i >> 8);
            byteArr[3] = (byte) i;
            return byteArr;
        }

        public static int ByteArrToInt(this byte[] b)
        {
            int i = b[0] << 24 
                  | b[1] << 16 
                  | b[2] << 8 
                  | b[3];
            return i;
        }
    }
}
