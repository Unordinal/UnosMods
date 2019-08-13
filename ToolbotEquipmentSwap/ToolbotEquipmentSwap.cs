using System;
using RoR2;
using BepInEx;
using BepInEx.Configuration;
using MiniRpcLib.Action;
using MiniRpcLib.Func;
using UnityEngine;
using UnityEngine.Networking;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using R2API.Utils;
using System.Collections.Generic;
using EntityStates.Toolbot;
using System.Linq;

namespace UnosMods.ToolbotEquipmentSwap
{
    [BepInDependency(R2API.R2API.PluginGUID)]
    [BepInDependency(MiniRpcLib.MiniRpcPlugin.Dependency)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class ToolbotEquipmentSwap : BaseUnityPlugin
    {
        public const string PluginName = "ToolbotEquipmentSwap";
        public const string PluginGUID = "com.unordinal.toolbotequipmentswap";
        public const string PluginVersion = "1.0.0";
        private const string PluginRpcGUID = "UnosMods.ToolbotEquipmentSwap";

        public IRpcAction<ToolbotEquipmentSwapMessage> CmdSwitchEquipmentSlots;
        public IRpcFunc<string, bool> GetClientAutoSwapConfig { get; set; }

        public static ConfigWrapper<string> equipSwapKeyString;
        public static ConfigWrapper<bool> stopAutoSwap;
        public static KeyCode? equipSwapKey;

        public static Dictionary<NetworkUser, bool> usersAutoSwap = new Dictionary<NetworkUser, bool>();

        internal void Awake()
        {
            equipSwapKeyString = Config.Wrap("ToolbotEquipmentSwap", "SwapKey", "The key to swap between MUL-T's equipment slots. (Default: X)", KeyCode.X.ToString());
            equipSwapKey = GetKey(equipSwapKeyString);
            if (equipSwapKey == null)
                Logger.LogError("Invalid SwapKey for ToolbotEquipmentSwap.");

            stopAutoSwap = Config.Wrap("ToolbotEquipmentSwap", "StopAutoSwap", "(Server-side) Whether to stop the equipment slot changing when using MUL-T's Retool ability. (Default: true)", true);

            var miniRpc = MiniRpcLib.MiniRpc.CreateInstance(PluginRpcGUID);
            CmdSwitchEquipmentSlots = miniRpc.RegisterAction(MiniRpcLib.Target.Server, (Action<NetworkUser, ToolbotEquipmentSwapMessage>)DoSwitchEquipmentSlots);
            GetClientAutoSwapConfig = miniRpc.RegisterFunc<string, bool>(MiniRpcLib.Target.Client, ClientSendAutoSwapConfigValue);

            // Do IL replacement to stop automatic equipment slot swapping if wanted
            Logger.LogInfo("Modifying Toolbot Stances OnEnter()");
            IL.EntityStates.Toolbot.ToolbotStanceA.OnEnter += ToolbotStanceA_OnEnter;
            IL.EntityStates.Toolbot.ToolbotStanceB.OnEnter += ToolbotStanceB_OnEnter;
        }

        public void ToolbotStanceA_OnEnter(ILContext il)
        {
            try
            {
                var c = new ILCursor(il);
                c.GotoNext(
                    x => x.MatchCall<NetworkServer>("get_active"),                                  // NetworkServer.active
                    x => x.MatchBrfalse(out _),                                                     // break if above is false
                    x => x.MatchLdarg(0),                                                           // Argument 0 (OnEnter method itself)
                    x => x.MatchLdcI4(0),                                                           // 0 (byte, Equipment slot)
                    x => x.MatchCall<ToolbotStanceBase>("SetEquipmentSlot")    // SetEquipmentSlot((byte)LdcI4.0)
                    );
                c.Index += 2;                                                                       // Next = IL_0039: ldarg.0
                Logger.LogDebug(c);
                c.RemoveRange(3);
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Action<ToolbotStanceBase>> ((stance) =>
                {
                    NetworkUser user = stance.GetFieldValue<Inventory>("inventory")?.GetComponentInParent<PlayerCharacterMasterController>()?.networkUser;
                    if (user)
                    {
                        GetClientAutoSwapConfig.Invoke(null, result =>
                        {
                            bool stopAutoSwap = result;
                            Logger.LogDebug($"Invoke result [{user.userName}]: {result}");
                            if (!stopAutoSwap)
                                stance.InvokeMethod("SetEquipmentSlot", (byte)0);
                        }, user);
                    }
                });
                Logger.LogDebug(c);
                Logger.LogDebug(il.ToString());
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
                /*var c = new ILCursor(il);
                c.GotoNext(
                    x => x.MatchCall<NetworkServer>("get_active"),                                  // NetworkServer.active
                    x => x.MatchBrfalse(out _),                                                     // break if above is false
                    x => x.MatchLdarg(0),                                                           // Argument 0 (OnEnter method itself)
                    x => x.MatchLdcI4(1),                                                           // 1 (byte, Equipment slot)
                    x => x.MatchCall<ToolbotStanceBase>("SetEquipmentSlot")    // SetEquipmentSlot((byte)LdcI4.1)
                    );
                c.Index += 2;                                                                       // Next = IL_0039: ldarg.0
                Logger.LogDebug(c);
                c.RemoveRange(3);
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Action<ToolbotStanceBase>>((stance) =>
                {
                    NetworkUser user = stance.GetFieldValue<Inventory>("inventory")?.GetComponentInParent<PlayerCharacterMasterController>()?.networkUser;
                    if (user)
                    {
                        GetClientAutoSwapConfig.Invoke("this works, yay", result =>
                        {
                            bool stopAutoSwap = false;
                            stopAutoSwap = result;
                            Logger.LogDebug($"Invoke result [{user.userName}]: {result}");
                            if (stopAutoSwap)
                                return;
                        }, user);
                        stance.InvokeMethod("SetEquipmentSlot", (byte)1);
                    }
                });
                Logger.LogDebug(c);
                Logger.LogDebug(il.ToString());
                Logger.LogInfo("Modified ToolbotStanceB.OnEnter()");*/
            }
            catch (Exception e)
            {
                Logger.LogError($"There was an error modifying ToolbotStanceB.OnEnter():\n{e}");
            }
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
                CmdSwitchEquipmentSlots.Invoke(new ToolbotEquipmentSwapMessage());
                return;
            }

            Logger.LogDebug($"Attempting to switch equipment slots for host '{user.userName}'");

            if (SwitchEquipmentSlots(user.master?.inventory))
                Logger.LogInfo($"'{user.userName}' switched to equipment slot '{user.master.inventory.activeEquipmentSlot}'");
            else
                Logger.LogWarning($"Unable to switch equipment slots for host '{user.userName}'");
        }

        public void DoSwitchEquipmentSlots(NetworkUser user, ToolbotEquipmentSwapMessage message)
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

        public bool ClientSendAutoSwapConfigValue(NetworkUser user, string message)
        {
            if (!NetworkServer.active)
                return false;
            return stopAutoSwap.Value;
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

        public KeyCode? GetKey(ConfigWrapper<string> param)
        {
            if (!Enum.TryParse(param.Value, out KeyCode result))
                return null;
            return result;
        }

        internal void Update()
        {
            if (Run.instance)
                if (equipSwapKey != null && Input.GetKeyDown(equipSwapKey.Value))
                    InvokeSwitchEquipmentSlots();
        }
    }
}
