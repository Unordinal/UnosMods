using System.Collections.Generic;
using System.Linq;
using RoR2;
using Unordinal.InventoryStats.Stats.Modifiers;

namespace Unordinal.InventoryStats.Providers
{
    public static class ContextProvider
    {
        public static CharacterBody GetLocalBody(int? userID = null)
        {
            return userID is null
                ? LocalUserManager.GetFirstLocalUser()?.currentNetworkUser?.GetCurrentBody()
                : LocalUserManager.FindLocalUser(userID.Value)?.currentNetworkUser?.GetCurrentBody();
        }

        public static IEnumerable<CharacterBody> GetAllPlayerBodies()
        {
            return NetworkUser.readOnlyInstancesList
                .Select(user => user?.master?.GetBody());
        }
        
        public static IEnumerable<CharacterBody> GetAllPlayerBodiesExcept(CharacterBody body = null)
        {
            body = body ?? GetLocalBody();
            return NetworkUser.readOnlyInstancesList
                .Where(user => user?.master?.GetBody() != body)
                .Select(user => user?.master?.GetBody());
        }

        public static bool PlayerBodyIsValid(int? userID = null)
        {
            return GetLocalBody(userID);
        }

        public static int GetPickupCount(this Inventory inv, PickupIndex pickupIndex)
        {
            PickupDef pickupDef = PickupCatalog.GetPickupDef(pickupIndex);
            if (pickupDef is null) return 0;

            return inv?.GetItemCount(pickupDef.itemIndex) ?? 0;
        }
        
        public static int GetPickupCount(this CharacterBody body, PickupIndex pickupIndex)
        {
            Inventory inv = body?.inventory;
            return GetPickupCount(inv, pickupIndex);
        }
        
        public static int GetPickupCount(this CharacterBody body, PickupIndex[] pickupIndices)
        {
            Inventory inv = body?.inventory;
            if (inv is null) return 0;

            int total = 0;
            foreach (var pickup in pickupIndices)
            {
                total += GetPickupCount(inv, pickup);
            }
            return total;
        }

        public static int GetPickupCount(PickupIndex pickupIndex, int? userID = null)
        {
            Inventory inv = GetLocalBody(userID)?.inventory;
            return GetPickupCount(inv, pickupIndex);
        }
        
        public static int GetPickupCount(PickupIndex[] pickupIndices, int? userID = null)
        {
            Inventory inv = GetLocalBody(userID)?.inventory;
            if (inv is null) return 0;

            int total = 0;
            foreach (var pickup in pickupIndices)
            {
                total += GetPickupCount(inv, pickup);
            }
            return total;
        }

        public static bool PlayerHasModifyingItems(StatModifier[] modifiers, int? userID = null)
        {
            return modifiers.Any(m => GetPickupCount(m.ModifyingIndices, userID) > 0);
        }

        public static int GetScaledGoldCost(int cost)
        {
            return Run.instance?.GetDifficultyScaledCost(cost) ?? 0;
        }

        public static uint GetTeamLevel(TeamIndex team = TeamIndex.Player)
        {
            return TeamManager.instance?.GetTeamLevel(team) ?? 0;
        }
    }
}
