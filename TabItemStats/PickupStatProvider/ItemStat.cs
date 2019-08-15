using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnosMods.TabItemStats.AbstractModifiers;
using UnosMods.TabItemStats.Formatters;

namespace UnosMods.TabItemStats
{
    public class ItemStat
    {
        private readonly Func<float, float> formula;
        public readonly IStatFormatter Formatter;
        public string StatText { get; }
        public bool DoesNotStack { get; }
        public AbstractModifier[] StatModifiers { get; }

        public ItemStat(Func<float, float> formula, string statText, IStatFormatter formatter = null, bool doesNotStack = false, params AbstractModifier[] modifiers)
        {
            this.formula = formula;
            StatText = statText;
            Formatter = formatter ?? new PercentageFormatter();
            DoesNotStack = doesNotStack;
            StatModifiers = modifiers;
        }

        public float GetInitialStat(float count)
        {
            return formula(count);
        }

        public IEnumerable<float> GetSubStats(float count)
        {
            var initialStat = GetInitialStat(count);
            foreach (var stat in StatModifiers)
                yield return stat.GetInitialStat(initialStat) - initialStat;
        }

        public string Format(float value)
        {
            return Formatter.Format(value);
        }

        public string FormatSubStats(float count)
        {
            var formattedValue = string.Empty;
            var result = formula(count);

            foreach (var stat in StatModifiers)
            {
                var valueDiff = stat.GetInitialStat(result) - result;
                if (Math.Round(valueDiff, 3) > 0)
                    formattedValue += "\n" + stat.Format(valueDiff);
            }
            return formattedValue;
        }
    }
}
