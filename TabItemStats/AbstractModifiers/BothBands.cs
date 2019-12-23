using System;
using UnityEngine;
using UnosMods.TabItemStats.Formatters;

namespace UnosMods.TabItemStats.AbstractModifiers
{
    public sealed class BothBands : AbstractModifier
    {
        protected override Func<float, float> Func =>
            result => 1 - Mathf.Pow(1 - result, 
                ((ContextProvider.ItemStacks(RoR2.ItemIndex.IceRing) > 0 ? 1 : 0) + 
                (ContextProvider.ItemStacks(RoR2.ItemIndex.FireRing) > 0 ? 1 : 0)
                ));
        protected override IStatFormatter Formatter => new ModifierFormatter("Both Bands");
    }
}
