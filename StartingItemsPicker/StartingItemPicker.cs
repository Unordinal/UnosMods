using System;
using BepInEx;
using BepInEx.Configuration;
using RoR2;

namespace UnosMods.StartingItemsPicker
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.unordinal.startingitempicker", "Starting Item Picker", "1.0.0")]

    public class StartingItemPicker : BaseUnityPlugin
    {
        public void Awake()
        {
            On.RoR2.Run.Start += Run_Start;
        }

        void DisplayItemPicker()
        {
            ItemPickerWindow IPW = new ItemPickerWindow();
        }

        private void Run_Start(On.RoR2.Run.orig_Start orig, Run self)
        {
            orig(self);
            DisplayItemPicker();
        }
    }
}
