using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using R2API.Utils;

namespace PickupStatsAPI.Hooks
{
    public static class CharacterBody
    {
        private static void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, RoR2.CharacterBody self)
        {
            self.SetPropertyValue("experience", TeamManager.instance.GetTeamExperience(self.teamComponent.teamIndex));
            self.SetPropertyValue("level", TeamManager.instance.GetTeamLevel(self.teamComponent.teamIndex));
            self.SetPropertyValue("isElite", self.GetFieldValue<int>("eliteBuffCount"));

            float levelMod = self.level - 1f;

            float maxHealth = self.maxHealth;
            float maxShield = self.maxShield;
            float maxHealthLeveled = self.baseMaxHealth + (self.levelMaxHealth * levelMod);

            float maxHealthMod = 1f;
        }

    }
}
