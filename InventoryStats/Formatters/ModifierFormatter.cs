using RoR2;
using Unordinal.InventoryStats.Providers;

namespace Unordinal.InventoryStats.Formatters
{
    public class ModifierFormatter : NumStatFormatter
    {
        protected readonly string name;
        protected readonly PickupIndex[] modifyingIndices;
        protected readonly bool showStacks;

        public ModifierFormatter(string name, PickupIndex[] modifyingIndices = null, bool showStacks = true, string prefix = "", string suffix = "", string color = "", int places = 0, float effectiveMax = 0f) : base(prefix, suffix, color, places, effectiveMax)
        {
            this.name = name;
            this.modifyingIndices = modifyingIndices ?? new[] { PickupIndex.none };
            this.showStacks = showStacks;
        }

        public override string Format(string value)
        {
            int stacks = ContextProvider.GetPickupCount(modifyingIndices);
            string stacksStr = string.Empty;
            if (showStacks)
                stacksStr += $" ({stacks})";

            return $"\t{name}{stacksStr}: {base.Format(value)}";
        }

        public override string Format(float value)
        {
            string valueStr = value.ToString(nfi);

            if (effectiveMax != 0f && value >= effectiveMax)
                valueStr += " [Effective Max]";

            return Format(valueStr);
        }
    }
}
