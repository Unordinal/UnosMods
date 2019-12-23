using BepInEx;
using BepInEx.Configuration;

namespace UnosMods.BossTeleportPercent
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.unordinal.bossteleportpercent", "Boss Teleport Percent", "1.0.1")]

    public class BossTeleportPercent : BaseUnityPlugin
    {
        private static ConfigEntry<float> ChargePercent;
        //private static bool running_debug = false;
        
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
                if (RoR2.TeleporterInteraction.instance.isCharging)
                {
                    if (RoR2.TeleporterInteraction.instance.remainingChargeTimer - (ConstrainedPercent * 0.9f) <= 0f)
                        RoR2.TeleporterInteraction.instance.remainingChargeTimer = 0f;
                    else
                        RoR2.TeleporterInteraction.instance.remainingChargeTimer -= (ConstrainedPercent * 0.9f);
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
    }
}
