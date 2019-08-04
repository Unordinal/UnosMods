using System;
using System.Reflection;
using BepInEx;
using R2API.Utils;
using UnityEngine;
using UnosUtilities;

namespace UnosMods.PriceOnPing
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.unordinal.priceonping", "Price On Ping", "1.0.0")]

    public class PriceOnPing : BaseUnityPlugin
    {
        public void Awake()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            On.RoR2.UI.PingIndicator.RebuildPing += (orig, self) =>
            {
                orig(self);
                try
                {
                    if (self.GetFieldValue<RoR2.UI.PingIndicator.PingType>("pingType") == RoR2.UI.PingIndicator.PingType.Interactable)
                    {
                        RoR2.PurchaseInteraction PI = self.pingTarget.GetComponent<RoR2.PurchaseInteraction>();
                        if (PI && PI.costType != RoR2.CostTypeIndex.None)
                        {
                            string interactable = $"{PI.GetDisplayName()}";
                            string cost = PI.GetTextFromPurchasableType();
                            string costColor = PI.GetColorFromPurchasableType();
                            string message = $"<color={RoR2Colors.Tier1ItemDark}>{interactable}:</style> <color={costColor}>{cost}</color>";
                            RoR2.Chat.AddMessage(message);
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
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("PriceOnPing.UnosUtilities.dll"))
            {
                byte[] assemblyData = new byte[stream.Length];
                stream.Read(assemblyData, 0, assemblyData.Length);
                return Assembly.Load(assemblyData);
            }
        }
    }
}