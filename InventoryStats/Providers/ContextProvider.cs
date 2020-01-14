using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RoR2;
using Unordinal.InventoryStats.Stats.Modifiers;

namespace Unordinal.InventoryStats.Providers
{
    public static class ContextProvider
    {
        public static CharacterBody GetLocalBody(int userID = 0)
        {
            return userID == 0
                ? LocalUserManager.GetFirstLocalUser()?.currentNetworkUser?.GetCurrentBody()
                : LocalUserManager.FindLocalUser(userID)?.currentNetworkUser?.GetCurrentBody();
        }

        public static ReadOnlyCollection<NetworkUser> GetNetworkUsers()
        {
            return NetworkUser.readOnlyInstancesList;
        }

        public static IEnumerable<CharacterBody> GetAllPlayerBodies()
        {
            return GetNetworkUsers().Select(user => user.GetCurrentBody());
        }
        
        public static IEnumerable<CharacterBody> GetAllPlayerBodiesExcept(CharacterBody body = null)
        {
            var exceptBody = body ?? GetLocalBody();
            return GetAllPlayerBodies().Where(b => b != exceptBody);
        }

        public static int GetPickupCount(this Inventory inv, PickupIndex pickupIndex)
        {
            PickupDef pickupDef = PickupCatalog.GetPickupDef(pickupIndex);
            if (inv is null || pickupDef is null) return 0;

            return inv.GetItemCount(pickupDef.itemIndex);
        }
        
        public static int GetPickupCount(this Inventory inv, IEnumerable<PickupIndex> pickupIndices)
        {
            return pickupIndices?.Sum(inv.GetPickupCount) ?? 0;
        }

        public static int GetPickupCount(this CharacterBody body, PickupIndex pickupIndex)
        {
            Inventory inv = body?.inventory;
            return inv.GetPickupCount(pickupIndex);
        }
        
        public static int GetPickupCount(this CharacterBody body, IEnumerable<PickupIndex> pickupIndices)
        {
            Inventory inv = body?.inventory;
            return inv.GetPickupCount(pickupIndices);
        }
        
        public static int GetPickupCount(this IEnumerable<CharacterBody> bodies, IEnumerable<PickupIndex> pickupIndices)
        {
            if (bodies is null || pickupIndices is null) return 0;

            return bodies.Sum(body => body.GetPickupCount(pickupIndices));
        }

        public static int GetPickupCount(PickupIndex pickupIndex, int userID = 0)
        {
            Inventory inv = GetLocalBody(userID)?.inventory;
            return GetPickupCount(inv, pickupIndex);
        }
        
        public static int GetPickupCount(IEnumerable<PickupIndex> pickupIndices, int userID = 0)
        {
            Inventory inv = GetLocalBody(userID)?.inventory;
            if (inv is null || pickupIndices is null) return 0;

            return pickupIndices.Sum(inv.GetPickupCount);
        }

        public static bool PlayerHasModifyingItems(IEnumerable<StatModifier> modifiers, int userID = 0)
        {
            return modifiers?.Any(m => GetPickupCount(m.ModifyingIndices, userID) > 0) ?? false;
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
