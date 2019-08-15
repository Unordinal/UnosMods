using System;
using RoR2;
using UnityEngine;
using UnosMods.TabItemStats.Formatters;

namespace UnosMods.TabItemStats.AbstractModifiers
{
    public sealed class MultiCount : AbstractModifier
    {
        protected override Func<float, uint, float> Func =>
            (damage, count) => ;
        protected override IStatFormatter Formatter => new ModifierFormatter("Luck", ItemIndex.Clover);
    }
}
