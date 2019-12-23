using System;
using UnityEngine;
using RoR2;
using BepInEx;
using BepInEx.Logging;
using R2API;

namespace UnosMods.RiskOfStrategy
{
    [BepInDependency(R2API.R2API.PluginGUID)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]

    public class RiskOfStrategy : BaseUnityPlugin
    {
        public const string PluginGUID = "com.unordinal.riskofstrategy";
        public const string PluginName = "Risk of Strategy";
        public const string PluginVersion = "1.0.0";
        internal new static ManualLogSource Logger { get; } = new ManualLogSource(PluginName);

        public RiskOfStrategy()
        {

        }

        public void Update()
        {

        }
    }
}