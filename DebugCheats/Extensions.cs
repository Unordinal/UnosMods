using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace Unordinal.DebugCheats
{
    public static class Extensions
    {
        public static BuffIndex FindBuff(string buffName)
        {
            Enum.TryParse(buffName, true, out BuffIndex buff);
            if (Enum.IsDefined(typeof(BuffIndex), buff))
                return buff;
            else
                return BuffIndex.None;
        }
    }
}
