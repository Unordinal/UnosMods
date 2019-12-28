using RoR2;
using Unordinal.InventoryStats.Formatters;
using static Unordinal.StyleCatalog;

namespace Unordinal.InventoryStats.Stats.Modifiers
{
    public class DualBandsModifier : StatModifier
    {
        public static PickupIndex FireBand => PickupCatalog.FindPickupIndex(ItemIndex.FireRing);
        public static PickupIndex IceBand => PickupCatalog.FindPickupIndex(ItemIndex.IceRing);

        public static DualBandsModifier Instance { get; } = new DualBandsModifier();

        private DualBandsModifier()
        {
            ModifyingIndices = new[] { FireBand, IceBand };
            Formula = ModifierFormulas.DualBands;
            Formatter = new PercentageModifierFormatter(
                name: "Fire and Ice",
                modifyingIndices: ModifyingIndices,
                color: StyleIndex.cIsUtility.ToHex(),
                showStacks: false);
        }
    }
}
