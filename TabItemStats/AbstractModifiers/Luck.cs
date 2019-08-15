using System;
using RoR2;
using UnityEngine;
using UnosMods.TabItemStats.Formatters;

namespace UnosMods.TabItemStats.AbstractModifiers
{
    public sealed class Luck : AbstractModifier
    {
        protected override Func<float, float> Func =>
            result => 1 - Mathf.Pow(1 - result, 1 + ContextProvider.ItemStacks(ItemIndex.Clover));
        protected override IStatFormatter Formatter => new ModifierFormatter("Luck", ItemIndex.Clover);
    }
}
