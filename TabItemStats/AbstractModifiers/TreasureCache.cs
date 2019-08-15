using System;
using System.Linq;
using UnosMods.TabItemStats.Formatters;

namespace UnosMods.TabItemStats.AbstractModifiers
{
    public sealed class TreasureCache : AbstractModifier
    {
        protected override Func<float, float> Func =>
            count =>
            {
                return ContextProvider.GetPlayerBodiesExcept(0)
                .Sum(body => body.ItemStacks(RoR2.ItemIndex.TreasureCache)) + count;
            };
        protected override IStatFormatter Formatter => new ModifierFormatter("From Teammates' Keys");
    }
}
