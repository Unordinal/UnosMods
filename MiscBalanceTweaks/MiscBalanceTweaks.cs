using BepInEx;
using BepInEx.Logging;
using MonoMod.Cil;
using static UnosMods.MiscBalanceTweaks.MiscBalanceTweaksConfig;

namespace UnosMods.MiscBalanceTweaks
{
    [BepInDependency(R2API.R2API.PluginGUID)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]

    public class MiscBalanceTweaks : BaseUnityPlugin
    {
        public const string PluginGUID = "com.unordinal.MiscBalanceTweaks";
        public const string PluginName = "MiscBalanceTweaks";
        public const string PluginVersion = "1.0.1";
        internal new static ManualLogSource Logger { get; } = new ManualLogSource(PluginName);

        public MiscBalanceTweaks()
        {
            Init(Config);
            if (MinSkillCooldown != 0.5f)
            {
                Logger.LogInfo("Creating hook.");
                IL.RoR2.GenericSkill.CalculateFinalRechargeInterval += GenericSkill_CalculateFinalRechargeInterval;
            }
            else
                Logger.LogWarning("MinSkillCooldown is 0.5, which is the vanilla value. Did you intend to change this?");
        }

        private void GenericSkill_CalculateFinalRechargeInterval(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(
                x => x.MatchLdcR4(0.5f),
                x => x.MatchLdarg(0)
            );
            c.Next.Operand = MinSkillCooldown;
        }
    }
}