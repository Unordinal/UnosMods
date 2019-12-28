using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using UnityEngine;
using Unordinal;
using Unordinal.InventoryStats.Formatters;
using Unordinal.InventoryStats.Providers;
using Unordinal.InventoryStats.Stats.Modifiers;
using static Unordinal.StyleCatalog;

namespace Unordinal.InventoryStats.Stats
{
    public static class PickupStatsDefinitions
    {
        public static string ColorPositive { get; } = StyleIndex.cIsHealing.ToHex();
        public static string ColorNegative { get; } = StyleIndex.cIsHealth.ToHex();
        public static string ColorNeutral { get; } = StyleIndex.cIsDamage.ToHex();
        public static string ColorEquipmentCooldown { get; } = ColorCatalog.ColorIndex.Equipment.ToHex();
        public static string ColorNote { get; } = StyleIndex.cIsUtility.ToHex();

        public static IReadOnlyDictionary<ItemIndex, List<Stat>> ItemStatDefs { get; private set; }
        public static IReadOnlyDictionary<EquipmentIndex, List<Stat>> EquipmentStatDefs { get; private set; }

        public static void UpdateItemStatDefs()
        {
            ItemStatDefs = new Dictionary<ItemIndex, List<Stat>>
            {
                #region Common
                [ItemIndex.Syringe]                 = new List<Stat>
                {
                    new Stat(
                        text: "Attack Speed",
                        formula: count => StackFormulas.Linear(count, 0.15f),
                        formatter: new PercentageStatFormatter()
                        ),
                },
                [ItemIndex.Bear]                    = new List<Stat>
                {
                    new Stat(
                        text: "Block Chance",
                        formula: count => StackFormulas.Hyperbolic(count, 0.15f),
                        formatter: new PercentageStatFormatter(effectiveMax: 97.5f) // TODO: probably wrong
                        ),
                },
                [ItemIndex.Tooth]                   = new List<Stat>
                {
                    new Stat(
                        text: "Health per Orb",
                        formula: count => StackFormulas.Linear(count, 8.0f),
                        formatter: new NumStatFormatter()
                        )
                },
                [ItemIndex.CritGlasses]             = new List<Stat>
                {
                    new Stat(
                        text: "Crit. Chance",
                        formula: count => StackFormulas.Linear(count, 0.10f),
                        formatter: new PercentageStatFormatter(effectiveMax: 1.0f)
                        )
                },
                [ItemIndex.Hoof]                    = new List<Stat>
                {
                    new Stat(
                        text: "Move Speed",
                        formula: count => StackFormulas.Linear(count, 0.14f),
                        formatter: new PercentageStatFormatter()
                        )
                },
                [ItemIndex.Mushroom]                = new List<Stat>
                {
                    new Stat(
                        text: "Health per Second",
                        formula: count => StackFormulas.Linear(count, 0.0225f, 0.045f),
                        formatter: new PercentageStatFormatter()
                        ),
                    new Stat(
                        text: "Radius",
                        formula: count => StackFormulas.Linear(count, 1.5f, 3.0f),
                        formatter: new DistanceStatFormatter()
                        ),
                    new Stat(
                        text: "Unmoving Duration",
                        formula: count => 2f,
                        formatter: new TimeStatFormatter(),
                        stacks: false
                        ),
                },
                [ItemIndex.Crowbar]                 = new List<Stat>
                {
                    new Stat(
                        text: "Damage",
                        formula: count => StackFormulas.Linear(count, 0.5f, 1.5f),
                        formatter: new PercentageStatFormatter()
                        ),
                    new Stat(
                        text: "Health Threshold",
                        formula: count => 0.90f,
                        formatter: new PercentageStatFormatter(),
                        stacks: false
                        ),
                },
                [ItemIndex.BleedOnHit]              = new List<Stat>
                {
                    new Stat(
                        text: "Proc. Chance",
                        formula: count => StackFormulas.Linear(count, 0.15f),
                        formatter: new PercentageStatFormatter(),
                        modifiers: LuckModifier.Instance
                        ),
                    new Stat(
                        text: "Bleed Damage",
                        formula: count => 2.4f,
                        formatter: new PercentageStatFormatter(),
                        stacks: false
                        ),
                },
                [ItemIndex.WardOnLevel]             = new List<Stat>
                {
                    new Stat(
                        text: "Radius",
                        formula: count => StackFormulas.Linear(count, 8.0f, 16.0f),
                        formatter: new DistanceStatFormatter()
                        ),
                    new Stat(
                        text: "Attack Speed",
                        formula: count => 0.3f,
                        formatter: new PercentageStatFormatter(),
                        stacks: false
                        ),
                    new Stat(
                        text: "Move Speed",
                        formula: count => 0.3f,
                        formatter: new PercentageStatFormatter(),
                        stacks: false
                        ),
                },
                [ItemIndex.HealWhileSafe]           = new List<Stat>
                {
                    new Stat(
                        text: "Health Regen",
                        formula: count => StackFormulas.Linear(count, 3.0f),
                        formatter: new NumStatFormatter(suffix: " hp/s")
                        )
                },
                [ItemIndex.PersonalShield]          = new List<Stat>
                {
                    new Stat(
                        text: "Shield",
                        formula: count => StackFormulas.Linear(count, 0.08f),
                        formatter: new PercentageStatFormatter()
                        )
                },
                [ItemIndex.Medkit]                  = new List<Stat>
                {
                    new Stat(
                        text: "Heal",
                        formula: count => StackFormulas.Linear(count, 10f),
                        formatter: new NumStatFormatter()
                        ),
                    new Stat(
                        text: "Heal Delay",
                        formula: count => 1.1f,
                        formatter: new TimeStatFormatter(),
                        stacks: false
                        )
                },
                [ItemIndex.IgniteOnKill]            = new List<Stat>
                {
                    new Stat(
                        text: "Damage",
                        formula: count => StackFormulas.Linear(count, 0.75f, 1.5f),
                        formatter: new PercentageStatFormatter()
                        ),
                    new Stat(
                        text: "Radius",
                        formula: count => StackFormulas.Linear(count, 4.0f, 12.0f),
                        formatter: new DistanceStatFormatter()
                        ),
                },
                [ItemIndex.StunChanceOnHit]         = new List<Stat>
                {
                    new Stat(
                        text: "Proc. Chance",
                        formula: count => StackFormulas.Hyperbolic(count, 0.05f),
                        formatter: new PercentageStatFormatter(),
                        modifiers: LuckModifier.Instance
                        ),
                    new Stat(
                        text: "Duration",
                        formula: count => 2f,
                        formatter: new TimeStatFormatter(),
                        stacks: false
                        )
                },
                [ItemIndex.Firework]                = new List<Stat>
                {
                    new Stat(
                        text: "Fireworks",
                        formula: count => StackFormulas.Linear(count, 4f, 8f),
                        formatter: new NumStatFormatter()
                        ),
                    new Stat(
                        text: "Damage",
                        formula: count => 3.0f,
                        formatter: new PercentageStatFormatter(),
                        stacks: false
                        )
                },
                [ItemIndex.SprintBonus]             = new List<Stat>
                {
                    new Stat(
                        text: "Sprint Speed",
                        formula: count => StackFormulas.Linear(count, 0.2f, 0.3f),
                        formatter: new PercentageStatFormatter()
                        )
                },
                [ItemIndex.SecondarySkillMagazine]  = new List<Stat>
                {
                    new Stat(
                        text: "Secondary Skill Charges",
                        formula: count => StackFormulas.Linear(count, 1f),
                        formatter: new NumStatFormatter()
                        )
                },
                [ItemIndex.StickyBomb]              = new List<Stat>
                {
                    new Stat(
                        text: "Proc. Chance",
                        formula: count => StackFormulas.Linear(count, 0.05f),
                        formatter: new PercentageStatFormatter(),
                        modifiers: LuckModifier.Instance
                        ),
                    new Stat(
                        text: "Damage",
                        formula: count => 1.8f,
                        formatter: new PercentageStatFormatter(),
                        stacks: false
                        )
                },
                [ItemIndex.TreasureCache]           = new List<Stat>
                {
                    new Stat(
                        text: "Common Item Rarity",
                        formula: _ => TreasureCacheModifier.CommonRarity,
                        formatter: new PercentageStatFormatter(color: ColorCatalog.ColorIndex.Tier1Item.ToHex()),
                        modifiers: TreasureCacheModifier.Instance
                        ),
                    new Stat(
                        text: "Uncommon Item Rarity",
                        formula: _ => TreasureCacheModifier.UncommonRarity,
                        formatter: new PercentageStatFormatter(color: ColorCatalog.ColorIndex.Tier2Item.ToHex()),
                        modifiers: TreasureCacheModifier.Instance
                        ),
                    new Stat(
                        text: "Legendary Item Rarity",
                        formula: _ => TreasureCacheModifier.LegendaryRarity,
                        formatter: new PercentageStatFormatter(color: ColorCatalog.ColorIndex.Tier3Item.ToHex()),
                        modifiers: TreasureCacheModifier.Instance
                        ),
                },
                [ItemIndex.BossDamageBonus]         = new List<Stat>
                {
                    new Stat(
                        text: "Boss Damage",
                        formula: count => StackFormulas.Linear(count, 0.2f),
                        formatter: new PercentageStatFormatter()
                        ),
                },
                [ItemIndex.BarrierOnKill]           = new List<Stat>
                {
                    new Stat(
                        text: "Barrier Gain",
                        formula: count => StackFormulas.Linear(count, 15f),
                        formatter: new NumStatFormatter()
                        )
                },
                [ItemIndex.NearbyDamageBonus]       = new List<Stat>
                {
                    new Stat(
                        text: "Damage",
                        formula: count => StackFormulas.Linear(count, 0.15f),
                        formatter: new PercentageStatFormatter()
                        ),
                    new Stat(
                        text: "Range",
                        formula: count => 13.0f,
                        formatter: new DistanceStatFormatter(),
                        stacks: false
                        )
                },
                [ItemIndex.RegenOnKill]             = new List<Stat>
                {
                    new Stat(
                        text: "Duration",
                        formula: count => StackFormulas.Linear(count, 3f),
                        formatter: new TimeStatFormatter()
                        ),
                    new Stat(
                        text: "Health Regen",
                        formula: count => 2f,
                        formatter: new NumStatFormatter(suffix: " hp/s"),
                        stacks: false
                        )
                },
                #endregion

                #region Uncommon
                [ItemIndex.Missile]                 = new List<Stat>
                {
                    new Stat(
                        text: "Damage",
                        formula: count => StackFormulas.Linear(count, 3f),
                        formatter: new DistanceStatFormatter()
                        ),
                    new Stat(
                        text: "Proc. Chance",
                        formula: count => 0.10f,
                        formatter: new PercentageStatFormatter(),
                        stacks: false,
                        modifiers: LuckModifier.Instance
                        ),
                },
                [ItemIndex.ExplodeOnDeath]          = new List<Stat>
                {
                    new Stat(
                        text: "Radius",
                        formula: count => StackFormulas.Linear(count, 2.4f, 12.0f),
                        formatter: new DistanceStatFormatter()
                        ),
                    new Stat(
                        text: "Damage",
                        formula: count => StackFormulas.Linear(count, 2.8f, 3.5f),
                        formatter: new PercentageStatFormatter()
                        ),
                },
                [ItemIndex.Feather]                 = new List<Stat>
                {
                    new Stat(
                        text: "Extra Jumps",
                        formula: count => StackFormulas.Linear(count, 1f),
                        formatter: new NumStatFormatter()
                        )
                },
                [ItemIndex.ChainLightning]          = new List<Stat>
                {
                    new Stat(
                        text: "Targets",
                        formula: count => StackFormulas.Linear(count, 2f, 3f),
                        formatter: new NumStatFormatter()
                        ),
                    new Stat(
                        text: "Radius",
                        formula: count => StackFormulas.Linear(count, 2.0f, 20.0f),
                        formatter: new DistanceStatFormatter()
                        ),
                    new Stat(
                        text: "Proc. Chance",
                        formula: count => 0.25f,
                        formatter: new PercentageStatFormatter(),
                        stacks: false,
                        modifiers: LuckModifier.Instance
                        ),
                    new Stat(
                        text: "Damage",
                        formula: count => 0.80f,
                        formatter: new PercentageStatFormatter(),
                        stacks: false
                        ),
                },
                [ItemIndex.Seed]                    = new List<Stat>
                {
                    new Stat(
                        text: "Heal",
                        formula: count => StackFormulas.Linear(count, 1f),
                        formatter: new NumStatFormatter()
                        )
                },
                [ItemIndex.AttackSpeedOnCrit]       = new List<Stat>
                {
                    new Stat(
                        text: "Attack Speed Cap",
                        formula: count => StackFormulas.Linear(count, 0.24f, 0.36f),
                        formatter: new PercentageStatFormatter()
                        ),
                    new Stat(
                        text: "Attack Speed",
                        formula: count => 0.12f,
                        formatter: new PercentageStatFormatter(),
                        stacks: false
                        ),
                    new Stat(
                        text: "Crit. Chance",
                        formula: count => 0.05f,
                        formatter: new PercentageStatFormatter(),
                        stacks: false
                        ),
                },
                [ItemIndex.SprintOutOfCombat]       = new List<Stat>
                {
                    new Stat(
                        text: "Move Speed",
                        formula: count => StackFormulas.Linear(count, 0.30f),
                        formatter: new PercentageStatFormatter()
                        )
                },
                [ItemIndex.Phasing]                 = new List<Stat>
                {
                    new Stat(
                        text: "Duration",
                        formula: count => StackFormulas.Linear(count, 1.5f, 3.0f),
                        formatter: new TimeStatFormatter()
                        ),
                    new Stat(
                        text: "Move Speed",
                        formula: count => 0.40f,
                        formatter: new PercentageStatFormatter(),
                        stacks: false
                        ),
                    new Stat(
                        text: "1% Damage = 1% Proc. Chance (affected by Luck)",
                        formula: null,
                        formatter: new StatFormatter()
                        ),
                },
                [ItemIndex.HealOnCrit]              = new List<Stat>
                {
                    new Stat(
                        text: "Heal",
                        formula: count => StackFormulas.Linear(count, 4.0f, 8.0f),
                        formatter: new NumStatFormatter()
                        ),
                    new Stat(
                        text: "Crit. Chance",
                        formula: count => 0.05f,
                        formatter: new PercentageStatFormatter(),
                        stacks: false
                        )
                },
                [ItemIndex.EquipmentMagazine]       = new List<Stat>
                {
                    new Stat(
                        text: "Extra Charges",
                        formula: count => StackFormulas.Linear(count, 1f),
                        formatter: new NumStatFormatter()
                        ),
                    new Stat(
                        text: "Cooldown Reduction",
                        formula: count => StackFormulas.Exponential(count, 0.15f),
                        formatter: new PercentageStatFormatter()
                        )
                },
                [ItemIndex.Infusion]                = new List<Stat>
                {
                    new Stat(
                        text: "Max Health Increase",
                        formula: count => StackFormulas.Linear(count, 100f),
                        formatter: new NumStatFormatter()
                        ),
                    new Stat(
                        text: "Max Health per Kill",
                        formula: count => 1f,
                        formatter: new NumStatFormatter(),
                        stacks: false
                        )
                },
                [ItemIndex.Bandolier]               = new List<Stat>
                {
                    new Stat(
                        text: "Proc. Chance",
                        formula: count => StackFormulas.Special.Bandolier(count),
                        formatter: new PercentageStatFormatter(),
                        modifiers: LuckModifier.Instance
                        )
                },
                [ItemIndex.WarCryOnMultiKill]       = new List<Stat>
                {
                    new Stat(
                        text: "Duration",
                        formula: count => StackFormulas.Linear(count, 4.0f, 6.0f),
                        formatter: new TimeStatFormatter()
                        ),
                    new Stat(
                        text: "Kill Threshold",
                        formula: count => 3f,
                        formatter: new NumStatFormatter(),
                        stacks: false
                        ),
                    new Stat(
                        text: "Attack Speed",
                        formula: count => 1.00f,
                        formatter: new PercentageStatFormatter(),
                        stacks: false
                        ),
                    new Stat(
                        text: "Move Speed",
                        formula: count => 0.50f,
                        formatter: new PercentageStatFormatter(),
                        stacks: false
                        )
                },
                [ItemIndex.SprintArmor]             = new List<Stat>
                {
                    new Stat(
                        text: "Armor",
                        formula: count => StackFormulas.Linear(count, 30f),
                        formatter: new NumStatFormatter()
                        ),
                    new Stat(
                        text: "\tDamage Resistance",
                        formula: count => StackFormulas.Fractional(count * 30f, count * 30f + 100),
                        formatter: new PercentageStatFormatter()
                        ),
                },
                [ItemIndex.IceRing]                 = new List<Stat>
                {
                    new Stat(
                        text: "Damage",
                        formula: count => StackFormulas.Linear(count, 1.25f, 2.50f),
                        formatter: new PercentageStatFormatter()
                        ),
                    new Stat(
                        text: "Proc. Chance",
                        formula: count => 0.08f,
                        formatter: new PercentageStatFormatter(),
                        stacks: false,
                        modifiers: new StatModifier[] { LuckModifier.Instance, DualBandsModifier.Instance }
                        ),
                    new Stat(
                        text: "Slow Amount",
                        formula: count => 0.80f,
                        formatter: new PercentageStatFormatter(),
                        stacks: false
                        ),
                },
                [ItemIndex.FireRing]                = new List<Stat>
                {
                    new Stat(
                        text: "Damage",
                        formula: count => StackFormulas.Linear(count, 2.5f, 5.0f),
                        formatter: new PercentageStatFormatter()
                        ),
                    new Stat(
                        text: "Proc. Chance",
                        formula: count => 0.08f,
                        formatter: new PercentageStatFormatter(),
                        stacks: false,
                        modifiers: new StatModifier[] { LuckModifier.Instance, DualBandsModifier.Instance }
                        )
                },
                [ItemIndex.SlowOnHit]               = new List<Stat>
                {
                    new Stat(
                        text: "Duration",
                        formula: count => StackFormulas.Linear(count, 2.0f),
                        formatter: new TimeStatFormatter()
                        ),
                    new Stat(
                        text: "Slow Amount",
                        formula: count => 0.60f,
                        formatter: new PercentageStatFormatter(),
                        stacks: false
                        )
                },
                [ItemIndex.JumpBoost]               = new List<Stat>
                {
                    new Stat(
                        text: "Range",
                        formula: count => StackFormulas.Linear(count, 10f),
                        formatter: new DistanceStatFormatter()
                        )
                },
                [ItemIndex.ExecuteLowHealthElite]   = new List<Stat>
                {
                    new Stat(
                        text: "Health Threshold",
                        formula: count => StackFormulas.Hyperbolic(count, 0.20f),
                        formatter: new PercentageStatFormatter()
                        )
                },
                [ItemIndex.EnergizedOnEquipmentUse] = new List<Stat>
                {
                    new Stat(
                        text: "Duration",
                        formula: count => StackFormulas.Linear(count, 4.0f, 8.0f),
                        formatter: new TimeStatFormatter()
                        ),
                    new Stat(
                        text: "Attack Speed",
                        formula: count => 0.70f,
                        formatter: new PercentageStatFormatter(),
                        stacks: false
                        ),
                },
                [ItemIndex.TPHealingNova]           = new List<Stat>
                {
                    new Stat(
                        text: "Nova Count",
                        formula: count => StackFormulas.Linear(count, 1f),
                        formatter: new NumStatFormatter(),
                        modifiers: TPHealingNovaModifier.Instance
                        ),
                    new Stat(
                        text: "Heal",
                        formula: count => 0.5f,
                        formatter: new PercentageStatFormatter(),
                        stacks: false
                        )
                },
                [ItemIndex.Thorns]                  = new List<Stat>
                {
                    new Stat(
                        text: "Targets",
                        formula: count => StackFormulas.Linear(count, 2f, 5f),
                        formatter: new NumStatFormatter()
                        ),
                    new Stat(
                        text: "Range",
                        formula: count => StackFormulas.Linear(count, 10.0f, 25.0f),
                        formatter: new DistanceStatFormatter()
                        ),
                    new Stat(
                        text: "Damage",
                        formula: count => 1.6f,
                        formatter: new PercentageStatFormatter(),
                        stacks: false
                        )
                },
                [ItemIndex.BonusGoldPackOnKill]     = new List<Stat>
                {
                    new Stat(
                        text: "Proc. Chance",
                        formula: count => StackFormulas.Linear(count, 0.04f),
                        formatter: new PercentageStatFormatter(),
                        modifiers: LuckModifier.Instance
                        ),
                    new Stat(
                        text: "Current Gold Reward",
                        formula: count => ContextProvider.GetScaledGoldCost(25),
                        formatter: new NumStatFormatter(prefix: "$", color: ColorNeutral)
                        )
                },
                #endregion

                #region Legendary
                [ItemIndex.Behemoth]                = new List<Stat>
                {
                    new Stat(
                        text: "Radius",
                        formula: count => StackFormulas.Linear(count, 1.5f, 4.0f),
                        formatter: new DistanceStatFormatter()
                        ),
                    new Stat(
                        text: "Explosion Damage",
                        formula: count => 0.60f,
                        formatter: new PercentageStatFormatter(),
                        stacks: false
                        ),
                },
                [ItemIndex.Dagger]                  = new List<Stat>
                {
                    new Stat(
                        text: "Damage",
                        formula: count => StackFormulas.Linear(count, 1.5f),
                        formatter: new PercentageStatFormatter()
                        ),
                    new Stat(
                        text: "Dagger Count",
                        formula: count => 3,
                        formatter: new NumStatFormatter(),
                        stacks: false
                        ),
                },
                [ItemIndex.Icicle]                  = new List<Stat>
                {
                    new Stat(
                        text: "Max Radius",
                        formula: count => StackFormulas.Linear(count, 6f),
                        formatter: new DistanceStatFormatter()
                        ),
                    new Stat(
                        text: "Radius per Kill",
                        formula: count => 1f,
                        formatter: new DistanceStatFormatter(),
                        stacks: false
                        ),
                    new Stat(
                        text: "Damage",
                        formula: count => 6.0f,
                        formatter: new PercentageStatFormatter(),
                        stacks: false
                        ),
                },
                [ItemIndex.GhostOnKill]             = new List<Stat>
                {
                    new Stat(
                        text: "Ghost Duration",
                        formula: count => StackFormulas.Linear(count, 30f),
                        formatter: new TimeStatFormatter()
                        ),
                    new Stat(
                        text: "Proc. Chance",
                        formula: count => 0.07f,
                        formatter: new PercentageStatFormatter(),
                        stacks: false,
                        modifiers: LuckModifier.Instance
                        ),
                    new Stat(
                        text: "Ghost Damage",
                        formula: count => 15.0f,
                        formatter: new PercentageStatFormatter(),
                        stacks: false
                        ),
                },
                [ItemIndex.FallBoots]               = new List<Stat>
                {
                    new Stat(
                        text: "Recharge Time",
                        formula: count => StackFormulas.Fractional(10f, count),
                        formatter: new TimeStatFormatter()
                        ),
                    new Stat(
                        text: "Radius",
                        formula: count => 10.0f,
                        formatter: new DistanceStatFormatter(),
                        stacks: false
                        ),
                    new Stat(
                        text: "Base Damage",
                        formula: count => 23.0f,
                        formatter: new PercentageStatFormatter(),
                        stacks: false
                        ),
                },
                [ItemIndex.NovaOnHeal]              = new List<Stat>
                {
                    new Stat(
                        text: "Energy Stored",
                        formula: count => StackFormulas.Linear(count, 1.0f),
                        formatter: new PercentageStatFormatter()
                        ),
                    new Stat(
                        text: "Energy Threshold",
                        formula: count => 0.10f,
                        formatter: new PercentageStatFormatter(suffix: " of max health"),
                        stacks: false
                        ),
                    new Stat(
                        text: "Damage",
                        formula: count => 2.5f,
                        formatter: new PercentageStatFormatter(suffix: " of stored energy"),
                        stacks: false
                        ),
                },
                [ItemIndex.ShockNearby]             = new List<Stat>
                {
                    new Stat(
                        text: "Targets",
                        formula: count => StackFormulas.Linear(count, 2f, 3f),
                        formatter: new NumStatFormatter()
                        ),
                    new Stat(
                        text: "Damage",
                        formula: count => 2.0f,
                        formatter: new PercentageStatFormatter(),
                        stacks: false
                        ),
                    new Stat(
                        text: "Fire Rate",
                        formula: count => 0.5f,
                        formatter: new TimeStatFormatter(),
                        stacks: false
                        )
                },
                [ItemIndex.Clover]                  = new List<Stat>
                {
                    new Stat(
                        text: "Luck",
                        formula: count => StackFormulas.Linear(count, 1f),
                        formatter: new NumStatFormatter()
                        )
                },
                [ItemIndex.BounceNearby]            = new List<Stat>
                {
                    new Stat(
                        text: "Proc. Chance",
                        formula: count => StackFormulas.Fractional(count, count + 5),
                        formatter: new PercentageStatFormatter(),
                        modifiers: LuckModifier.Instance
                        ),
                    new Stat(
                        text: "Targets",
                        formula: count => StackFormulas.Linear(count, 5f, 10f),
                        formatter: new NumStatFormatter()
                        ),
                    new Stat(
                        text: "Damage",
                        formula: count => 1.0f,
                        formatter: new PercentageStatFormatter(),
                        stacks: false
                        ),
                },
                [ItemIndex.AlienHead]               = new List<Stat>
                {
                    new Stat(
                        text: "Cooldown Reduction",
                        formula: count => StackFormulas.Exponential(count, 0.25f),
                        formatter: new PercentageStatFormatter()
                        )
                },
                [ItemIndex.Talisman]                = new List<Stat>
                {
                    new Stat(
                        text: "Cooldown Reduction",
                        formula: count => StackFormulas.Linear(count, 2.0f, 4.0f),
                        formatter: new TimeStatFormatter()
                        )
                },
                [ItemIndex.ExtraLife]               = new List<Stat>
                {
                    new Stat(
                        text: "Extra Lives",
                        formula: count => StackFormulas.Linear(count, 1f),
                        formatter: new NumStatFormatter()
                        )
                },
                [ItemIndex.UtilitySkillMagazine]    = new List<Stat>
                {
                    new Stat(
                        text: "Utility Skill Charges",
                        formula: count => StackFormulas.Linear(count, 2f),
                        formatter: new NumStatFormatter()
                        ),
                    new Stat(
                        text: "Cooldown Reduction",
                        formula: count => 0.33f,
                        formatter: new PercentageStatFormatter(),
                        stacks: false
                        )
                },
                [ItemIndex.HeadHunter]              = new List<Stat>
                {
                    new Stat(
                        text: "Duration",
                        formula: count => StackFormulas.Linear(count, 5.0f, 8.0f),
                        formatter: new TimeStatFormatter()
                        )
                },
                [ItemIndex.KillEliteFrenzy]         = new List<Stat>
                {
                    new Stat(
                        text: "Duration",
                        formula: count => StackFormulas.Linear(count, 4f),
                        formatter: new TimeStatFormatter()
                        )
                },
                [ItemIndex.IncreaseHealing]         = new List<Stat>
                {
                    new Stat(
                        text: "Heal",
                        formula: count => StackFormulas.Linear(count, 1.0f),
                        formatter: new PercentageStatFormatter()
                        )
                },
                [ItemIndex.BarrierOnOverHeal]       = new List<Stat>
                {
                    new Stat(
                        text: "Barrier Granted",
                        formula: count => StackFormulas.Linear(count, 0.5f),
                        formatter: new PercentageStatFormatter(suffix: " of heal")
                        )
                },
                [ItemIndex.ArmorReductionOnHit]     = new List<Stat>
                {
                    new Stat(
                        text: "Duration",
                        formula: count => StackFormulas.Linear(count, 8f),
                        formatter: new TimeStatFormatter()
                        ),
                    new Stat(
                        text: "Armor Reduction",
                        formula: count => 60f,
                        formatter: new NumStatFormatter(),
                        stacks: false
                        )
                },
                [ItemIndex.LaserTurbine]            = new List<Stat>
                {
                    new Stat(
                        text: "Damage",
                        formula: count => StackFormulas.Linear(count, 3.0f),
                        formatter: new PercentageStatFormatter()
                        ),
                    new Stat(
                        text: "Explosion Damage",
                        formula: count => StackFormulas.Linear(count, 10.0f),
                        formatter: new PercentageStatFormatter()
                        ),
                    new Stat(
                        text: "Return Damage",
                        formula: count => StackFormulas.Linear(count, 3.0f),
                        formatter: new PercentageStatFormatter()
                        ),
                },
                #endregion

                #region Boss
                [ItemIndex.Knurl]                   = new List<Stat>
                {
                    new Stat(
                        text: "Max Health Increase",
                        formula: count => StackFormulas.Linear(count, 40.0f),
                        formatter: new NumStatFormatter()
                        ),
                    new Stat(
                        text: "Health Regen",
                        formula: count => StackFormulas.Linear(count, 1.6f),
                        formatter: new NumStatFormatter(suffix: " hp/s")
                        )
                },
                [ItemIndex.BeetleGland]             = new List<Stat>
                {
                    new Stat(
                        text: "Beetle Guards",
                        formula: count => StackFormulas.Linear(count, 1f),
                        formatter: new NumStatFormatter()
                        ),
                    new Stat(
                        text: "Beetle Damage",
                        formula: count => 3.0f,
                        formatter: new PercentageStatFormatter(),
                        stacks: false
                        ),
                    new Stat(
                        text: "Beetle Health",
                        formula: count => 1.0f,
                        formatter: new PercentageStatFormatter(),
                        stacks: false
                        ),
                },
                [ItemIndex.TitanGoldDuringTP]       = new List<Stat>
                {
                    new Stat(
                        text: "Aurelionite Damage",
                        formula: count => StackFormulas.Linear(count, 0.5f, 1.0f),
                        formatter: new PercentageStatFormatter()
                        ),
                    new Stat(
                        text: "Aurelionite Health",
                        formula: count => StackFormulas.Linear(count, 1.0f),
                        formatter: new PercentageStatFormatter()
                        ),
                },
                [ItemIndex.SprintWisp]              = new List<Stat>
                {
                    new Stat(
                        text: "Damage",
                        formula: count => StackFormulas.Linear(count, 1f),
                        formatter: new PercentageStatFormatter()
                        )
                },
                [ItemIndex.Pearl]                   = new List<Stat>
                {
                    new Stat(
                        text: "Max Health",
                        formula: count => StackFormulas.Linear(count, 0.1f),
                        formatter: new PercentageStatFormatter()
                        )
                },
                [ItemIndex.ShinyPearl]              = new List<Stat>
                {
                    new Stat(
                        text: "Max Health",
                        formula: count => StackFormulas.Linear(count, 0.1f),
                        formatter: new PercentageStatFormatter()
                        ),
                    new Stat(
                        text: "Health Regen",
                        formula: count => StackFormulas.Linear(count, 0.1f),
                        formatter: new PercentageStatFormatter()
                        ),
                    new Stat(
                        text: "Move Speed",
                        formula: count => StackFormulas.Linear(count, 0.1f),
                        formatter: new PercentageStatFormatter()
                        ),
                    new Stat(
                        text: "Damage",
                        formula: count => StackFormulas.Linear(count, 0.1f),
                        formatter: new PercentageStatFormatter()
                        ),
                    new Stat(
                        text: "Attack Speed",
                        formula: count => StackFormulas.Linear(count, 0.1f),
                        formatter: new PercentageStatFormatter()
                        ),
                    new Stat(
                        text: "Crit. Chance",
                        formula: count => StackFormulas.Linear(count, 0.1f),
                        formatter: new PercentageStatFormatter()
                        ),
                    new Stat(
                        text: "Armor Multiplier",
                        formula: count => StackFormulas.Linear(count, 0.1f),
                        formatter: new PercentageStatFormatter()
                        ),
                },
                [ItemIndex.NovaOnLowHealth]         = new List<Stat>
                {
                    new Stat(
                        text: "Recharge Time",
                        formula: count => StackFormulas.Fractional(30f, count + 1),
                        formatter: new TimeStatFormatter()
                        ),
                    new Stat(
                        text: "Damage",
                        formula: count => 60.0f,
                        formatter: new PercentageStatFormatter(),
                        stacks: false
                        ),
                    new Stat(
                        text: "Health Threshold",
                        formula: count => 0.25f,
                        formatter: new PercentageStatFormatter(),
                        stacks: false
                        )
                },
                #endregion

                #region Lunar
                [ItemIndex.LunarDagger]             = new List<Stat>
                {
                    new Stat(
                        text: "Damage",
                        formula: count => StackFormulas.Exponential(count, 1.0f),
                        formatter: new PercentageStatFormatter(color: ColorPositive)
                        ),
                    new Stat(
                        text: "Health Reduction",
                        formula: count => StackFormulas.Exponential(count, 0.5f, decreasing: true),
                        formatter: new PercentageStatFormatter(color: ColorNegative)
                        )
                },
                [ItemIndex.GoldOnHit]               = new List<Stat>
                {
                    new Stat(
                        text: "Gold Gain",
                        formula: count => StackFormulas.Special.GoldOnHit(count),
                        formatter: new NumStatFormatter(color: ColorPositive)
                        ),
                    new Stat(
                        text: "Gold Loss",
                        formula: count => StackFormulas.Linear(count, 1.0f),
                        formatter: new PercentageStatFormatter(suffix: " of health lost", color: ColorNegative)
                        ),
                    new Stat(
                        text: "Gain Chance",
                        formula: count => 0.3f,
                        formatter: new PercentageStatFormatter(),
                        stacks: false
                        ),
                },
                [ItemIndex.ShieldOnly]              = new List<Stat>
                {
                    new Stat(
                        text: "Max Health Increase",
                        formula: count => StackFormulas.Linear(count, 0.25f, 0.5f),
                        formatter: new PercentageStatFormatter()
                        )
                },
                [ItemIndex.RepeatHeal]              = new List<Stat>
                {
                    new Stat(
                        text: "Extra Healing",
                        formula: count => StackFormulas.Linear(count, 1.0f),
                        formatter: new PercentageStatFormatter(color: ColorPositive)
                        ),
                    new Stat(
                        text: "Max Healing Per Second",
                        formula: count => StackFormulas.Fractional(0.1f, count),
                        formatter: new PercentageStatFormatter(color: ColorNegative)
                        )
                },
                [ItemIndex.AutoCastEquipment]       = new List<Stat>
                {
                    new Stat(
                        text: "Cooldown Reduction",
                        formula: count => StackFormulas.Exponential(count, 0.15f, 0.5f),
                        formatter: new PercentageStatFormatter(color: ColorPositive)
                        )
                },
                [ItemIndex.LunarUtilityReplacement] = new List<Stat>
                {
                    new Stat(
                        text: "Heal",
                        formula: count => StackFormulas.Linear(count, 0.25f),
                        formatter: new PercentageStatFormatter()
                        ),
                    new Stat(
                        text: "Duration",
                        formula: count => StackFormulas.Linear(count, 3f),
                        formatter: new TimeStatFormatter()
                        )
                },
                [ItemIndex.LunarPrimaryReplacement] = new List<Stat>
                {
                    new Stat(
                        text: "Charges",
                        formula: count => StackFormulas.Linear(count, 12f),
                        formatter: new NumStatFormatter()
                        ),
                    new Stat(
                        text: "Recharge Time",
                        formula: count => StackFormulas.Linear(count, 2f),
                        formatter: new TimeStatFormatter()
                        ),
                    new Stat(
                        text: "Damage",
                        formula: count => 1.2f,
                        formatter: new PercentageStatFormatter(),
                        stacks: false
                        )
                },
                [ItemIndex.LunarTrinket]            = new List<Stat>
                {
                    new Stat(
                        text: "Enables access to 'A Moment, Whole'",
                        formula: null,
                        formatter: new StatFormatter()
                        )
                },
                #endregion

                #region None
                [ItemIndex.BoostHp]                 = new List<Stat>
                {
                    new Stat(
                        text: "Max Health",
                        formula: count => StackFormulas.Linear(count, 0.1f),
                        formatter: new PercentageStatFormatter()
                        )
                },
                [ItemIndex.BoostDamage]             = new List<Stat>
                {
                    new Stat(
                        text: "Damage",
                        formula: count => StackFormulas.Linear(count, 0.1f),
                        formatter: new PercentageStatFormatter()
                        )
                },
                [ItemIndex.ExtraLifeConsumed]       = new List<Stat>(),
                [ItemIndex.TonicAffliction]         = new List<Stat>
                {
                    new Stat(
                        text: "Max Health Decrease",
                        formula: count => StackFormulas.Linear(count, 0.05f),
                        formatter: new PercentageStatFormatter(color: ColorNegative, effectiveMax: 1.0f)
                        ),
                    new Stat(
                        text: "Health Regen Decrease",
                        formula: count => StackFormulas.Linear(count, 0.05f),
                        formatter: new PercentageStatFormatter(color: ColorNegative, effectiveMax: 1.0f)
                        ),
                    new Stat(
                        text: "Move Speed Decrease",
                        formula: count => StackFormulas.Linear(count, 0.05f),
                        formatter: new PercentageStatFormatter(color: ColorNegative, effectiveMax: 1.0f)
                        ),
                    new Stat(
                        text: "Damage Decrease",
                        formula: count => StackFormulas.Linear(count, 0.05f),
                        formatter: new PercentageStatFormatter(color: ColorNegative, effectiveMax: 1.0f)
                        ),
                    new Stat(
                        text: "Attack Speed Decrease",
                        formula: count => StackFormulas.Linear(count, 0.05f),
                        formatter: new PercentageStatFormatter(color: ColorNegative, effectiveMax: 1.0f)
                        ),
                    new Stat(
                        text: "Crit. Chance Decrease",
                        formula: count => StackFormulas.Linear(count, 0.05f),
                        formatter: new PercentageStatFormatter(color: ColorNegative, effectiveMax: 1.0f)
                        ),
                    new Stat(
                        text: "Armor Mult. Decrease",
                        formula: count => StackFormulas.Linear(count, 0.05f),
                        formatter: new PercentageStatFormatter(color: ColorNegative, effectiveMax: 1.0f)
                        ),
                },
                #endregion
            };
        }
        
