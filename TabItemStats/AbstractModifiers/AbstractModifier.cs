﻿using System;
using UnosMods.TabItemStats.Formatters;

namespace UnosMods.TabItemStats.AbstractModifiers
{
    public abstract class AbstractModifier
    {
        //protected abstract Func<float, float> Func { get; }
        protected abstract IStatFormatter Formatter { get; }
        protected abstract float Func(float value, uint count = 1);
        public float GetInitialStat(float count) => Func(count);
        public string Format(float statValue) => Formatter.Format(statValue);
    }
}
