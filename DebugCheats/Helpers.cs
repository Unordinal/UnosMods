using RoR2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace DebugCheats
{
    public static class Helpers
    {
        public static T Convert<T>(this string str)
        {
            try
            {
                var converter = TypeDescriptor.GetConverter(typeof(T));
                if (converter != null)
                    return (T)converter.ConvertFromString(str);
                return default;
            }
            catch (NotSupportedException)
            {
                return default;
            }
        }

        public static NetworkUser GetNetworkUser(this ulong networkId)
        {
            var user = NetworkUser.readOnlyInstancesList.Where(x => x.Network_id.value == networkId).FirstOrDefault();
            return user ?? LocalUserManager.GetFirstLocalUser()?.currentNetworkUser;
        }

        public static string PadBoth(this string source, int length) //https://stackoverflow.com/a/17590723/
        {
            int spaces = length - source.Length;
            int padLeft = spaces / 2 + source.Length;
            return source.PadLeft(padLeft).PadRight(length);
        }
    }
}
