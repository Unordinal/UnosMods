using BepInEx;
using BepInEx.Logging;
using R2API.Utils;
using RoR2;
using UnityEngine;
using static RoR2.DotController;

namespace Unordinal.ChatDeathCause
{
    [BepInDependency(R2API.R2API.PluginGUID)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [NetworkCompatibility(CompatibilityLevel.NoNeedForSync)]
    public class ChatDeathCause : BaseUnityPlugin
    {
        public const string PluginGUID = "Unordinal.ChatDeathCause";
        public const string PluginName = "ChatDeathCause";
        public const string PluginVersion = "1.0.0";
        public static new ManualLogSource Logger { get; private set; }


        // Called when the script instance is initialized. Only called once during the lifetime of the script.
        internal void Awake()
        {
            Logger = base.Logger;
            AddHooks();
        }

        /*internal void Update()
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                foreach (var cbody in CharacterBody.readOnlyInstancesList)
                {
                    if (cbody && cbody.inventory) cbody.inventory.GiveItem(ItemIndex.CritGlasses, 10);
                }
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                foreach (var cbody in CharacterBody.readOnlyInstancesList)
                {
                    if (cbody && cbody.inventory) cbody.inventory.GiveItem(ItemIndex.BleedOnHit, 10);
                }
            }
        }*/

        internal void AddHooks()
        {
            RemoveHooks();
            On.RoR2.GlobalEventManager.OnPlayerCharacterDeath += GlobalEventManager_OnPlayerCharacterDeath;
        }

        internal void RemoveHooks()
        {
            On.RoR2.GlobalEventManager.OnPlayerCharacterDeath -= GlobalEventManager_OnPlayerCharacterDeath;
        }

        private void GlobalEventManager_OnPlayerCharacterDeath(On.RoR2.GlobalEventManager.orig_OnPlayerCharacterDeath orig, GlobalEventManager self, DamageReport damageReport, NetworkUser netUser)
        {
            orig(self, damageReport, netUser);
            if (!netUser) return;

            var attackerBody = damageReport.attackerBody;
            var damageTaken = damageReport.damageInfo.damage;
            var healthLeft = damageReport.combinedHealthBeforeDamage;
            bool wasCrit = damageReport.damageInfo.crit;
            var dotType = damageReport.dotType;
            bool wasDot = dotType != DotIndex.None;
            string prefixStr = string.Empty;

            if (wasCrit || wasDot)
            {
                prefixStr = "<style=cDeath>";
                if (wasCrit)
                {
                    Color critColor = DamageColor.FindColor(DamageColorIndex.WeakPoint);
                    string critStr = Util.GenerateColoredString("Crit ", critColor);
                    prefixStr += critStr;
                }
                if (wasDot)
                {
                    string dotStr = DotIndexToString(dotType);
                    if (!string.IsNullOrWhiteSpace(dotStr))
                    {
                        Color dotColor = DamageColor.FindColor(damageReport.damageInfo.damageColorIndex);
                        dotStr = Util.GenerateColoredString($"{dotStr} ", dotColor);
                        prefixStr += dotStr;
                    }
                }
                prefixStr += "from </style>";
            }

            string text = $"<style=cDeath>Killed by: <style=cIsDamage>{prefixStr}{(attackerBody ? attackerBody.GetDisplayName() : "Unknown causes")}</style> <style=cIsUtility>({damageTaken:F2} damage dealt to '{netUser.userName}' with {healthLeft:F2} health left)</style></style>";

            Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = text });
        }

        private string DotIndexToString(DotIndex dotIndex)
        {
            switch (dotIndex)
            {
                case DotIndex.Bleed:
                    return "Bleed";
                case DotIndex.Blight:
                    return "Blight";
                case DotIndex.Burn:
                case DotIndex.PercentBurn:
                    return "Burn";
                case DotIndex.Helfire:
                    return "Helfire";
                case DotIndex.Poison:
                    return "Poison";
                case DotIndex.None:
                default:
                    return string.Empty;
            }
        }
    }
}
