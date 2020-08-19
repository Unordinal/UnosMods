using BepInEx;
using BepInEx.Configuration;
using RoR2;
using UnityEngine.Networking;

namespace Unordinal.StartingBannerBuff
{
    [BepInDependency(R2API.R2API.PluginGUID)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class StartingBannerBuff : BaseUnityPlugin
    {
        public const string PluginName = "Starting Banner Buff";
        public const string PluginGUID = "Unordinal.StartingBannerBuff";
        public const string PluginVersion = "1.0.3";

        private static ConfigEntry<int> keepBuffUntilStage;
        private bool ShouldBeBuffed
        {
            get
            {
                if (!Run.instance)
                    return false;
                return Run.instance.stageClearCount + 1 < KeepBuffUntilStage;
            }
        }

        internal void Awake()
        {
            InitConfig();
            On.RoR2.Stage.RespawnCharacter += Stage_RespawnCharacter;
        }

        private void Stage_RespawnCharacter(On.RoR2.Stage.orig_RespawnCharacter orig, Stage self, CharacterMaster characterMaster)
        {
            orig(self, characterMaster);
            if (NetworkServer.active)
            {
                if (Run.instance && Stage.instance && Run.instance.stageClearCount + 1 <= KeepBuffUntilStage) // Toggle buffs until equal KeepBuff var
                {
                    if (characterMaster?.GetComponent<PlayerCharacterMasterController>())
                    {
                        Logger.LogInfo("Player spawned, checking for buff toggle");
                        ToggleBuff();
                    }
                }
            }
        }

        internal void ToggleBuff()
        {
            if (NetworkServer.active && Run.instance && Stage.instance)
            {
                var players = NetworkUser.readOnlyInstancesList;
                if (ShouldBeBuffed)
                {
                    Logger.LogInfo("Toggling buff on");
                    foreach (var player in players)
                        if (!player.GetCurrentBody()?.HasBuff(BuffIndex.Warbanner) ?? false)
                            player.GetCurrentBody().AddBuff(BuffIndex.Warbanner);
                }
                else if (!ShouldBeBuffed)
                {
                    Logger.LogInfo("Toggling buff off");
                    foreach (var player in players)
                        if (player.GetCurrentBody()?.HasBuff(BuffIndex.Warbanner) ?? false)
                            player.GetCurrentBody().RemoveBuff(BuffIndex.Warbanner);
                }
            }
        }

        private void InitConfig()
        {
            keepBuffUntilStage = Config.Bind(nameof(StartingBannerBuff), "KeepBuffUntilStage", 3, "Keeps the buff until the given stage. (2-20, Default: 3)");
        }

        /*private static int SpeedBuffPercent
        {
            get
            {
                try
                {
                    if (speedBuffPercent.Value < 1)
                        return 1;
                    else if (speedBuffPercent.Value > 100)
                        return 100;
                    return speedBuffPercent.Value;
                }
                catch
                {
                    return 25;
                }
            }
            set => speedBuffPercent.Value = value;
        }*/

        private static int KeepBuffUntilStage
        {
            get
            {
                try
                {
                    if (keepBuffUntilStage.Value < 2)
                        return 2;
                    else if (keepBuffUntilStage.Value > 20)
                        return 20;
                    return keepBuffUntilStage.Value;
                }
                catch
                {
                    return 3;
                }
            }
            set => keepBuffUntilStage.Value = value;
        }
    }
}
