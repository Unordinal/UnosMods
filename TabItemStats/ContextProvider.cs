using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnosMods.TabItemStats
{
    public static class ContextProvider
    {
        public static CharacterBody GetLocalBody(int userId = 0)
        {
            if (userId == 0)
                return LocalUserManager.GetFirstLocalUser()?.currentNetworkUser?.GetCurrentBody();
            return LocalUserManager.FindLocalUser(userId)?.currentNetworkUser?.GetCurrentBody();
        }
        public static bool PlayerIsValid(int userId = 0)
        {
            var body = GetLocalBody(userId);
            if (body)
                return true;
            return false;
        }

        public static int ItemStacks(ItemIndex item, int userId = 0) =>
            GetLocalBody(userId)?.ItemStacks(item) ?? 0;
        public static float PlayerMaxHealth(int userId = 0) =>
            GetLocalBody(userId)?.maxHealth ?? 0;
        public static float PlayerDamage(int userId = 0) =>
            GetLocalBody(userId)?.damage ?? 0;
        public static float PlayerAttackSpeed(int userId = 0) =>
            GetLocalBody(userId)?.attackSpeed ?? 0;
        public static float PlayerArmor(int userId = 0) =>
            GetLocalBody(userId)?.armor ?? 0;
        public static float PlayerRegen(int userId = 0) =>
            GetLocalBody(userId)?.regen ?? 0;
        public static float PlayerSpeed(int userId = 0) =>
            GetLocalBody(userId)?.moveSpeed ?? 0;

        public static IEnumerable<CharacterBody> GetPlayerBodiesExcept(int userId)
        {
            return LocalUserManager.readOnlyLocalUsersList
                .Where(user => user.id != userId)
                .Select(user => user.cachedBody);
        }

        public static uint TeamLevel(TeamIndex index = TeamIndex.Player)
        {
            return TeamManager.instance?.GetTeamLevel(index) ?? 0;
        }

        public static float ScaleByLevel(float value, TeamIndex index = TeamIndex.Player)
        {
            return TeamLevel(index) > 0 ? TeamLevel(index) * value : 0f;
        }
    }
}
