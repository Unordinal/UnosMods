//#define DEBUG
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using BepInEx;
using BepInEx.Configuration;
using MiniRpcLib;
using MiniRpcLib.Action;
using MiniRpcLib.Func;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using EntityStates.Toolbot;

namespace UnosMods.ToolbotEquipmentSwap
{
    [BepInDependency(R2API.R2API.PluginGUID)]
    [BepInDependency(MiniRpcPlugin.Dependency)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class ToolbotEquipmentSwap : BaseUnityPlugin
    {
        public const string PluginName = "ToolbotEquipmentSwap";
        public const string PluginGUID = "com.unordinal.toolbotequipmentswap";
        public const string PluginVersion = "1.0.1";
        private const string PluginRpcGUID = "UnosMods.ToolbotEquipmentSwap";

        public IRpcAction<bool> CmdSwitchEquipmentSlots { get; private set; }
        public IRpcFunc<bool, TESClientModConfig> CmdGetClientModConfig { get; private set; }

        public static ConfigEntry<string> equipSwapKeyString;
        public static ConfigEntry<bool> stopAutoSwap;
        public static KeyCode? equipSwapKey;

        public static Dictionary<NetworkUser, TESClientModConfig> usersModConfig = new Dictionary<NetworkUser, TESClientModConfig>();

        internal void Awake()
        {
            equipSwapKeyString = Config.Bind("ToolbotEquipmentSwap", "SwapKey", KeyCode.X.ToString(), "The key to swap between MUL-T's equipment slots. (Default: X)");
            stopAutoSwap = Config.Bind("ToolbotEquipmentSwap", "StopAutoSwap", true, "Whether to stop the equipment slot changing when using MUL-T's Retool ability. (Default: true)");

            equipSwapKey = GetKey(equipSwapKeyString);
            if (equipSwapKey == null)
                Logger.LogError("Invalid SwapKey for ToolbotEquipmentSwap.");

            var miniRpc = MiniRpc.CreateInstance(PluginRpcGUID);
            CmdSwitchEquipmentSlots = miniRpc.RegisterAction<bool>(Target.Server, DoSwitchEquipmentSlots);
            CmdGetClientModConfig = miniRpc.RegisterFunc<bool, TESClientModConfig>(Target.Client, (user, b) =>
            {
                return new TESClientModConfig(equipSwapKeyString.Value, stopAutoSwap.Value);
            });

            // Do IL replacement to stop automatic equipment slot swapping if wanted
            Logger.LogInfo("Modifying ToolbotStances A & B OnEnter()");
            IL.EntityStates.Toolbot.ToolbotStanceA.OnEnter += ToolbotStanceA_OnEnter;
            IL.EntityStates.Toolbot.ToolbotStanceB.OnEnter += ToolbotStanceB_OnEnter;
        }

        private void ToolbotStanceA_OnEnter(ILContext il)
        {
            try
            {
                var c = new ILCursor(il);
                c.GotoNext(
                    x => x.MatchCall<NetworkServer>("get_active"),          // NetworkServer.active
                    x => x.MatchBrfalse(out _),                             // break if above is false
                    x => x.MatchLdarg(0),                                   // Argument 0 (OnEnter method itself)
                    x => x.MatchLdcI4(0),                                   // 0 (byte, Equipment slot)
                    x => x.MatchCall<ToolbotStanceBase>("SetEquipmentSlot") // SetEquipmentSlot((byte)LdcI4.0)
                    );
                Logger.LogDebug(c);
                c.Index += 2;
                c.RemoveRange(3);
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Action<ToolbotStanceBase>>(stance =>
                {
                    NetworkUser user = stance.GetFieldValue<Inventory>("inventory")?.GetComponent<PlayerCharacterMasterController>().networkUser ?? null;
                    var stopAutoSwap = false;

                    var config = GetClientModConfig(user);
                    if (config != null)
                        stopAutoSwap = config.StopAutoSwap;

                    if (!stopAutoSwap)
                        stance.InvokeMethod("SetEquipmentSlot", (byte)0);
                });
#if (DEBUG)
                Logger.LogDebug(il);
#endif
                Logger.LogInfo("Modified ToolbotStanceA.OnEnter()");
            }
            catch (Exception e)
            {
                Logger.LogError($"There was an error modifying ToolbotStanceA.OnEnter():\n{e}");
            }
        }

        private void ToolbotStanceB_OnEnter(ILContext il)
        {
            try
            {
                var c = new ILCursor(il);
                c.GotoNext(
                    x => x.MatchCall<NetworkServer>("get_active"),          // NetworkServer.active
                    x => x.MatchBrfalse(out _),                             // break if above is false
                    x => x.MatchLdarg(0),                                   // Argument 0 (OnEnter method itself)
                    x => x.MatchLdcI4(1),                                   // 1 (byte, Equipment slot)
                    x => x.MatchCall<ToolbotStanceBase>("SetEquipmentSlot") // SetEquipmentSlot((byte)LdcI4.1)
                    );
                Logger.LogDebug(c);
                c.Index += 2;
                c.RemoveRange(3);
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Action<ToolbotStanceBase>>(stance =>
                {
                    NetworkUser user = stance.GetFieldValue<Inventory>("inventory")?.GetComponent<PlayerCharacterMasterController>().networkUser ?? null;
                    var stopAutoSwap = false;

                    var config = GetClientModConfig(user);
                    if (config != null)
                        stopAutoSwap = config.StopAutoSwap;

                    if (!stopAutoSwap)
                        stance.InvokeMethod("SetEquipmentSlot", (byte)1);
                });
#if (DEBUG)
                Logger.LogDebug(il);
#endif
                Logger.LogInfo("Modified ToolbotStanceB.OnEnter()");
            }
            catch (Exception e)
            {
                Logger.LogError($"There was an error modifying ToolbotStanceB.OnEnter():\n{e}");
            }
        }

        public TESClientModConfig GetClientModConfig(NetworkUser user)
        {
            if (!user)
                return null;

            if (!usersModConfig.ContainsKey(user))
            {
                usersModConfig.Add(user, null);
                CmdGetClientModConfig.Invoke(true, CMC =>
                {
                    if (CMC != null)
                    {
                        usersModConfig[user] = CMC;
                    }
                }, user);
            }
            return usersModConfig[user];
        }

        public void InvokeSwitchEquipmentSlots()
        {
            var user = LocalUserManager.GetFirstLocalUser().currentNetworkUser;
            if (!user)
            {
                Logger.LogWarning("Attempt to invoke SwitchEquipmentSlots on invalid user.");
                return;
            }

            if (!NetworkServer.active)
            {
                CmdSwitchEquipmentSlots.Invoke(true);
                return;
            }

            Logger.LogDebug($"Attempting to switch equipment slots for host '{user.userName}'");

            if (SwitchEquipmentSlots(user.master?.inventory))
                Logger.LogInfo($"'{user.userName}' switched to equipment slot '{user.master.inventory.activeEquipmentSlot}'");
            else
                Logger.LogWarning($"Unable to switch equipment slots for host '{user.userName}'");
        }

        public void DoSwitchEquipmentSlots(NetworkUser user, bool b)
        {
            Logger.LogDebug($"Attempting to switch equipment slots for client '{user.userName}'");

            if (SwitchEquipmentSlots(user.master?.inventory))
                Logger.LogInfo($"'{user.userName}' switched to equipment slot '{user.master.inventory.activeEquipmentSlot}'");
            else
                Logger.LogWarning($"Unable to switch equipment slots for client '{user.userName}'");
        }

        public bool SwitchEquipmentSlots(Inventory inv)
        {
            var body = inv.GetComponentInParent<PlayerCharacterMasterController>()?.master?.GetBody();
            if (!inv || !body || !BodyIsSurvivorIndex(body, SurvivorIndex.Toolbot))
            {
                Logger.LogWarning($"SwitchEquipmentSlots was called on an invalid character. ({(inv ? (body ? "Character is not MUL-T." : "Body is null.") : "Inventory is null.")})");
                return false;
            }

            if (inv.activeEquipmentSlot == 0)
                inv.SetActiveEquipmentSlot(1);
            else
                inv.SetActiveEquipmentSlot(0);
            return true;
        }

        public bool BodyIsSurvivorIndex(CharacterBody body, SurvivorIndex index)
        {
            var bodyPrefab = body?.master?.bodyPrefab;
            if (!bodyPrefab)
            {
                Logger.LogWarning($"Body passed to BodyIsSurvivorIndex is invalid. ({(body ? "bodyPrefab is null." : "Body is null.")})");
                return false;
            }
            return SurvivorCatalog.FindSurvivorDefFromBody(bodyPrefab)?.survivorIndex == index;
        }

        public KeyCode? GetKey(ConfigEntry<string> param)
        {
            if (!Enum.TryParse(param.Value, out KeyCode result))
                return null;
            return result;
        }

        internal void Update()
        {
            if (Run.instance && LocalUserManager.GetFirstLocalUser()?.currentNetworkUser?.GetCurrentBody() != null)
                if (equipSwapKey != null && Input.GetKeyDown(equipSwapKey.Value))
                    InvokeSwitchEquipmentSlots();
        }
    }
}
