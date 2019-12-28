using RoR2;
using UnityEngine;
using Unordinal.InventoryStats.Providers;

namespace Unordinal.InventoryStats.Stats
{
    public static class StackFormulas
    {
        public delegate float StackFormula(float count, float stackValue, float initialValue = 0f);

        public static float Linear(float count, float stackValue, float initialValue = 0f)
        {
            return initialValue > 0f ? ((count - 1) * stackValue + initialValue) : (count * stackValue);
        }

        public static float Exponential(float count, float stackValue, float initialValue = 0f, bool decreasing = false)
        {
            if (initialValue == 0f)
                initialValue = 1f;
            else
                count -= 1;

            float result = Mathf.Pow(1.0f - stackValue, count) * initialValue;
            return decreasing ? result : 1 - result;
        }

        public static float Hyperbolic(float count, float stackValue)
        {
            return 1 - 1 / ((stackValue * count) + 1);
        }

        // Math is not my strong suit. Not sure if this has a specific name but the wiki calls it Exponential... which it also uses for an entirely different equation (that's probably very similar to this one somehow but whatever).
        public static float Fractional(float dividend, float divisor)
        {
            return dividend / divisor;
        }

        public static class Special
        {
            public static float Bandolier(float count)
            {
                return 1 - 1 / Mathf.Pow(count + 1, 0.33f);
            }

            public static float GoldOnHit(float count)
            {
                return count * (Run.instance?.difficultyCoefficient ?? 1f) * 3;
            }

            public static int GoldGatBulletCost()
            {
                return (int)(1f * (1f + (ContextProvider.GetTeamLevel() - 1f) * 0.25f));
            }
        }
    }
}
