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
            string reward = $"<color={RoR2Colors.Money}>${goldReward}</color>, <color={RoR2Colors.LunarCoin}>{expReward} EXP</color>";
            if (BI.InteractableIsUsedUp())
                reward = $"<color={RoR2Colors.Tier1ItemDark}>Opened</color>";

            string message = $"<color={RoR2Colors.Tier1ItemDark}>{displayName}: {reward}";
            return message;
        }

        public static string PurchasableMessage(PurchaseInteraction PI)
        {
            string displayName = $"{PI.GetDisplayName()}";
            string cost = PI.GetTextFromPurchasableType();
            string costColor = PI.GetColorFromPurchasableType();

            if (PI.InteractableIsUsedUp())
            {
                if (PI.IsContainer())
                    cost = "Opened";
                else if (PI.IsShrine())
                    cost = "Used";
                else
                    cost = "Unavailable";
                costColor = RoR2Colors.Tier1ItemDark;
            }
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
            string tierColor = $"#{ColorUtility.ToHtmlStringRGB(pickupIdx.GetPickupColor())}";
            string tierColorDark = $"#{ColorUtility.ToHtmlStringRGB(pickupIdx.GetPickupColorDark())}";
            if (STB.pickupIndexIsHidden)
            {
                string message = $"<color={tierColor}>??? ({cost}): <color={tierColorDark}>???</color>";
                return message;
            }
            return ItemMessage(pickupIdx, cost);
        }

        public static string CharacterHeaderMessage(CharacterBody CB)
        {
            CharacterMaster CM = CB.master;
            string nameColor = $"{(CB.teamComponent.teamIndex == TeamIndex.Player ? RoR2Colors.EasyDifficulty : RoR2Colors.HardDifficulty)}";
            string displayName = $"{CB.GetDisplayName()}";
            string level = $"{CB.level}";
            
            string message = $"<color={nameColor}>{displayName} (lv. {level}):</color>";
            return message;
        }

        public static string CharacterHealthStatsMessage(CharacterBody CB)
        {
            float cbHealth = CB.healthComponent.combinedHealth;
            float cbHealthMax = CB.healthComponent.fullCombinedHealth;
            string healthColor = $"{RoR2Colors.Tier1ItemDark}";
            string hpStats = $"<color={healthColor}>HP: <color=#{ColorExtensions.InterpolatedHealthColor(cbHealth, cbHealthMax)}>{CB.healthComponent.combinedHealth.ToString("n1")}</color>\\{CB.healthComponent.fullCombinedHealth.ToString("n1")}";

            return hpStats;
        }

        public static string CharacterDamageStatsMessage(CharacterBody CB)
        {
            float cbDamage = CB.damage;
            //float cbAtkSpd = Mathf.Max(CB.attackSpeed, 0.01f);
            //float cbDPS = cbDamage * cbAtkSpd;
            //string dmgStats = $"<color={RoR2Colors.Tier1ItemDark}>DPS: {cbDPS.ToString("n1")} (DMG: {cbDamage}, SPD: {cbAtkSpd})";
            string dmgStats = $"<color={RoR2Colors.Tier1ItemDark}>Damage: {cbDamage}</color>";

            return dmgStats;
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
