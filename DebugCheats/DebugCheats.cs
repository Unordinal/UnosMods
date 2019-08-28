﻿using System;
using BepInEx;
using BepInEx.Configuration;
using RoR2;
using R2API.Utils;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Networking;
using static UnosMods.DebugCheats.Extensions;
using DebugCheats;

namespace UnosMods.DebugCheats
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.unordinal.debugcheats", "Debug Cheats", "1.0.0")]

    public class DebugCheats : BaseUnityPlugin
    {
        List<PickupIndex> tier1Drops = new List<PickupIndex>();
        List<PickupIndex> tier2Drops = new List<PickupIndex>();
        List<PickupIndex> tier3Drops = new List<PickupIndex>();
        List<PickupIndex> lunarDrops = new List<PickupIndex>();
        List<PickupIndex> allDrops = new List<PickupIndex>();
        List<PickupIndex> equipmentDrops = new List<PickupIndex>();
        List<PickupIndex> lunarEquipment = new List<PickupIndex>();
        bool givenMoney = false;
        bool godMode = false;

        public DebugCheats()
        {
            On.RoR2.RoR2Application.UnitySystemConsoleRedirector.Redirect += orig => { }; // Stop in-game console from redirecting from base cmd console.
            On.RoR2.Console.Awake += (orig, self) =>
            {
                CommandHelper.RegisterCommands(self);
                orig(self);
            };
        }

        public void Update()
        {
            if (Run.instance)
            {
                if (!tier1Drops.Any() || !tier2Drops.Any() || !tier3Drops.Any() || !lunarDrops.Any() || !allDrops.Any() || !equipmentDrops.Any())
                {
                    tier1Drops = Run.instance.availableTier1DropList;
                    tier2Drops = Run.instance.availableTier2DropList;
                    tier3Drops = Run.instance.availableTier3DropList;
                    lunarDrops = Run.instance.availableLunarDropList.Where(t => t.equipmentIndex == EquipmentIndex.None).ToList();
                    allDrops = tier1Drops.Concat(tier2Drops.Concat(tier3Drops)).ToList();
                    equipmentDrops = Run.instance.availableEquipmentDropList;
                    lunarEquipment = Run.instance.availableLunarDropList.Where(t => t.equipmentIndex != EquipmentIndex.None).ToList();
                }
                else if (NetworkServer.active)
                {
                    var player = PlayerCharacterMasterController.instances[0];
                    var inv = player.master?.inventory;

                    if (!givenMoney)
                    {
                        player.master.GiveMoney(1000000);
                        givenMoney = true;
                    }
                    if (Input.GetKeyDown(KeyCode.Alpha0))
                    {
                        godMode = !godMode;
                        foreach (var netPlayer in NetworkUser.readOnlyInstancesList)
                            netPlayer.GetCurrentBody().healthComponent.godMode = godMode;
                        Chat.AddMessage($"Godmode {godMode}");
                    }
                    if (Input.GetKeyDown(KeyCode.F1))
                    {
                        for (var i = 0; i < 10; i++)
                            inv.GiveItem(GetRandomDropFromList(tier3Drops).itemIndex);
                    }
                    if (Input.GetKeyDown(KeyCode.F2))
                    {
                        inv.GiveItem(ItemIndex.SprintBonus, 15);
                        inv.GiveItem(ItemIndex.Feather, 15);
                    }
                    if (Input.GetKeyDown(KeyCode.F3))
                    {
                        var transform = player.master.GetBody().coreTransform;
                        PickupDropletController.CreatePickupDroplet(GetRandomDropFromList(allDrops), transform.position, transform.forward * 20f);
                    }
                    if (Input.GetKeyDown(KeyCode.F4))
                    {
                        var transform = player.master.GetBody().coreTransform;
                        PickupDropletController.CreatePickupDroplet(GetRandomDropFromList(equipmentDrops), transform.position, transform.forward * 20f);
                        PickupDropletController.CreatePickupDroplet(GetRandomDropFromList(lunarEquipment), transform.position, transform.forward * 20f);
                    }
                    if (Input.GetKeyDown(KeyCode.F5))
                    {
                        foreach (var item in allDrops)
                            inv.GiveItem(item.itemIndex);
                    }
                    if (Input.GetKeyDown(KeyCode.F6))
                    {
                        foreach (var item in lunarDrops)
                            inv.GiveItem(item.itemIndex);
                    }
                    if (Input.GetKey(KeyCode.F7))
                    {
                        player.master.GiveMoney(100);
                    }
                    if (Input.GetKeyDown(KeyCode.F8))
                    {
                        TeamManager.instance?.SetTeamLevel(TeamIndex.Player, TeamManager.instance.GetTeamLevel(TeamIndex.Player) + 1);
                    }
                    if (Input.GetKeyDown(KeyCode.F9))
                    {
                        Run.instance.AdvanceStage(Run.instance.nextStageScene);
                    }
                }
            }
        }

        public PickupIndex GetRandomDropFromList(List<PickupIndex> list)
        {
            if (!list.Any() && (!tier1Drops.Any() || !tier2Drops.Any() || !tier3Drops.Any() || !allDrops.Any() || !equipmentDrops.Any()))
                return PickupIndex.none;
            else if (list.Any())
                return list[Run.instance.treasureRng.RangeInt(0, list.Count)];
            else
                return allDrops[Run.instance.treasureRng.RangeInt(0, allDrops.Count)];
        }

        [ConCommand(commandName = "dc_givebuff", flags = ConVarFlags.ExecuteOnServer, helpText = "Gives a buff.\n\t[0]: buffName")]
        public static void CCGiveBuff(ConCommandArgs args)
        {
            if (args.Count == 0)
                return;

            string buffName = args[0];
            try
            {
                var userBody = args.sender?.GetCurrentBody();
                BuffIndex buff = FindBuff(buffName);
                if (userBody && buff != BuffIndex.None)
                    userBody.AddBuff(buff);
                else if (!userBody)
                    throw new Exception("Invalid body");
                else if (buff == BuffIndex.None)
                    throw new Exception($"Invalid buff '{buffName}'");
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        [ConCommand(commandName = "dc_removebuff", flags = ConVarFlags.ExecuteOnServer, helpText = "Removes a buff.\n\t[0]: buffName")]
        public static void CCRemoveBuff(ConCommandArgs args)
        {
            if (args.Count == 0)
                return;

            string buffName = args[0];
            CharacterBody playerBody = string.IsNullOrEmpty(args[1]) 
                ? args[1].Convert<ulong>().GetNetworkUser()?.GetCurrentBody() 
                : args.sender?.GetCurrentBody();
            try
            {
                BuffIndex buff = FindBuff(buffName);
                if (playerBody && buff != BuffIndex.None)
                    playerBody.RemoveBuff(buff);
                else if (!playerBody)
                    throw new Exception("Invalid body");
                else if (buff == BuffIndex.None)
                    throw new Exception($"Invalid buff '{buffName}'");
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        [ConCommand(commandName = "dc_loadscene", flags = ConVarFlags.ExecuteOnServer, helpText = "Loads the specified scene.\n\t[0]: sceneName")]
        public static void CCLoadScene(ConCommandArgs args)
        {
            if (args.Count == 0)
                return;

            string sceneName = args[0];
            try
            {
                if (!SceneCatalog.allSceneDefs.Contains(SceneCatalog.GetSceneDefFromSceneName(sceneName)))
                    throw new Exception();
                Run.instance.AdvanceStage(sceneName);
            }
            catch
            {
                Debug.LogError($"Invalid scene name '{sceneName}'");
            }
        }

        [ConCommand(commandName = "dc_listscenes", flags = ConVarFlags.None, helpText = "Lists valid scenes.\n\t[0]: bool includeInvalid: Lists all scenes if true.")]
        public static void CCListScenes(ConCommandArgs args)
        {
            try
            {
                bool includeInvalid = false;
                if (args.Count > 0)
                    bool.TryParse(args[0], out includeInvalid);

                var sceneNamePadding = SceneCatalog.allSceneDefs.Aggregate((max, cur) => max.sceneName.Length > cur.sceneName.Length ? max : cur).sceneName.Length + 2;
                var nameTokenPadding = Language.GetString(SceneCatalog.allSceneDefs.Aggregate((max, cur) => Language.GetString(max.nameToken).Length > Language.GetString(cur.nameToken).Length ? max : cur).nameToken).Length + 2; // this is a mess of a line, ey?
                /*string codeNameHeader = PadBoth("Code name", sceneNamePadding);
                string sceneTitleHeader = PadBoth("Scene title", nameTokenPadding);*/ // Not monospace console lulllllll bleh

                Debug.Log("Code name: Scene title");
                Debug.Log(new string('-', sceneNamePadding + nameTokenPadding));
                foreach (var scene in SceneCatalog.allSceneDefs.Where(x => includeInvalid || (x.sceneType == SceneType.Stage || x.sceneType == SceneType.Intermission)))
                    Debug.Log($"{scene.sceneName}: {(!Language.GetString(scene.nameToken).IsNullOrWhiteSpace() ? Language.GetString(scene.nameToken) : scene.sceneName)}");
            }
            catch (Exception exc)
            {
                Debug.LogError($"{exc}");
            }
        }
    }
}