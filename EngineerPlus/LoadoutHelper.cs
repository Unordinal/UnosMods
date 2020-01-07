using System;
using UnityEngine;
using RoR2;
using RoR2.Skills;
using EntityStates;

namespace EngineerPlus
{
    /// <summary>
    /// Most code lovingly stolen from Rein#7551 on the RoR2 Modding Discord.
    /// </summary>
    public static class LoadoutHelper
    {
        public static void AddSkillVariant(SkillFamily skillFamily, NewSkillInfo nsi)
        {
            SkillDef skillDef = ScriptableObject.CreateInstance<SkillDef>();
            skillDef.activationState = nsi.activationState;
            skillDef.activationStateMachineName = nsi.activationStateMachineName;
            skillDef.icon = nsi.icon;
            skillDef.skillName = nsi.skillName;
            skillDef.skillNameToken = nsi.skillNameToken;
            skillDef.skillDescriptionToken = nsi.skillDescriptionToken;
            skillDef.interruptPriority = nsi.interruptPriority;
            skillDef.baseRechargeInterval = nsi.baseRechargeInterval;
            skillDef.baseMaxStock = nsi.baseMaxStock;
            skillDef.rechargeStock = nsi.rechargeStock;
            skillDef.isBullets = nsi.isBullets;
            skillDef.shootDelay = nsi.shootDelay;
            skillDef.beginSkillCooldownOnSkillEnd = nsi.beginSkillCooldownOnSkillEnd;
            skillDef.requiredStock = nsi.requiredStock;
            skillDef.stockToConsume = nsi.stockToConsume;
            skillDef.isCombatSkill = nsi.isCombatSkill;
            skillDef.noSprint = nsi.noSprint;
            skillDef.canceledFromSprinting = nsi.canceledFromSprinting;
            skillDef.mustKeyPress = nsi.mustKeyPress;
            skillDef.fullRestockOnAssign = nsi.fullRestockOnAssign;

            SkillFamily.Variant variant = default;
            variant.skillDef = skillDef;
            variant.viewableNode = nsi.viewableNode;
            variant.unlockableName = nsi.unlockableName;

            int famVariantLength = skillFamily.variants.Length;
            Array.Resize(ref skillFamily.variants, famVariantLength + 1);
            skillFamily.variants[famVariantLength] = variant;
        }

        public static SkillFamily GetSkillFamily(GameObject prefab, SkillSlot slot)
        {
            SkillLocator skillLocator = prefab.GetComponent<SkillLocator>();

            if (skillLocator)
            {
                switch (slot)
                {
                    case SkillSlot.Primary:
                        return skillLocator.primary.skillFamily;
                    case SkillSlot.Secondary:
                        return skillLocator.secondary.skillFamily;
                    case SkillSlot.Utility:
                        return skillLocator.utility.skillFamily;
                    case SkillSlot.Special:
                        return skillLocator.special.skillFamily;
                    default:
                        return skillLocator.primary.skillFamily;
                }
            }

            return null;
        }

        public static SkillFamily GetSkillFamily(CharacterBody body, SkillSlot slot)
        {
            return GetSkillFamily(body.gameObject, slot);
        }

        public struct NewSkillInfo
        {
            public SerializableEntityStateType activationState;
            public string activationStateMachineName;
            public Sprite icon;
            public ViewablesCatalog.Node viewableNode;
            public string unlockableName;
            public string skillName;
            public string skillNameToken;
            public string skillDescriptionToken;
            public InterruptPriority interruptPriority;
            public float baseRechargeInterval;
            public int baseMaxStock;
            public int rechargeStock;
            public bool isBullets;
            public float shootDelay;
            public bool beginSkillCooldownOnSkillEnd;
            public int requiredStock;
            public int stockToConsume;
            public bool isCombatSkill;
            public bool noSprint;
            public bool canceledFromSprinting;
            public bool mustKeyPress;
            public bool fullRestockOnAssign;
        }
    }
}
