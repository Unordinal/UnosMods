using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace Unordinal.InventoryStats.Formatters
{
    public class PercentageModifierFormatter : ModifierFormatter
    {
        public PercentageModifierFormatter(string name, PickupIndex[] modifyingIndices = null, bool showStacks = true, string prefix = "", string suffix = "", string color = "", int places = 2, float effectiveMax = 0f) 
            : base(name, modifyingIndices, showStacks, prefix, suffix, color, places, effectiveMax) { }

        public override string Format(float value)
        {
            string valueStr = value.ToString("P", nfi);

            if (effectiveMax != 0f && value >= effectiveMax)
                valueStr += " [Effective Max]";

            return Format(valueStr);
        }
    }
}
