using UnityEngine;

namespace UnosUtilities
{
    public static class PurchaseInteractionExtensions
    {
        /// <summary>
        /// Returns a color based on the cost type (money, lunar coins, etc). Mostly used for coloring text.
        /// </summary>
        /// <param name="typeIndex">The <c>RoR2.CostTypeIndex</c> to use.</param>
        /// <returns>Returns a hexadecimal color string (ex: #FF0000).</returns>
        public static string GetColorFromPurchasableType(this RoR2.CostTypeIndex typeIndex)
        {
            switch (typeIndex)
            {
                case RoR2.CostTypeIndex.Money:
                    return RoR2Colors.Money;
                case RoR2.CostTypeIndex.PercentHealth:
                    return RoR2Colors.Blood;
                case RoR2.CostTypeIndex.Lunar:
                    return RoR2Colors.LunarCoin;
                case RoR2.CostTypeIndex.WhiteItem:
                    return RoR2Colors.Tier1Item;
                case RoR2.CostTypeIndex.GreenItem:
                    return RoR2Colors.Tier2Item;
                case RoR2.CostTypeIndex.RedItem:
                    return RoR2Colors.Tier3Item;
                case RoR2.CostTypeIndex.Equipment:
                    return RoR2Colors.Equipment;
                case RoR2.CostTypeIndex.VolatileBattery:
                    return RoR2Colors.Equipment;
                default:
                    Debug.LogWarning($"Invalid purchasable typeIndex '{typeIndex}' specified for GetColorFromPurchasableType()");
                    return RoR2Colors.Error;
            }
        }

        /// <summary>
        /// Returns a color based on the cost type (money, lunar coins, etc) of the RoR2.PurchaseInteraction. Mostly used for coloring text.
        /// </summary>
        /// <param name="PI">The RoR2.PurchaseInteraction to use.</param>
        /// <returns>Returns a hexadecimal color string (ex: #FF0000).</returns>
        public static string GetColorFromPurchasableType(this RoR2.PurchaseInteraction PI)
        {
            return GetColorFromPurchasableType(PI.costType);
        }

        /// <summary>
        /// Returns cost text based on the cost type (money, lunar coins, etc) of the RoR2.PurchaseInteraction (ex: $24, 50% HP, etc).
        /// </summary>
        /// <param name="PI">The RoR2.PurchaseInteraction to use.</param>
        /// <returns>Returns a cost text (ex: $24, 50% HP, etc).</returns>
        public static string GetTextFromPurchasableType(this RoR2.PurchaseInteraction PI)
        {
            string cost = PI.cost.ToString();
            switch (PI.costType)
            {
                case RoR2.CostTypeIndex.Money:
                    return $"${cost}";
                case RoR2.CostTypeIndex.PercentHealth:
                    return cost + "% HP";
                case RoR2.CostTypeIndex.Lunar:
                    return cost + " Lunar";
                case RoR2.CostTypeIndex.WhiteItem:
                    return cost + $" White Item{(PI.cost > 1 ? "s" : "")}";
                case RoR2.CostTypeIndex.GreenItem:
                    return cost + $" Green Item{(PI.cost > 1 ? "s" : "")}";
                case RoR2.CostTypeIndex.RedItem:
                    return cost + $" Red Item{(PI.cost > 1 ? "s" : "")}";
                case RoR2.CostTypeIndex.Equipment:
                    return cost + " Equipment";
                case RoR2.CostTypeIndex.VolatileBattery:
                    return cost + " Fuel Cell";
                default:
                    Debug.LogWarning($"Invalid purchasable costType '{PI.costType}' retrieved from '{PI.GetDisplayName()}' in GetTextFromPurchasableType(). Using default.");
                    return $"{cost} {PI.costType.ToString()}";
            }
        }
    }
}
