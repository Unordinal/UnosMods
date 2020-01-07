﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIBlacklister
{
    public static class Util
    {
        public static T[] Add<T>(this T[] array, params T[] items)
        {
            return (array ?? Enumerable.Empty<T>()).Concat(items).ToArray();
        }

        public static T[] Remove<T>(this T[] array, params T[] items)
        {
            return (array ?? Enumerable.Empty<T>()).Except(items).ToArray();
        }
    }
}
