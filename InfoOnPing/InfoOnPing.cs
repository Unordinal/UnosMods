using System;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using R2API.Utils;
using RoR2;
using UnityEngine;
using UnosUtilities;

namespace UnosMods.InfoOnPing
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.unordinal.infoonping", "Info On Ping", "1.0.0")]

    public class InfoOnPing : BaseUnityPlugin
    {
        private static ConfigWrapper<bool> UseLongDescription;
        public void Awake()
        {
            UseLongDescription = Config.Wrap("InfoOnPing", "UseLongDescription", "Whether to show the long description in chat when a pickup is pinged.", false);

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            On.RoR2.UI.PingIndicator.RebuildPing += (orig, self) =>
            {
                orig(self);
                try
                {
                    if (self.GetFieldValue<RoR2.UI.PingIndicator.PingType>("pingType") == RoR2.UI.PingIndicator.PingType.Interactable) // Object is an interactable
                    {
                        PurchaseInteraction PI = self.pingTarget.GetComponent<PurchaseInteraction>(); // Purchase Interactable info
                        GenericPickupController GPC = self.pingTarget.GetComponent<GenericPickupController>(); // Generic item pickup

                        if (PI && PI.costType != CostTypeIndex.None) // Object is purchasable and has a valid cost type
                        {
                            string displayName = $"{PI.GetDisplayName()}";
                            string cost = PI.GetTextFromPurchasableType();
                            string costColor = PI.GetColorFromPurchasableType();
                            string message = $"<color={RoR2Colors.Tier1ItemDark}>{displayName}:</color> <color={costColor}>{cost}</color>";
                            Chat.AddMessage(message); // Send a chat message with name and price info (ex: Chest: $22, Shrine of Blood: 50% HP, etc)
                        }
                        else if (GPC) // Object is a pickup
                        {
                            string displayName = $"{GPC.GetDisplayName()}";
                            string description;

                            PickupIndex pickupIdx = GPC.pickupIndex;
                            ItemIndex itemIdx = pickupIdx.itemIndex;
                            ItemDef itemDef = ItemCatalog.GetItemDef(itemIdx);

                            if (itemDef.descriptionToken != null && UseLongDescription.Value) // Config is set to show long description and object has valid long description
                                description = itemDef.descriptionToken;
                            else
                                description = itemDef.pickupToken ?? itemDef.descriptionToken;

                            string message = $"<color={RoR2Colors.Tier1ItemDark}>{displayName}: {description}</color>";
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