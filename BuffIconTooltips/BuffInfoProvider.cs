using RoR2;
using RoR2.UI;
using System.Collections.Generic;
using System.Linq;
using static Unordinal.BuffIconTooltips.Util;

namespace Unordinal.BuffIconTooltips
{
    public static class BuffInfoProvider
    {
        private static readonly Dictionary<BuffIndex, BuffInfo> buffIndexToInfo = new Dictionary<BuffIndex, BuffInfo>();

        internal static void Init()
        {
            buffIndexToInfo.Clear();
            RegisterBuffInfo(BuffIndex.AffixRed, new BuffInfo
            {
                Effect = $"Leave a {"fire trail".Stylize(Style.Damage)} and apply a {"percent burn".Stylize(Style.Damage)} on hit.",
                Sources = { GetNameFromIndex(EquipmentIndex.AffixRed), GetNameFromIndex(ItemIndex.HeadHunter) }
            });
            RegisterBuffInfo(BuffIndex.AffixBlue, new BuffInfo
            {
                Effect = $"Attacks {"explode".Stylize(Style.Damage)} after a delay. 50% of your max health is {"replaced by shield".Stylize(Style.Healing)}.",
                Sources = { GetNameFromIndex(EquipmentIndex.AffixBlue), GetNameFromIndex(ItemIndex.HeadHunter) }
            });
            RegisterBuffInfo(BuffIndex.AffixWhite, new BuffInfo
            {
                Effect = $"Leave an {"ice explosion".Stylize(Style.Utility)} on death. {"Slow".Stylize(Style.Utility)} enemies on hit for {"-80% movement speed".Stylize(Style.Utility)}.",
                Sources = { GetNameFromIndex(EquipmentIndex.AffixWhite), GetNameFromIndex(ItemIndex.HeadHunter) }
            });
            RegisterBuffInfo(BuffIndex.AffixPoison, new BuffInfo
            {
                Effect = $"{"Shoot occasional urchins".Stylize(Style.Utility)}. {"Disable healing".Stylize(Style.Damage)} for hit enemies.",
                Sources = { GetNameFromIndex(EquipmentIndex.AffixPoison), GetNameFromIndex(ItemIndex.HeadHunter) }
            });
            RegisterBuffInfo(BuffIndex.AffixHaunted, new BuffInfo
            {
                Effect = $"{"Cloak".Stylize(Style.Utility)} nearby allies. {"Slow".Stylize(Style.Utility)} enemies on hit for {"-80% movement speed".Stylize(Style.Utility)}.",
                Sources = { GetNameFromIndex(EquipmentIndex.AffixHaunted), GetNameFromIndex(ItemIndex.HeadHunter) }
            });
            RegisterBuffInfo(BuffIndex.AffixHauntedRecipient, new BuffInfo
            {
                Effect = $"{"Hidden".Stylize(Style.Utility)} by a Celestine Elite.",
                Sources = { GetNameFromIndex(EquipmentIndex.AffixHaunted), "Celestine Elites" }
            });
            RegisterBuffInfo(BuffIndex.ArmorBoost, new BuffInfo
            {
                Effect = $"Gain {"200 armor".Stylize(Style.Healing)}.",
                Sources = { "Transport Mode (MUL-T)", "Recover (Clay Dunestrider)", "Find Item (Scavenger)" }
            });
            RegisterBuffInfo(BuffIndex.AttackSpeedOnCrit, new BuffInfo
            {
                Effect = $"Increases {"attack speed".Stylize(Style.Damage)} by {"12%".Stylize(Style.Damage)} {"(+12% per stack)".Stylize(Style.Stack)}.",
                Sources = { GetNameFromIndex(ItemIndex.AttackSpeedOnCrit) }
            });
            RegisterBuffInfo(BuffIndex.BugWings, new BuffInfo
            {
                Effect = $"Has the ability to {"fly".Stylize(Style.Utility)}. Increases {"movement speed".Stylize(Style.Utility)} by {"20%".Stylize(Style.Utility)}.",
                Sources = { GetNameFromIndex(EquipmentIndex.Jetpack) }
            });
            RegisterBuffInfo(BuffIndex.Cloak, new BuffInfo
            {
                Effect = $"{"Invisible.".Stylize(Style.Utility)}.",
                Sources = { GetNameFromIndex(ItemIndex.Phasing) }
            });
            RegisterBuffInfo(BuffIndex.CloakSpeed, new BuffInfo
            {
                Effect = $"Increases {"movement speed".Stylize(Style.Utility)} by {"40%".Stylize(Style.Utility)}.",
                Sources = { GetNameFromIndex(ItemIndex.ShockNearby) }
            });
            RegisterBuffInfo(BuffIndex.ElephantArmorBoost, new BuffInfo
            {
                Effect = $"Gain {"500 armor".Stylize(Style.Healing)}.",
                Sources = { GetNameFromIndex(EquipmentIndex.GainArmor) }
            });
            RegisterBuffInfo(BuffIndex.Energized, new BuffInfo
            {
                Effect = $"Increases {"attack speed".Stylize(Style.Damage)} by {"70%".Stylize(Style.Damage)}.",
                Sources = { GetNameFromIndex(ItemIndex.EnergizedOnEquipmentUse) }
            });
            RegisterBuffInfo(BuffIndex.FullCrit, new BuffInfo
            {
                Effect = $"Increases {"critical strike chance".Stylize(Style.Damage)} by {"100%".Stylize(Style.Damage)}.",
                Sources = { GetNameFromIndex(EquipmentIndex.CritOnUse) }
            });
            RegisterBuffInfo(BuffIndex.HiddenInvincibility, new BuffInfo
            {
                Effect = $"{"Invulnerable".Stylize(Style.Utility)}.",
                Sources = { "Eviscerate (Mercenary)", "Blinding Assault (Mercenary)", "Spawn Protection (3s)" }
            });
            RegisterBuffInfo(BuffIndex.Immune, new BuffInfo
            {
                Effect = $"{"Invulnerable".Stylize(Style.Utility)}.",
                Sources = { GetNameFromIndex(ItemIndex.ExtraLife) + " (3s)", "Spawn Protection (3s)" }
            });
            RegisterBuffInfo(BuffIndex.NullSafeZone, new BuffInfo
            {
                Effect = $"{"Safe".Stylize(Style.Utility)} from damage in the {"Void Fields".Stylize(Style.Artifact)}.",
                Sources = { "Void Fields' Cell Domes" }
            });
            RegisterBuffInfo(BuffIndex.CrocoRegen, new BuffInfo
            {
                Effect = $"{"Boosted regeneration".Stylize(Style.Healing)}.",
                Sources = { GetNameFromIndex(ItemIndex.RegenOnKill) }
            });
            RegisterBuffInfo(BuffIndex.MeatRegenBoost, new BuffInfo
            {
                Effect = $"{"Regenerate".Stylize(Style.Healing)} for {"10%".Stylize(Style.Healing)} of your maximum health over {"0.5s".Stylize(Style.Utility)}.",
                Sources = { "Vicious Wounds (Acrid)", "Ravenous Bite (Acrid)" }
            });
            RegisterBuffInfo(BuffIndex.MedkitHeal, new BuffInfo
            {
                Effect = $"{"Heal".Stylize(Style.Healing)} after not taking damage for {"2s".Stylize(Style.Utility)}.",
                Sources = { GetNameFromIndex(ItemIndex.Medkit) }
            });
            RegisterBuffInfo(BuffIndex.NoCooldowns, new BuffInfo
            {
                Effect = $"{"Skills have 0.5s cooldowns".Stylize(Style.Utility)}.",
                Sources = { GetNameFromIndex(ItemIndex.KillEliteFrenzy) }
            });
            RegisterBuffInfo(BuffIndex.TeslaField, new BuffInfo
            {
                Effect = $"{"Lighting shoots out".Stylize(Style.Damage)} from you, hitting nearby enemies.",
                Sources = { GetNameFromIndex(ItemIndex.ShockNearby) }
            });
            RegisterBuffInfo(BuffIndex.TonicBuff, new BuffInfo
            {
                Effect = $"{"Massive boost".Stylize(Style.Utility)} to all stats.",
                Sources = { GetNameFromIndex(EquipmentIndex.Tonic) }
            });
            RegisterBuffInfo(BuffIndex.Warbanner, new BuffInfo
            {
                Effect = $"Increased {"attack".Stylize(Style.Damage)} and {"movement speed".Stylize(Style.Utility)}.",
                Sources = { GetNameFromIndex(ItemIndex.WardOnLevel) }
            });
            RegisterBuffInfo(BuffIndex.WarCryBuff, new BuffInfo
            {
                Effect = $"Increased {"attack".Stylize(Style.Damage)} and {"movement speed".Stylize(Style.Utility)}.",
                Sources = { GetNameFromIndex(ItemIndex.WarCryOnMultiKill) }
            });
            RegisterBuffInfo(BuffIndex.TeamWarCry, new BuffInfo
            {
                Effect = $"Increased {"attack".Stylize(Style.Damage)} and {"movement speed".Stylize(Style.Utility)} for the entire team.",
                Sources = { GetNameFromIndex(EquipmentIndex.TeamWarCry) }
            });
            RegisterBuffInfo(BuffIndex.WhipBoost, new BuffInfo
            {
                Effect = $"Increased {"movement speed".Stylize(Style.Utility)} by {"30%".Stylize(Style.Utility)} while not in combat.",
                Sources = { GetNameFromIndex(ItemIndex.SprintOutOfCombat) }
            });
            RegisterBuffInfo(BuffIndex.LifeSteal, new BuffInfo
            {
                Effect = $"{"Heals".Stylize(Style.Healing)} for {"20%".Stylize(Style.Healing)} of damage dealt.",
                Sources = { GetNameFromIndex(EquipmentIndex.LifestealOnHit) }
            });
            RegisterBuffInfo(BuffIndex.PowerBuff, new BuffInfo
            {
                Effect = $"Increased {"damage".Stylize(Style.Damage)} by {"50%".Stylize(Style.Damage)}.",
                Sources = { GetNameFromIndex(ItemIndex.RandomDamageZone) }
            });
            RegisterBuffInfo(BuffIndex.ElementalRingsReady, new BuffInfo
            {
                Effect = $"Runald's Band and/or Kjaro's Band {"can be activated".Stylize(Style.Damage)}.",
                Sources = { GetNameFromIndex(ItemIndex.IceRing), GetNameFromIndex(ItemIndex.FireRing) }
            });
            RegisterBuffInfo(BuffIndex.ElementalRingsCooldown, new BuffInfo
            {
                Effect = $"Runald's Band and/or Kjaro's Band are {"on cooldown".Stylize(Style.Death)}.",
                Sources = { GetNameFromIndex(ItemIndex.IceRing), GetNameFromIndex(ItemIndex.FireRing) }
            });
            RegisterBuffInfo(BuffIndex.LunarShell, new BuffInfo
            {
                Effect = $"If an instance of damage would deal more than {"10%".Stylize(Style.Health)} of your max HP (including shield), it instead deals {"10%".Stylize(Style.Utility)} of your max HP.",
                Sources = { "Lunar Chimera (Golem)" }
            });


            RegisterBuffInfo(BuffIndex.DeathMark, new BuffInfo
            {
                Effect = $"Increases {"damage taken".Stylize(Style.Damage)} by {"50%".Stylize(Style.Damage)} from all sources for {"7s".Stylize(Style.Utility)} {"(+7s per stack)".Stylize(Style.Stack)}.",
                Sources = { GetNameFromIndex(ItemIndex.DeathMark) }
            });
            RegisterBuffInfo(BuffIndex.OnFire, new BuffInfo
            {
                Effect = $"{"Burning".Stylize(Style.Damage)}. {"Health regen".Stylize(Style.Health)} set to {"0".Stylize(Style.Health)}.",
                Sources = { "Blazing Elite", "Magma Worm", "Fire Breath (Elder Lemurian)", "Flame Bolt (Artificer)", "Flamethrower (Artificer)", GetNameFromIndex(ItemIndex.IgniteOnKill) }
            });
            RegisterBuffInfo(BuffIndex.Bleeding, new BuffInfo
            {
                Effect = $"{"Bleeding".DamageColorize(DamageColorIndex.Bleed)}.",
                Sources = { "Slash (Imp)", "Spike Volley (Imp Overlord)", GetNameFromIndex(ItemIndex.BleedOnHit) }
            });
            RegisterBuffInfo(BuffIndex.Poisoned, new BuffInfo
            {
                Effect = $"{"Poisoned".DamageColorize(DamageColorIndex.Poison)}.",
                Sources = { "Poison (Acrid)" }
            });
            RegisterBuffInfo(BuffIndex.Blight, new BuffInfo
            {
                Effect = $"Deal {"60%".Stylize(Style.Damage)} damage per second.",
                Sources = { "Blight (Acrid)" }
            });
            RegisterBuffInfo(BuffIndex.Weak, new BuffInfo
            {
                Effect = $"Armor is {"reduced by 20".Stylize(Style.Health)}. Damage and movement speed is {"reduced by 40%".Stylize(Style.Health)}.",
                Sources = { "DIRECTIVE: Inject (REX)", "DIRECTIVE: Disperse (REX)" }
            });
            RegisterBuffInfo(BuffIndex.Entangle, new BuffInfo
            {
                Effect = $"You are {"rooted in place".Stylize(Style.Health)} and {"unable to move".Stylize(Style.Health)}.",
                Sources = { "Tangling Growth (REX)" }
            });
            RegisterBuffInfo(BuffIndex.PulverizeBuildup, new BuffInfo
            {
                Effect = $"Your armor is {"cracked".Stylize(Style.Damage)}. Receiving five stacks of this will {"Pulverize".Stylize(Style.Utility)} your armor, {"reducing".Stylize(Style.Damage)} it by {"60".Stylize(Style.Damage)}.",
                Sources = { GetNameFromIndex(ItemIndex.ArmorReductionOnHit) }
            });
            RegisterBuffInfo(BuffIndex.Pulverized, new BuffInfo
            {
                Effect = $"{"Pulverized".Stylize(Style.Utility)}. Armor {"reduced".Stylize(Style.Health)} by {"60".Stylize(Style.Health)}.",
                Sources = { GetNameFromIndex(ItemIndex.ArmorReductionOnHit) }
            });
            RegisterBuffInfo(BuffIndex.MercExpose, new BuffInfo
            {
                Effect = $"Takes an additional {"350% damage".Stylize(Style.Damage)}.",
                Sources = { "Laser Sword (Mercenary)", "Slicing Winds (Mercenary)" }
            });
            RegisterBuffInfo(BuffIndex.ClayGoo, new BuffInfo
            {
                Effect = $"Movement speed {"reduced".Stylize(Style.Health)} by {"50%".Stylize(Style.Health)}.",
                Sources = { "Exploding Pots (Environment, Clay Dunestrider)", "Goo (Abandoned Aqueduct)", "Tar Blast (Clay Templar)" }
            });
            RegisterBuffInfo(BuffIndex.Slow50, new BuffInfo
            {
                Effect = $"Movement speed {"reduced".Stylize(Style.Health)} by {"50%".Stylize(Style.Health)}.",
                Sources = { "Arrow Rain (Huntress)", "TR58 Carbonizer Turret (Engineer)", "Solus Control Unit", "Alloy Worship Unit", "Clay Templar (self while shooting)", "Mini Mushrum" }
            });
            RegisterBuffInfo(BuffIndex.Slow60, new BuffInfo
            {
                Effect = $"Movement speed {"reduced".Stylize(Style.Health)} by {"60%".Stylize(Style.Health)}.",
                Sources = { GetNameFromIndex(ItemIndex.SlowOnHit) }
            });
            RegisterBuffInfo(BuffIndex.Slow80, new BuffInfo
            {
                Effect = $"Movement speed {"reduced".Stylize(Style.Health)} by {"80%".Stylize(Style.Health)}.",
                Sources = { "Glacial Elite", "Celestine Elite", GetNameFromIndex(ItemIndex.IceRing) }
            });
            RegisterBuffInfo(BuffIndex.Cripple, new BuffInfo
            {
                Effect = $"Movement speed {"reduced".Stylize(Style.Health)} by {"50%".Stylize(Style.Health)}. Armor {"reduced".Stylize(Style.Health)} by {"20".Stylize(Style.Health)}.",
                Sources = { GetNameFromIndex(EquipmentIndex.CrippleWard) }
            });
            RegisterBuffInfo(BuffIndex.BeetleJuice, new BuffInfo
            {
                Effect = $"Movement speed {"reduced".Stylize(Style.Health)} by {"5%".Stylize(Style.Health)}. Damage {"reduced".Stylize(Style.Health)} by {"5%".Stylize(Style.Health)}. Attack speed {"reduced".Stylize(Style.Health)} by {"5%".Stylize(Style.Health)}.",
                Sources = { "Beetle Queen" }
            });
            RegisterBuffInfo(BuffIndex.HealingDisabled, new BuffInfo
            {
                Effect = $"{"Healing disabled".Stylize(Style.Health)}.",
                Sources = { "Malachite Elite" }
            });
            RegisterBuffInfo(BuffIndex.NullifyStack, new BuffInfo
            {
                Effect = $"Nullify stack. Upon getting three stacks, apply {"Nullified".Stylize(Style.Utility)}.",
                Sources = { "Void Reaver" }
            });
            RegisterBuffInfo(BuffIndex.Nullified, new BuffInfo
            {
                Effect = $"{"Nullified".Stylize(Style.Utility)}. Movement speed {"reduced".Stylize(Style.Health)} to {"0".Stylize(Style.Health)}.",
                Sources = { "Void Reaver" }
            });
            RegisterBuffInfo(BuffIndex.PermanentCurse, new BuffInfo
            {
                Effect = $"Upon taking damage, gain stacks of {"Permanent Curse".Stylize(Style.Utility)}. {"Stacks determine percentage of your max health lost to curse".Stylize(Style.Death)}. Reset to 0 at end of stage.",
                Sources = { "Eclipse 8" }
            });

            for (BuffIndex i = 0; i < BuffIndex.Count; i++)
            {
                if (!buffIndexToInfo.ContainsKey(i))
                {
                    BuffIconTooltips.Logger.LogDebug($"Unregistered BuffInfo for {i}!");
                }
            }
        }

