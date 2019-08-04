using System;
using BepInEx;
using BepInEx.Configuration;
using RoR2;
using R2API.Utils;
using UnityEngine;

namespace UnosMods.DebugCheats
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.unordinal.debugcheats", "Debug Cheats", "1.0.0")]

    public class DebugCheats : BaseUnityPlugin
    {
        public void Update()
        {
            if (Run.instance)
            {
                if (Run.instance.participatingPlayerCount == 1)
                {
                    if (Input.GetKeyDown(KeyCode.F1))
                    {
                        var dropList = Run.instance.availableTier3DropList;
                        var inv = PlayerCharacterMasterController.instances[0].master.inventory;

                        inv.GiveItem(dropList[Run.instance.treasureRng.RangeInt(0, dropList.Count)].itemIndex);
                        inv.GiveItem(dropList[Run.instance.treasureRng.RangeInt(0, dropList.Count)].itemIndex);
                        inv.GiveItem(dropList[Run.instance.treasureRng.RangeInt(0, dropList.Count)].itemIndex);
                        inv.GiveItem(dropList[Run.instance.treasureRng.RangeInt(0, dropList.Count)].itemIndex);
                        inv.GiveItem(dropList[Run.instance.treasureRng.RangeInt(0, dropList.Count)].itemIndex);
                        inv.GiveItem(dropList[Run.instance.treasureRng.RangeInt(0, dropList.Count)].itemIndex);
                        inv.GiveItem(dropList[Run.instance.treasureRng.RangeInt(0, dropList.Count)].itemIndex);
                        inv.GiveItem(dropList[Run.instance.treasureRng.RangeInt(0, dropList.Count)].itemIndex);
                        inv.GiveItem(dropList[Run.instance.treasureRng.RangeInt(0, dropList.Count)].itemIndex);
                        inv.GiveItem(dropList[Run.instance.treasureRng.RangeInt(0, dropList.Count)].itemIndex);
                    }
                    if (Input.GetKeyDown(KeyCode.F2))
                    {
                        var inv = PlayerCharacterMasterController.instances[0].master.inventory;

                        inv.GiveItem(ItemIndex.SprintBonus, 15);
                        inv.GiveItem(ItemIndex.Feather, 15);
                    }
                }
            }
        }
    }
}