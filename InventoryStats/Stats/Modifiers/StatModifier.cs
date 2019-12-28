using RoR2;
using Unordinal.InventoryStats.Formatters;
using static Unordinal.InventoryStats.Stats.Modifiers.ModifierFormulas;

namespace Unordinal.InventoryStats.Stats.Modifiers
{
    public abstract class StatModifier
    {
        public PickupIndex[] ModifyingIndices { get; protected set; }
        public ModifierFormula Formula { get; protected set; }
        public ModifierFormatter Formatter { get; protected set; }

        public string Format(string value)
        {
            return Formatter.Format(value);
        }

        public string Format(float value)
        {
            return Formatter.Format(value);
        }

        public float GetInitialStat(float count) => Formula(count);
    }
}