        public static void UpdateEquipmentStatDefs()
        {
            EquipmentStatDefs = new Dictionary<EquipmentIndex, List<Stat>>
            {
                #region Normal
                [EquipmentIndex.Cleanse]                = new List<Stat>
                {
                    new Stat(
                        text: "Cooldown",
                        formula: count => 20f,
                        formatter: new TimeStatFormatter(color: ColorEquipmentCooldown)
                        ),
                },
                [EquipmentIndex.CommandMissile]         = new List<Stat>
                {
                    new Stat(
                        text: "Cooldown",
                        formula: count => 45f,
                        formatter: new TimeStatFormatter(color: ColorEquipmentCooldown)
                        ),
                    new Stat(
                        text: "Damage",
                        formula: count => 3.0f,
                        formatter: new PercentageStatFormatter(prefix: "12x")
                        ),
                },
                [EquipmentIndex.Gateway]                = new List<Stat>
                {
                    new Stat(
                        text: "Cooldown",
                        formula: count => 45f,
                        formatter: new TimeStatFormatter(color: ColorEquipmentCooldown)
                        ),
                    new Stat(
                        text: "Length",
                        formula: count => 1000.0f,
                        formatter: new DistanceStatFormatter()
                        ),
                },
                [EquipmentIndex.Fruit]                  = new List<Stat>
                {
                    new Stat(
                        text: "Cooldown",
                        formula: count => 45f,
                        formatter: new TimeStatFormatter(color: ColorEquipmentCooldown)
                        ),
                    new Stat(
                        text: "Heal",
                        formula: count => 0.5f,
                        formatter: new PercentageStatFormatter()
                        ),
                },
                [EquipmentIndex.QuestVolatileBattery]   = new List<Stat>
                {
                    new Stat(
                        text: "Used to unlock 'REX'",
                        formula: null,
                        formatter: new StatFormatter()
                        ),
                },
                [EquipmentIndex.PassiveHealing]         = new List<Stat>
                {
                    new Stat(
                        text: "Cooldown",
                        formula: count => 15f,
                        formatter: new TimeStatFormatter(color: ColorEquipmentCooldown)
                        ),
                    new Stat(
                        text: "Passive Healing",
                        formula: count => 0.015f,
                        formatter: new PercentageStatFormatter(suffix: " hp/s")
                        ),
                    new Stat(
                        text: "Active Healing",
                        formula: count => 0.10f,
                        formatter: new PercentageStatFormatter()
                        ),
                },
                [EquipmentIndex.GainArmor]              = new List<Stat>
                {
                    new Stat(
                        text: "Cooldown",
                        formula: count => 45f,
                        formatter: new TimeStatFormatter(color: ColorEquipmentCooldown)
                        ),
                    new Stat(
                        text: "Duration",
                        formula: count => 5f,
                        formatter: new TimeStatFormatter()
                        ),
                    new Stat(
                        text: "Armor",
                        formula: count => 500f,
                        formatter: new NumStatFormatter()
                        ),
                    new Stat(
                        text: "\tDamage Resistance",
                        formula: count => StackFormulas.Fractional(500f, 500f + 100),
                        formatter: new PercentageStatFormatter()
                        ),
                },
                [EquipmentIndex.Jetpack]                = new List<Stat>
                {
                    new Stat(
                        text: "Cooldown",
                        formula: count => 60f,
                        formatter: new TimeStatFormatter(color: ColorEquipmentCooldown)
                        ),
                    new Stat(
                        text: "Duration",
                        formula: count => 15f,
                        formatter: new TimeStatFormatter()
                        ),
                    new Stat(
                        text: "Move Speed",
                        formula: count => 0.2f,
                        formatter: new PercentageStatFormatter()
                        ),
                },
                [EquipmentIndex.CritOnUse]              = new List<Stat>
                {
                    new Stat(
                        text: "Cooldown",
                        formula: count => 60f,
                        formatter: new TimeStatFormatter(color: ColorEquipmentCooldown)
                        ),
                    new Stat(
                        text: "Duration",
                        formula: count => 8f,
                        formatter: new TimeStatFormatter()
                        ),
                    new Stat(
                        text: "Crit. Chance",
                        formula: count => 1.0f,
                        formatter: new PercentageStatFormatter()
                        ),
                },
                [EquipmentIndex.BFG]                    = new List<Stat>
                {
                    new Stat(
                        text: "Cooldown",
                        formula: count => 140f,
                        formatter: new TimeStatFormatter(color: ColorEquipmentCooldown)
                        ),
                    new Stat(
                        text: "Tendril Range",
                        formula: count => 35f,
                        formatter: new DistanceStatFormatter()
                        ),
                    new Stat(
                        text: "Tendril Damage",
                        formula: count => 6.0f,
                        formatter: new PercentageStatFormatter()
                        ),
                    new Stat(
                        text: "Explosion Range",
                        formula: count => 20f,
                        formatter: new DistanceStatFormatter()
                        ),
                    new Stat(
                        text: "Explosion Damage",
                        formula: count => 40.0f,
                        formatter: new PercentageStatFormatter()
                        ),
                },
                [EquipmentIndex.Blackhole]              = new List<Stat>
                {
                    new Stat(
                        text: "Cooldown",
                        formula: count => 60f,
                        formatter: new TimeStatFormatter(color: ColorEquipmentCooldown)
                        ),
                    new Stat(
                        text: "Duration",
                        formula: count => 10f,
                        formatter: new TimeStatFormatter()
                        ),
                    new Stat(
                        text: "Range",
                        formula: count => 30f,
                        formatter: new DistanceStatFormatter()
                        )
                },
                [EquipmentIndex.Scanner]                = new List<Stat>
                {
                    new Stat(
                        text: "Cooldown",
                        formula: count => 45f,
                        formatter: new TimeStatFormatter(color: ColorEquipmentCooldown)
                        ),
                    new Stat(
                        text: "Duration",
                        formula: count => 10f,
                        formatter: new TimeStatFormatter()
                        ),
                    new Stat(
                        text: "Range",
                        formula: count => 500f,
                        formatter: new DistanceStatFormatter()
                        )
                },
                [EquipmentIndex.Lightning]              = new List<Stat>
                {
                    new Stat(
                        text: "Cooldown",
                        formula: count => 20f,
                        formatter: new TimeStatFormatter(color: ColorEquipmentCooldown)
                        ),
                    new Stat(
                        text: "Damage",
                        formula: count => 30.0f,
                        formatter: new PercentageStatFormatter()
                        )
                },
                [EquipmentIndex.DroneBackup]            = new List<Stat>
                {
                    new Stat(
                        text: "Cooldown",
                        formula: count => 100f,
                        formatter: new TimeStatFormatter(color: ColorEquipmentCooldown)
                        ),
                    new Stat(
                        text: "Duration",
                        formula: count => 25f,
                        formatter: new TimeStatFormatter()
                        ),
                    new Stat(
                        text: "Drones",
                        formula: count => 4f,
                        formatter: new NumStatFormatter()
                        )
                },
                [EquipmentIndex.GoldGat]                = new List<Stat>
                {
                    new Stat(
                        text: "Cooldown",
                        formula: count => 0f,
                        formatter: new TimeStatFormatter(color: ColorEquipmentCooldown)
                        ),
                    new Stat(
                        text: "Bullet Cost",
                        formula: count => StackFormulas.Special.GoldGatBulletCost(),
                        formatter: new NumStatFormatter(prefix: "$", color: ColorNeutral)
                        ),
                    new Stat(
                        text: "Bullet Damage",
                        formula: count => 1.0f,
                        formatter: new PercentageStatFormatter()
                        )
                },
                [EquipmentIndex.FireBallDash]           = new List<Stat>
                {
                    new Stat(
                        text: "Cooldown",
                        formula: count => 30f,
                        formatter: new TimeStatFormatter(color: ColorEquipmentCooldown)
                        ),
                    new Stat(
                        text: "Duration",
                        formula: count => 5f,
                        formatter: new TimeStatFormatter()
                        ),
                    new Stat(
                        text: "Impact Damage",
                        formula: count => 5.0f,
                        formatter: new PercentageStatFormatter()
                        ),
                    new Stat(
                        text: "Explosion Damage",
                        formula: count => 8.0f,
                        formatter: new PercentageStatFormatter()
                        ),
                },
                #endregion

                #region Lunar
                [EquipmentIndex.CrippleWard]            = new List<Stat>
                {
                    new Stat(
                        text: "Cooldown",
                        formula: count => 0f,
                        formatter: new TimeStatFormatter(color: ColorEquipmentCooldown)
                        ),
                    new Stat(
                        text: "Radius",
                        formula: count => 16f,
                        formatter: new DistanceStatFormatter()
                        ),
                    new Stat(
                        text: "Slow Amount",
                        formula: count => 0.5f,
                        formatter: new PercentageStatFormatter()
                        ),
                    new Stat(
                        text: "Armor Reduction",
                        formula: count => 20f,
                        formatter: new NumStatFormatter()
                        ),
                    new Stat(
                        text: "\tDamage Resist",
                        formula: count => (2f - (100f / (100f - 20f))) - 1,
                        formatter: new PercentageStatFormatter()
                        ),
                },
                [EquipmentIndex.Meteor]                 = new List<Stat>
                {
                    new Stat(
                        text: "Cooldown",
                        formula: count => 140f,
                        formatter: new TimeStatFormatter(color: ColorEquipmentCooldown)
                        ),
                    new Stat(
                        text: "Duration",
                        formula: count => 20f,
                        formatter: new TimeStatFormatter()
                        ),
                    new Stat(
                        text: "Damage",
                        formula: count => 6.0f,
                        formatter: new PercentageStatFormatter()
                        )
                },
                [EquipmentIndex.BurnNearby]             = new List<Stat>
                {
                    new Stat(
                        text: "Cooldown",
                        formula: count => 45f,
                        formatter: new TimeStatFormatter(color: ColorEquipmentCooldown)
                        ),
                    new Stat(
                        text: "Range",
                        formula: count => 8f,
                        formatter: new DistanceStatFormatter()
                        ),
                    new Stat(
                        text: "Damage To Yourself",
                        formula: count => 0.05f,
                        formatter: new PercentageStatFormatter()
                        ),
                    new Stat(
                        text: "Damage To Friendlies",
                        formula: count => 0.025f,
                        formatter: new PercentageStatFormatter()
                        ),
                    new Stat(
                        text: "Damage To Enemies",
                        formula: count => 1.2f,
                        formatter: new PercentageStatFormatter()
                        )
                },
                [EquipmentIndex.Tonic]                  = new List<Stat>
                {
                    new Stat(
                        text: "Cooldown",
                        formula: count => 60f,
                        formatter: new TimeStatFormatter(color: ColorEquipmentCooldown)
                        ),
                    new Stat(
                        text: "Damage",
                        formula: count => 1.0f,
                        formatter: new PercentageStatFormatter()
                        ),
                    new Stat(
                        text: "Attack Speed",
                        formula: count => 0.7f,
                        formatter: new PercentageStatFormatter()
                        ),
                    new Stat(
                        text: "Armor",
                        formula: count => 20f,
                        formatter: new NumStatFormatter()
                        ),
                    new Stat(
                        text: "Max Health",
                        formula: count => 0.5f,
                        formatter: new PercentageStatFormatter()
                        ),
                    new Stat(
                        text: "Health Regen",
                        formula: count => 3.0f,
                        formatter: new PercentageStatFormatter()
                        ),
                    new Stat(
                        text: "Move Speed",
                        formula: count => 0.3f,
                        formatter: new PercentageStatFormatter()
                        ),
                    new Stat(
                        text: "Affliction Chance",
                        formula: count => 0.2f,
                        formatter: new PercentageStatFormatter(color: ColorNegative)
                        ),
                },
                #endregion

                #region Elite
                [EquipmentIndex.AffixWhite] = new List<Stat>(),
                [EquipmentIndex.AffixRed] = new List<Stat>(),
                [EquipmentIndex.AffixPoison] = new List<Stat>(),
                [EquipmentIndex.AffixBlue] = new List<Stat>(),
                [EquipmentIndex.AffixHaunted] = new List<Stat>(),
                [EquipmentIndex.AffixGold] = new List<Stat>(),
                [EquipmentIndex.AffixYellow] = new List<Stat>(),
                #endregion
            };
        }
    }
}
