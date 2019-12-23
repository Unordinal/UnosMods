using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoulLink
{
    public static class Extensions
    {
        public static void RemoveAllItems(this Inventory inv)
        {
            if (!Run.instance)
                return;

            foreach (var item in SoulLink.AvailableRunItems)
                inv.RemoveItem(item.itemIndex, int.MaxValue);
        }
    }
}
