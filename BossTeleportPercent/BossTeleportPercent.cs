using System;
using System.Collections;
using BepInEx;
using BepInEx.Configuration;
using R2API.Utils;
using UnityEngine;

namespace UnosMods.BossTeleportPercent
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.unordinal.bossteleportpercent", "Boss Teleport Percent", "1.0.0")]

    public class BossTeleportPercent : BaseUnityPlugin
    {
        private static ConfigWrapper<float> BossKillPercent;
        //private static bool running_debug = false;
        
        public void Awake()
        {
            BossKillPercent = Config.Wrap(
                "BossKillPercent",
                "BossKillPercent",
                $"Set the teleporter percentage gained by killing the teleporter boss (0-100, default is 25)",
                25f);
            On.RoR2.BossGroup.OnDefeatedServer += (orig, self) =>
            {
                if (RoR2.TeleporterInteraction.instance.isCharging)
                {
                    if (RoR2.TeleporterInteraction.instance.remainingChargeTimer - (constrainedPercent * 0.9f) <= 0f)
                        RoR2.TeleporterInteraction.instance.remainingChargeTimer = 0f;
                    else
                        RoR2.TeleporterInteraction.instance.remainingChargeTimer -= (constrainedPercent * 0.9f);
                }
            };
        }

        /*public void Update()
        {
            if (RoR2.Stage.instance != null)
            {
                if (!running_debug && RoR2.TeleporterInteraction.instance.isCharging)
                {
                    StartCoroutine(TeleportChargePercent_Debug());
                    running_debug = true;
                }
                else if (running_debug && !RoR2.TeleporterInteraction.instance.isCharging)
                {
                    StopCoroutine(TeleportChargePercent_Debug());
                    running_debug = false;
                }
            }
        }*/

        /*IEnumerator TeleportChargePercent_Debug()
        {
            while (RoR2.TeleporterInteraction.instance.isCharging)
            {
                Debug.Log(RoR2.TeleporterInteraction.instance.remainingChargeTimer);
                yield return new WaitForSeconds(1f);
            }
        }*/

        private float constrainedPercent
        {
            get
            {
                try
                {
                    if (BossKillPercent.Value < 0f)
                        return 0f;
                    if (BossKillPercent.Value > 100f)
                        return 100f;
                    return BossKillPercent.Value;
                }
                catch
                {
                    return 25f;
                }
            }
        }
    }
}
