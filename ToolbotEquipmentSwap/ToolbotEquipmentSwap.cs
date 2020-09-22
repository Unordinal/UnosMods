using BepInEx;
using BepInEx.Logging;
using EntityStates.Toolbot;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API.Networking;
using R2API.Networking.Interfaces;
using R2API.Utils;
using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Unordinal.ToolbotEquipmentSwap.Networking;

namespace Unordinal.ToolbotEquipmentSwap
{
    [BepInDependency(R2API.R2API.PluginGUID)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [R2APISubmoduleDependency(nameof(NetworkingAPI))]
    [NetworkCompatibility(CompatibilityLevel.NoNeedForSync)]
    public class ToolbotEquipmentSwap : BaseUnityPlugin
    {
        public const string PluginName = "ToolbotEquipmentSwap";
        public const string PluginVersion = "1.1.0";
        public const string PluginGUID = "Unordinal.ToolbotEquipmentSwap";

        internal static new ManualLogSource Logger;

        public static readonly Dictionary<NetworkUser, (KeyCode SwapKey, bool SwapOnRetool)> ClientsSettings = new Dictionary<NetworkUser, (KeyCode, bool)>();

        internal void Awake()
        {
            Logger = base.Logger;
            PluginConfig.Initialize(Config);
            //On.RoR2.Networking.GameNetworkManager.OnClientConnect += (self, user, nc) => { };
            AddBaseHooks();
            AddStanceHooks();
        }

        internal void Start()
        {
            // register net messages
            //NetworkingAPI.RegisterRequestTypes<ClientConfigRequest, ClientConfigRequestReply>();
            NetworkingAPI.RegisterCommandType<ClientConfigCommand>();
            NetworkingAPI.RegisterMessageType<ClientConfigMessage>();
            NetworkingAPI.RegisterMessageType<EquipSwapMessage>();
        }

        internal void Update()
        {
            if (Run.instance && LocalUserManager.GetFirstLocalUser().cachedBody != null)
            {
                if (Input.GetKeyDown(PluginConfig.SwapKey))
                {
                    var body = LocalUserManager.GetFirstLocalUser().cachedBody;
                    var survivorIndex = SurvivorCatalog.GetSurvivorIndexFromBodyIndex(body.bodyIndex);
                    if (survivorIndex == SurvivorIndex.Toolbot)
                    {
                        new EquipSwapMessage
                        { 
                            NetUser = LocalUserManager.GetFirstLocalUser().currentNetworkUser 
                        }.Send(NetworkDestination.Server);
                    }
                    else
                    {
                        Logger.LogDebug($"bodyIndex of cachedBody is not Toolbot.\nBody: {Language.GetString(body.baseNameToken)}\nBodyIndex: {body.bodyIndex}\nSurvivorIndex: {survivorIndex}");
                    }
                }
            }
        }

        internal void AddBaseHooks()
        {
            Run.onRunStartGlobal += Run_onRunStartGlobal;
        }

        internal void RemoveBaseHooks()
        {
            Run.onRunStartGlobal -= Run_onRunStartGlobal;
        }

        internal void AddStanceHooks()
        {
            Logger.LogDebug("Adding IL hooks to Toolbot Stance EntityStates.");
            IL.EntityStates.Toolbot.ToolbotStanceA.OnEnter += ToolbotStanceA_OnEnter;
            IL.EntityStates.Toolbot.ToolbotStanceB.OnEnter += ToolbotStanceB_OnEnter;
        }

        internal void RemoveStanceHooks()
        {
            Logger.LogDebug("Removing IL hooks from Toolbot Stance EntityStates.");
            IL.EntityStates.Toolbot.ToolbotStanceA.OnEnter -= ToolbotStanceA_OnEnter;
            IL.EntityStates.Toolbot.ToolbotStanceB.OnEnter -= ToolbotStanceB_OnEnter;
        }

        private void Run_onRunStartGlobal(Run obj)
        {
            Config.Reload();

            if (NetworkServer.active)
            {
                ClientsSettings.Clear();
                //new ClientConfigRequest().Send<ClientConfigRequest, ClientConfigRequestReply>(NetworkDestination.Clients);
                new ClientConfigCommand().Send(NetworkDestination.Clients);
            }
        }

        private void HandleToolbotStanceChange(ToolbotStanceBase stance)
        {
            if (NetworkServer.active)
            {
                byte swapToSlot = (byte)((stance is ToolbotStanceA) ? 1 : 0);
                bool swapOnRetool = true;

                NetworkUser user = stance.inventory?.GetComponent<PlayerCharacterMasterController>()?.networkUser ?? null;
                if (ClientsSettings.ContainsKey(user))
                {
                    swapOnRetool = ClientsSettings[user].SwapOnRetool;
                }
                
                if (swapOnRetool) stance.SetEquipmentSlot(swapToSlot);
            }
        }

        private void ToolbotStanceA_OnEnter(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            bool success = c.TryGotoNext(
                x => x.MatchCall<NetworkServer>("get_active"),          // NetworkServer.active
                x => x.MatchBrfalse(out _),                             // Break if above is false
                x => x.MatchLdarg(0),                                   // Argument 0 (which is the enclosing OnEnter method itself)
                x => x.MatchLdcI4(0),                                   // 0 (byte, equipment slot 0)
                x => x.MatchCall<ToolbotStanceBase>("SetEquipmentSlot") // SetEquipmentSlot((byte)LdcI4.0)
                );

            if (success)
            {
                c.Index += 3;               // Jumps to 'x.MatchLdarg(0)'
                c.RemoveRange(2);           // Removes the next three instructions (Ldarg, LdcI4, Call)
                c.EmitDelegate<Action<ToolbotStanceBase>>(HandleToolbotStanceChange);
            }
            else
            {
                Logger.LogWarning($"Unable to IL hook 'EntityStates.Toolbot.ToolbotStanceA.OnEnter'. The mod may need an update.");
            }
        }
        
        private void ToolbotStanceB_OnEnter(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            bool success = c.TryGotoNext(
                x => x.MatchCall<NetworkServer>("get_active"),          // NetworkServer.active
                x => x.MatchBrfalse(out _),                             // Break if above is false
                x => x.MatchLdarg(0),                                   // Argument 0 (which is the enclosing OnEnter method itself)
                x => x.MatchLdcI4(1),                                   // 1 (byte, equipment slot 1)
                x => x.MatchCall<ToolbotStanceBase>("SetEquipmentSlot") // SetEquipmentSlot((byte)LdcI4.0)
                );

            if (success)
            {
                c.Index += 2;
                c.RemoveRange(3);
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Action<ToolbotStanceBase>>(HandleToolbotStanceChange);
            }
            else
            {
                Logger.LogWarning($"Unable to IL hook 'EntityStates.Toolbot.ToolbotStanceB.OnEnter'. The mod may need an update.");
            }
        }
    }
}
