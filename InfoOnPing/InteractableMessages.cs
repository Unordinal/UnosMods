using RoR2;
using UnityEngine;
using UnosUtilities;

namespace InfoOnPing
{
    public static class ExtPingMessages
    {
        public static string BarrelMessage(BarrelInteraction BI)
        {
            string displayName = $"{BI.GetDisplayName()}";
            string goldReward = $"{BI.goldReward}";
            string expReward = $"{BI.expReward.ToString("n0")}";
            string message = $"<color={RoR2Colors.Tier1ItemDark}>{displayName}: " +         // Name
                             $"<color={RoR2Colors.Money}>${goldReward}</color>, " +         // Money Reward
                             $"<color={RoR2Colors.LunarCoin}>{expReward} EXP</color> ";     // EXP Reward
            return message;
        }

        public static string PurchasableMessage(PurchaseInteraction PI)
        {
            string displayName = $"{PI.GetDisplayName()}";
            string cost = PI.GetTextFromPurchasableType();
            string costColor = PI.GetColorFromPurchasableType();
            string message = $"<color={RoR2Colors.Tier1ItemDark}>{displayName}:</color> <color={costColor}>{cost}</color>";
            return message;
        }

        public static string ItemMessage(PickupIndex pickupIdx, string cost = null)
        {
            ItemDef itemDef = ItemCatalog.GetItemDef(pickupIdx.itemIndex);

            string displayName = $"{Language.GetString(pickupIdx.GetPickupNameToken())}";
            string description;
            string tierColor = $"#{ColorUtility.ToHtmlStringRGB(pickupIdx.GetPickupColor())}";
            string tierColorDark = $"#{ColorUtility.ToHtmlStringRGB(pickupIdx.GetPickupColorDark())}";
            if (cost != null)
                cost = $" ({cost})";

            if (itemDef.descriptionToken != null && UnosMods.InfoOnPing.InfoOnPing.UseLongDescription.Value) // If config is set to show long description and object has valid long description
                description = Language.GetString(itemDef.descriptionToken);
            else
                description = Language.GetString(itemDef.pickupToken) ?? Language.GetString(itemDef.descriptionToken); // Null checking because why not?

            string message = $"<color={tierColor}>{displayName}{cost}: <color={tierColorDark}>{description}</color>";
            return message;
        }

        public static string EquipmentMessage(PickupIndex pickupIdx)
        {
            EquipmentDef equipDef = EquipmentCatalog.GetEquipmentDef(pickupIdx.equipmentIndex);

            string displayName = $"{Language.GetString(pickupIdx.GetPickupNameToken())}";
            string description;
            string tierColor = $"#{ColorUtility.ToHtmlStringRGB(pickupIdx.GetPickupColor())}";
            string tierColorDark = $"#{ColorUtility.ToHtmlStringRGB(pickupIdx.GetPickupColorDark())}";

            if (equipDef.descriptionToken != null && UnosMods.InfoOnPing.InfoOnPing.UseLongDescription.Value) // If config is set to show long description and object has valid long description
                description = Language.GetString(equipDef.descriptionToken);
            else
                description = Language.GetString(equipDef.pickupToken) ?? Language.GetString(equipDef.descriptionToken); // Null checking because why not?

            string message = $"<color={tierColor}>{displayName}: <color={tierColorDark}>{description}</color>";
            return message;
        }

        public static string ShopTerminalMessage(ShopTerminalBehavior STB)
        {
            PickupIndex pickupIdx = STB.CurrentPickupIndex();
            PurchaseInteraction PI = STB.GetComponent<PurchaseInteraction>();
            string cost = $"<color={PI.GetColorFromPurchasableType()}>{PI.GetTextFromPurchasableType()}</color>";
            if (STB.pickupIndexIsHidden)
            {
                string message = $"<color={RoR2Colors.Unlockable}>??? ({cost}): <color={RoR2Colors.Unlockable}>???</color>";
                return message;
            }
            return ItemMessage(pickupIdx, cost);
        }

        public static string CharacterMessage(CharacterBody CB)
        {
            CharacterMaster CM = CB.master;
            string message = $"{CB.GetDisplayName()}: HP {CB.healthComponent.combinedHealth}\\{CB.healthComponent.fullCombinedHealth}";
            return message;
        }

        public static string ExpMessage()
        {
            TeamManager TMI = TeamManager.instance;
            string nextLevel = (TMI.GetTeamLevel(TeamIndex.Player) + 1).ToString();
            string expCurrent = TMI.GetTeamExperience(TeamIndex.Player).ToString("n0");
            string expNextLevel = TMI.GetTeamNextLevelExperience(TeamIndex.Player).ToString("n0");
            string expUntilNextLevel = (TMI.GetTeamNextLevelExperience(TeamIndex.Player) - TMI.GetTeamExperience(TeamIndex.Player)).ToString("n0");

            string message = $"{expUntilNextLevel} more EXP needed for level {nextLevel} [{expCurrent}\\{expNextLevel}]";
            return message;
        }
    }
}
