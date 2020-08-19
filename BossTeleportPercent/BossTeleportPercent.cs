using BepInEx;
using BepInEx.Configuration;
using RoR2;

namespace Unordinal.BossTeleportPercent
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class BossTeleportPercent : BaseUnityPlugin
    {
        public const string PluginGUID = "Unordinal.BossTeleportPercent";
        public const string PluginName = "Boss Teleport Percent";
        public const string PluginVersion = "1.0.4";

        private static ConfigEntry<float> ChargePercent;

        internal void Awake()
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
                    Logger.LogDebug("Boss defeated, detected teleporter charging event.");
                    float chargeBy = ConstrainedPercent;
                    if (TeleporterInteraction.instance.holdoutZoneController.Network_charge + chargeBy >= 1f)
                    {
                        float adjustedChargeBy = 1f - TeleporterInteraction.instance.holdoutZoneController.Network_charge;
                        Logger.LogDebug($"Fully charged holdout zone. (charged by '{adjustedChargeBy}')");
                        TeleporterInteraction.instance.holdoutZoneController.FullyChargeHoldoutZone();
                    }
                    else
                    {
                        Logger.LogDebug($"Charged holdout zone by '{chargeBy}'.");
                        TeleporterInteraction.instance.holdoutZoneController.Network_charge += chargeBy;
                    }
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
                        return 1f;
                    return ChargePercent.Value / 100;
                }
                catch
                {
                    return 0.25f;
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
