using UnityEngine;
using RoR2;
using RoR2.Skills;
using EntityStates;
using BepInEx;
using BepInEx.Logging;
using R2API;
using R2API.AssetPlus;
using R2API.Utils;
using EngineerPlus.Skills.Primary.PlasmoidLaunchers;
using static EngineerPlus.LoadoutHelper;

namespace EngineerPlus
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(R2API.R2API.PluginGUID)]
    [R2APISubmoduleDependency(nameof(EntityAPI), nameof(LoadoutAPI), nameof(SkillAPI), nameof(AssetPlus), nameof(PrefabAPI))]
    public class PluginEntry : BaseUnityPlugin
    {
        public const string PluginName = "Engineer+";
        public const string PluginVersion = "0.0.1";
        public const string PluginGUID = "com.unordinal.engineerplus";
        public static new ManualLogSource Logger { get; private set; }

        public static GameObject EngiBody { get; private set; }

        public void Awake()
        {
            Logger = base.Logger;

            if (!CanLoad()) return;
            
            EngiBody = Resources.Load<GameObject>("Prefabs/CharacterBodies/EngiBody");
            if (!EngiBody)
            {
                Logger.LogError("EngiBody not found, aborting.");
                return;
            }

            AddLanguageTokens();
            AddPlasmoidLaunchers();
        }

        private bool CanLoad()
        {
            try
            {
                var stateTest = new SerializableEntityStateType(typeof(ChargePlasmoids));
                stateTest.stateType.ToString();
                return true;
            }
            catch
            {
                Logger.LogError("Missing R2API Submodule! R2API may be installed incorrectly.");
                return false;
            }
        }

        private void AddPlasmoidLaunchers()
        {
            SkillAPI.AddSkill(typeof(ChargePlasmoids));
            SkillFamily skillFamily = LoadoutHelper.GetSkillFamily(EngiBody, SkillSlot.Primary);
            ViewablesCatalog.Node viewableNode = new ViewablesCatalog.Node("UnoEngiPrimary", false);
            Sprite icon = Resources.Load<Sprite>("UnoTODOActualIcon");

            NewSkillInfo nsi = new NewSkillInfo
            {
                activationState = new SerializableEntityStateType(typeof(ChargePlasmoids)),
                activationStateMachineName = "Weapon",
                icon = icon,
                viewableNode = viewableNode,
                unlockableName = string.Empty,
                skillName = "Plasmoid Launchers",
                skillNameToken = "UNO_ENGIPLUS_PLASMOIDS_NAME",
                skillDescriptionToken = "UNO_ENGIPLUS_PLASMOIDS_DESC",
                interruptPriority = InterruptPriority.Skill,
                baseRechargeInterval = 0f,
                baseMaxStock = 1,
                rechargeStock = 1,
                isBullets = false,
                shootDelay = 0f,
                beginSkillCooldownOnSkillEnd = false,
                requiredStock = 1,
                stockToConsume = 1,
                canceledFromSprinting = false,
                noSprint = false,
                isCombatSkill = true,
                mustKeyPress = true,
                fullRestockOnAssign = true
            };

            LoadoutHelper.AddSkillVariant(skillFamily, nsi);
        }

        private void AddLanguageTokens()
        {
            Languages.AddToken("UNO_ENGIPLUS_PLASMOIDS_NAME", "Plasmoid Launchers");
            Languages.AddToken("UNO_ENGIPLUS_PLASMOIDS_DESC", "Charge and fire alternating plasma shots for <style=cIsDamage>350% damage</style> each.");
        }
    }
}
