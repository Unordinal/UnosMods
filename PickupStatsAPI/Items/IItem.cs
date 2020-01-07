using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using PickupStatsAPI.Stats;
using RoR2;

namespace PickupStatsAPI.Items
{
    public abstract class Item
    {
        protected readonly List<Stat> stats;

        /// <summary>
        /// A read-only list of the item's stats. Use the item specific properties to make changes to its stats.
        /// </summary>
        public ReadOnlyCollection<Stat> Stats => stats.AsReadOnly();
        public abstract void ApplyHook();
        public void blah()
        {
            var itemDef = new RoR2.ItemDef();
            var item = new R2API.CustomItem(itemDef, null);
            item.ItemDef.
            R2API.ItemAPI.AddCustomItem();
        }
    }
}
