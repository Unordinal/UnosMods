using System;
using System.Collections.Generic;
using UnosMods.TabItemStats.AbstractModifiers;
using UnosMods.TabItemStats.Formatters;

namespace UnosMods.TabItemStats
{
    public class EquipmentStat
    {
        public readonly float value;
        public readonly IStatFormatter Formatter;
        public string StatText { get; }
        public AbstractModifier[] StatModifiers { get; }

        public EquipmentStat(float value, string statText, IStatFormatter formatter = null, params AbstractModifier[] modifiers)
        {
            this.value = value;
            StatText = statText;
            Formatter = formatter ?? new PercentageFormatter();
            StatModifiers = modifiers;
        }

        public float GetInitialStat()
        {
            return value;
        }

        public IEnumerable<float> GetSubStats()
        {
            var initialStat = GetInitialStat();
            foreach (var stat in StatModifiers)
                yield return stat.GetInitialStat(initialStat) - initialStat;
        }

        public string Format(float value)
        {
            return Formatter.Format(value);
        }

        public string FormatSubStats()
        {
            var formattedValue = string.Empty;

            foreach (var stat in StatModifiers)
            {
                var valueDiff = stat.GetInitialStat(1) - 1;
                if (Math.Round(valueDiff, 3) > 0)
                    formattedValue += stat.Format(valueDiff);
            }
            return formattedValue;
        }
    }
}
