using System;
using RoR2;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using R2API.Utils;

namespace ArtificerPrimFlamethrower
{

    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(R2API.R2API.PluginGUID)]
    public class ArtificerPrimFlamethrower : BaseUnityPlugin
    {
        public const string PluginName = "ArtificerPrimFlamethrower";
        public const string PluginVersion = "1.0.0";
        public const string PluginGUID = "com.Unordinal.ArtificerPrimFlamethrower";

        public static new ManualLogSource Logger { get; private set; }

        public void Awake()
        {
            Logger = base.Logger;
        }

        public void OnEnable()
        {
            AddHooks();
        }

        public void OnDisable()
        {
            RemoveHooks();
        }

        public void AddHooks()
        {
            On.RoR2.Skills.SkillCatalog.Init += ModifyArtificerSkills;
        }

        public void RemoveHooks()
        {
            On.RoR2.Skills.SkillCatalog.Init -= ModifyArtificerSkills;
        }

        private void ModifyArtificerSkills(On.RoR2.Skills.SkillCatalog.orig_Init orig)
        {
            orig();
            var arti = BodyCatalog.FindBodyPrefab("MageBody");
            var skillLoc = arti.GetComponent<SkillLocator>();
            var flamethrower = skillLoc.FindSkill("Flamethrower");
        }
    }
}
