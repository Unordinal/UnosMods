using System.Linq;
using RoR2;
using Unordinal.InventoryStats.Formatters;
using Unordinal.InventoryStats.Providers;
using static Unordinal.StyleCatalog;

namespace Unordinal.InventoryStats.Stats.Modifiers
{
    public class TPHealingNovaModifier : StatModifier
    {
        public static PickupIndex TPHealingNova { get; } = PickupCatalog.FindPickupIndex(ItemIndex.TPHealingNova);

        public static TPHealingNovaModifier Instance { get; } = new TPHealingNovaModifier();

        private TPHealingNovaModifier()
        {
            ModifyingIndices = new[] { TPHealingNova };
            Formula = ModifierFormulas.TPHealingNova;
            Formatter = new ModifierFormatter(
                name: "Teammates' Daisies",
                modifyingIndices: ModifyingIndices,
                color: StyleIndex.cIsUtility.ToHex());
        }

        public static int GetTeamCount()
        {
            return ContextProvider.GetPickupCount(ContextProvider.GetAllPlayerBodies(), Instance.ModifyingIndices);
        }

        public static int GetOnlyTeamCount()
        {
            return ContextProvider.GetPickupCount(ContextProvider.GetAllPlayerBodiesExcept(), Instance.ModifyingIndices);
        }
    }
}
