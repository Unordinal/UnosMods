using BepInEx;
using BepInEx.Configuration;
using RoR2;
using System.Collections;
using UnityEngine;

namespace UnosMods.BossTeleportPercent
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.unordinal.bossteleportpercent", "Boss Teleport Percent", "1.0.2")]

    public class BossTeleportPercent : BaseUnityPlugin
    {
        private static ConfigEntry<float> ChargePercent;

        public void Awake()
        {
            ChargePercent = Config.Bind(
                "ChargePercent",
                "ChargePercent",
                25f,
                $"Set the teleporter percentage gained by killing the teleporter boss (0-100, default is 25)");

            On.RoR2.BossGroup.OnDefeatedServer += (orig, self) =>
            {
                orig(self);
                if (TeleporterInteraction.instance && TeleporterInteraction.instance.isCharging)
                {
                    if (TeleporterInteraction.instance.remainingChargeTimer - (ConstrainedPercent * 0.9f) <= 0f)
                        TeleporterInteraction.instance.remainingChargeTimer = 0f;
                    else
                        TeleporterInteraction.instance.remainingChargeTimer -= (ConstrainedPercent * 0.9f);
                }
            };
        }

        private float ConstrainedPercent
        {
            get
            {
                try
                {
                    if (ChargePercent.Value < 0f)
                        return 0f;
                    if (ChargePercent.Value > 100f)
                        return 100f;
                    return ChargePercent.Value;
                }
                catch
                {
                    return 25f;
                }
            }
        }

        /*public void FixedUpdate()
        {
            if (Stage.instance && TeleporterInteraction.instance)
            {
                if (!running_debug && TeleporterInteraction.instance.isCharging)
                {
                    StartCoroutine(TeleportChargePercent_Debug());
                    running_debug = true;
                }
                else if (running_debug && !TeleporterInteraction.instance.isCharging)
                {
                    StopCoroutine(TeleportChargePercent_Debug());
                    running_debug = false;
                }
            }
        }

        IEnumerator TeleportChargePercent_Debug()
        {
            while (TeleporterInteraction.instance && TeleporterInteraction.instance.isCharging)
            {
                Debug.Log(TeleporterInteraction.instance.remainingChargeTimer);
                yield return new WaitForSeconds(1f);
            }
        }*/
    }
}
