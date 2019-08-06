using System;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using InfoOnPing;
using R2API.Utils;
using RoR2;
using UnityEngine;
using UnosUtilities;

namespace UnosMods.InfoOnPing
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.unordinal.infoonping", "Info On Ping", "1.1.0")]

    public class InfoOnPing : BaseUnityPlugin
    {
        public static ConfigWrapper<bool> UseLongDescription;
        public void Awake()
        {
            UseLongDescription = Config.Wrap("InfoOnPing", "UseLongDescription", "Whether to show the long description in chat when a pickup is pinged.", false);

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            On.RoR2.UI.PingIndicator.RebuildPing += (orig, self) =>
            {
                orig(self);
                try
                {
                    if (self.GetFieldValue<RoR2.UI.PingIndicator.PingType>("pingType") == RoR2.UI.PingIndicator.PingType.Interactable) // If object is an interactable
                    {
                        GameObject target = self.pingTarget;
                        BarrelInteraction BI = target.GetComponent<BarrelInteraction>(); // Barrel Interactable info
                        PurchaseInteraction PI = target.GetComponent<PurchaseInteraction>(); // Purchase Interactable info
                        GenericPickupController GPC = target.GetComponent<GenericPickupController>(); // Generic item pickup

                        if (BI)
                        {
                            Chat.AddMessage(ExtPingMessages.BarrelMessage(BI));        // Send chat message with name and rewards (gold, exp). Also shows exp needed for next level.
                            Chat.AddMessage(ExtPingMessages.ExpMessage());
                        }
                        else if (PI && PI.costType != CostTypeIndex.None) // If object is purchasable and has a valid cost type
                        {
                            if (PI.GetComponent<ShopTerminalBehavior>()) // If object is a shop terminal, don't use normal purchasable behavior.
                                Chat.AddMessage(ExtPingMessages.ShopTerminalMessage(PI.GetComponent<ShopTerminalBehavior>())); // Send chat message with shown pickup name, cost and description
                            else
                                Chat.AddMessage(ExtPingMessages.PurchasableMessage(PI));   // Send chat message with name and price info (ex: Chest: $22, Shrine of Blood: 50% HP, etc).
                        }
                        else if (GPC) // If object is a pickup
                        {
                            PickupIndex pickupIdx = GPC.pickupIndex;
                            if (pickupIdx.itemIndex != ItemIndex.None)
                                Chat.AddMessage(ExtPingMessages.ItemMessage(pickupIdx));        // Send chat message with item name and description.
                            else if (pickupIdx.equipmentIndex != EquipmentIndex.None)
                                Chat.AddMessage(ExtPingMessages.EquipmentMessage(pickupIdx));   // Send chat message with equipment name and description.
                        }
                    }
                    else if (self.GetFieldValue<RoR2.UI.PingIndicator.PingType>("pingType") == RoR2.UI.PingIndicator.PingType.Enemy) // If object is an enemy/character
                    {
                        GameObject target = self.pingTarget;
                        CharacterBody CB = target.GetComponent<CharacterBody>();
                        
                        if (CB)
                        {
                            Chat.AddMessage(ExtPingMessages.CharacterHeaderMessage(CB));
                            Chat.AddMessage(ExtPingMessages.CharacterHealthStatsMessage(CB));
                            Chat.AddMessage(ExtPingMessages.CharacterDamageStatsMessage(CB));
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            };
        }

        // You'd think this would be built-in! I'm stubborn though and don't want to copy all of these .cs files to each project.
        // Code snagged from https://www.youtube.com/watch?v=x-KK7bmo1AM
        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("InfoOnPing.UnosUtilities.dll"))
            {
                byte[] assemblyData = new byte[stream.Length];
                stream.Read(assemblyData, 0, assemblyData.Length);
                return Assembly.Load(assemblyData);
            }
        }
    }
}