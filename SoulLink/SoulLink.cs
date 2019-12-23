using BepInEx;
using RoR2;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace SoulLink
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class SoulLink
    {
        public const string PluginName = "Soul Link";
        public const string PluginVersion = "1.0.0";
        public const string PluginGUID = "com.unordinal.soullink";
        public const string RpcGUID = "UnosMods.SoulLink";

        public static CharacterBody LocalCharacter { get; private set; }
        public static CharacterSoulLink LocalSoulLink { get; private set; }
        public static List<PickupIndex> AvailableRunItems { get; private set; }

        internal void Awake()
        {
            On.RoR2.Run.Start += Run_Start;
            if (NetworkServer.active)
                On.RoR2.CharacterBody.OnTakeDamageServer += CharacterBody_OnTakeDamageServer;
        }

        private void CharacterBody_OnTakeDamageServer(On.RoR2.CharacterBody.orig_OnTakeDamageServer orig, CharacterBody self, DamageReport damageReport)
        {
            orig(self, damageReport);
            damageReport.
        }

        private void Run_Start(On.RoR2.Run.orig_Start orig, Run self)
        {
            orig(self);
            AvailableRunItems = new List<PickupIndex>();
            AvailableRunItems.AddRange(Run.instance.availableLunarDropList);
            AvailableRunItems.AddRange(Run.instance.availableTier1DropList);
            AvailableRunItems.AddRange(Run.instance.availableTier2DropList);
            AvailableRunItems.AddRange(Run.instance.availableTier3DropList);
        }

        internal void Update()
        {
            var cachedLocalBody = LocalUserManager.GetFirstLocalUser().cachedBody;
            if (LocalCharacter != cachedLocalBody)
                LocalCharacter = cachedLocalBody;
        }
    }
}
