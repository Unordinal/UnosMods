using System;
using System.Collections.Generic;
using Unordinal.InventoryStats.Formatters;
using Unordinal.InventoryStats.Stats.Modifiers;

namespace Unordinal.InventoryStats.Stats
{
    public class Stat
    {
        public string Text { get; }
        public bool Stacks { get; }
        public Func<float, float> Formula { get; }
        public StatFormatter Formatter { get; }
        public StatModifier[] Modifiers { get; }

        public Stat(string text, Func<float, float> formula, StatFormatter formatter, bool stacks = true, params StatModifier[] modifiers)
        {
            Text = text;
            Stacks = stacks;
            Formula = formula;
            Formatter = formatter;
            Modifiers = modifiers;

            if (!Stacks && string.IsNullOrWhiteSpace(Formatter?.Color))
                Formatter.Color = "#A0A0A0";
        }

        public string Format(string value)
        {
            return Formatter.Format(value);
        }
        
        public string Format(float value)
        {
            return Formatter.Format(value);
        }

        public float GetInitialStat(float count = 1)
        {
            return Formula is null ? 0f : Formula(count);
        }

        public IEnumerable<float> GetSubStats(float count = 1)
        {
            if (Formula is null) yield break;

            float currStat = Formula(count);
            foreach (var mod in Modifiers)
                yield return mod.Formula(currStat) - currStat;
        }

        public string FormatSubStats(float count = 1)
        {
            if (Formula is null) return null;

            string modifiersDesc = string.Empty;
            float currStat = Formula(count);

            foreach (var mod in Modifiers)
            {
                float diff = mod.Formula(currStat) - currStat;
                if (Math.Round(diff, 3) > 0)
                    modifiersDesc += "\n" + mod.Format(diff);
            }

            return modifiersDesc;
        }
    }
}
