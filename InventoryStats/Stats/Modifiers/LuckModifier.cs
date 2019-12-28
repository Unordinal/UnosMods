using RoR2;
using Unordinal.InventoryStats.Formatters;
using static Unordinal.StyleCatalog;

namespace Unordinal.InventoryStats.Stats.Modifiers
{
    public class LuckModifier : StatModifier
    {
        public static PickupIndex Clover { get; } = PickupCatalog.FindPickupIndex(ItemIndex.Clover);

        public static LuckModifier Instance { get; } = new LuckModifier();

        private LuckModifier()
        {
            ModifyingIndices = new[] { Clover };
            Formula = ModifierFormulas.Luck;
            Formatter = new PercentageModifierFormatter(
                name: "Luck",
                modifyingIndices: ModifyingIndices,
                color: StyleIndex.cIsUtility.ToHex());
        }
    }
}