        private static void RegisterBuffInfo(BuffIndex buffIndex, BuffInfo buffInfo)
        {
            if (buffIndexToInfo.ContainsKey(buffIndex))
            {
                BuffIconTooltips.Logger.LogWarning($"Attempted to register duplicate buff info for {buffIndex}!");
                return;
            }

            if (buffIndex < BuffIndex.Count) buffInfo.Name = buffIndex.ToString();
            buffInfo.BuffIndex = buffIndex;

            string desc = buffInfo.Effect;
            if (!string.IsNullOrWhiteSpace(desc)) desc += "\n\n";
            if (buffInfo.Sources.Any()) desc += $"{"Source(s):".Stylize(Style.Artifact)} {string.Join(", ", buffInfo.Sources)}";
            buffInfo.Description = desc;

            BuffDef buffDef = BuffCatalog.GetBuffDef(buffIndex);
            if (buffDef != null)
            {
                buffInfo.Color = buffDef.buffColor;
                buffInfo.IsDebuff = buffDef.isDebuff;
            }

            buffIndexToInfo[buffIndex] = buffInfo;
        }

        public static BuffInfo GetBuffInfoFromIndex(BuffIndex buffIndex)
        {
            buffIndexToInfo.TryGetValue(buffIndex, out BuffInfo buffInfo);
            return buffInfo;
        }

        public static string GetNameFromIndex(ItemIndex itemIndex)
        {
            var itemDef = ItemCatalog.GetItemDef(itemIndex);
            return Language.GetString(itemDef.nameToken);
        }
        
        public static string GetNameFromIndex(EquipmentIndex equipIndex)
        {
            var equipDef = EquipmentCatalog.GetEquipmentDef(equipIndex);
            return Language.GetString(equipDef.nameToken);
        }
    }
}
