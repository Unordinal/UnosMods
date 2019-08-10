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
    }
}