using System;
using BepInEx;
using BepInEx.Configuration;
using RoR2;
using R2API.Utils;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace UnosMods.DebugCheats
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.unordinal.debugcheats", "Debug Cheats", "1.0.0")]

    public class DebugCheats : BaseUnityPlugin
    {
        List<PickupIndex> tier1Drops = new List<PickupIndex>();
        List<PickupIndex> tier2Drops = new List<PickupIndex>();
        List<PickupIndex> tier3Drops = new List<PickupIndex>();
        List<PickupIndex> allDrops = new List<PickupIndex>();
        List<PickupIndex> equipmentDrops = new List<PickupIndex>();
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
                if (!tier1Drops.Any() || !tier2Drops.Any() || !tier3Drops.Any() || !allDrops.Any() || !equipmentDrops.Any())
                {
                    tier1Drops = Run.instance.availableTier1DropList;
                    tier2Drops = Run.instance.availableTier2DropList;
                    tier3Drops = Run.instance.availableTier3DropList;
                    allDrops = tier1Drops.Concat(tier2Drops.Concat(tier3Drops)).ToList();
                    equipmentDrops = Run.instance.availableEquipmentDropList;
                }
                else if (NetworkServer.active)
                {
                    var player = PlayerCharacterMasterController.instances[0];
                    var inv = player.master?.inventory;

                    if (!givenMoney)
                    {
                        player.master.GiveMoney(100000);
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
                    if (Input.GetKey(KeyCode.F4))
                    {
                        var transform = player.master.GetBody().coreTransform;
                        PickupDropletController.CreatePickupDroplet(GetRandomDropFromList(equipmentDrops), transform.position, transform.forward * 20f);
                    }
                    if (Input.GetKey(KeyCode.F5))
                    {
                        player.master.GiveMoney(100);
                    }
                    if (Input.GetKey(KeyCode.F6))
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

        public static string PadBoth(string source, int length) //https://stackoverflow.com/a/17590723/
        {
            int spaces = length - source.Length;
            int padLeft = spaces / 2 + source.Length;
            return source.PadLeft(padLeft).PadRight(length);
        }

        /*[ConCommand(commandName = "list_components", 
            flags = ConVarFlags.ExecuteOnServer, 
            helpText = "List components of the object under your crosshairs. Add an index to list components of the component whose index is entered. Can enter multiple, dot-separated indices, ex: 0.2.1")]
        private static void CCListComponents(ConCommandArgs args)
        {
            CharacterBody body = args.sender.master?.GetBody();
            if (!body)
                return;
            HurtBox hurtBox = null;
            RaycastHit raycastHit;
            if (Util.CharacterRaycast(body, aimRay, out raycastHit, 1000f, LayerIndex.world.mask | LayerIndex.defaultLayer.mask, QueryTriggerInteraction.Collide))
            {
                hurtBox = raycastHit.collider.GetComponent<HurtBox>();
            }
        }*/
    }
}