using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnosMods.TabItemStats.Formatters;
using UnosMods.TabItemStats.AbstractModifiers;

namespace UnosMods.TabItemStats
{
    public static partial class PickupStatProvider
    {
        private static float StackFormula(float itemCount, float stackVal, float initialVal = 0f)
        {
            if (initialVal > 0f)
                return (itemCount - 1) * stackVal + initialVal;
            else
                return itemCount * stackVal;
        }

        public static Dictionary<ItemIndex, List<ItemStat>> itemDefs;

        public static void UpdateItemDefs()
        {
            TabItemStats.Logger.LogDebug("Updating item defs...");
            itemDefs = new Dictionary<ItemIndex, List<ItemStat>>
            {
                #region Tier 1 Items
                [ItemIndex.Syringe] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => count* 0.15f,
                        statText: "Attack Speed"
                        )
                },
                [ItemIndex.Bear] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => 1f - 1f / (0.15f * count + 1f),
                        statText: "Block Chance"
                        )
                },
                [ItemIndex.Tooth] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => StackFormula(count, 6),
                        statText: "Health per Orb",
                        formatter: new IntFormatter(suffix: " HP")
                        )
                },
                [ItemIndex.CritGlasses] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => StackFormula(count, 0.1f),
                        statText: "Critical Chance",
                        formatter: new PercentageFormatter(maxValue: 1f),
                        modifiers: Modifiers.Luck
                        )
                },
                [ItemIndex.Hoof] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => StackFormula(count, 0.14f),
                        statText: "Movement Increase"
                        )
                },
                [ItemIndex.Mushroom] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => StackFormula(count, 0.0225f, 0.045f),
                        statText: "Healing per Second",
                        formatter: new PercentageFormatter(maxValue: 1f)
                        ),
                    new ItemStat(
                        formula: count => StackFormula(count, 1.5f, 3f),
                        statText: "Effect Radius",
                        formatter: new FloatFormatter(suffix: "m")
                        ),
                    new ItemStat(
                        formula: count => 2f,
                        statText: "Time until Heal",
                        doesNotStack: true,
                        formatter: new FloatFormatter(color: DoesNotStackColor, suffix: "s")
                        )
                },
                [ItemIndex.Crowbar] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => StackFormula(count, 0.5f),
                        statText: "Damage Boost"
                        ),
                    new ItemStat(
                        formula: count => 0.9f,
                        statText: "Health Threshold",
                        doesNotStack: true,
                        formatter: new PercentageFormatter(color: DoesNotStackColor)
                        )
                },
                [ItemIndex.BleedOnHit] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => StackFormula(count, 0.15f),
                        statText: "Bleed Chance",
                        formatter: new PercentageFormatter(maxValue: 1f),
                        modifiers: Modifiers.Luck
                        ),
                    new ItemStat(
                        formula: count => 2.4f,
                        statText: "Bleed Damage (of base)",
                        doesNotStack: true,
                        formatter: new PercentageFormatter(color: DoesNotStackColor)
                        ),
                },
                [ItemIndex.WardOnLevel] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => StackFormula(count, 8, 16),
                        statText: "Radius",
                        formatter: new IntFormatter(suffix: "m")
                        ),
                    new ItemStat(
                        formula: count => 0.3f,
                        statText: "Attack Speed",
                        doesNotStack: true,
                        formatter: new PercentageFormatter(color: DoesNotStackColor)
                        ),
                    new ItemStat(
                        formula: count => 0.3f,
                        statText: "Movement Speed",
                        doesNotStack: true,
                        formatter: new PercentageFormatter(color: DoesNotStackColor)
                        )
                },
                [ItemIndex.HealWhileSafe] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => StackFormula(count, 3f),
                        statText: "Regen. Rate"
                        )
                },
                [ItemIndex.PersonalShield] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => StackFormula(count, 25),
                        statText: "Shields",
                        formatter: new IntFormatter(suffix: " SP")
                        )
                },
                [ItemIndex.Medkit] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => StackFormula(count, 10),
                        statText: "Health",
                        formatter: new IntFormatter(suffix: " HP")
                        ),
                    new ItemStat(
                        formula: count => 1.1f,
                        statText: "Time until Heal",
                        doesNotStack: true,
                        formatter: new FloatFormatter(color: DoesNotStackColor, suffix: "s")
                        )
                },
                [ItemIndex.IgniteOnKill] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => StackFormula(count, 4f, 12f),
                        statText: "Radius",
                        formatter: new IntFormatter(suffix: "m")
                        ),
                    new ItemStat(
                        formula: count => StackFormula(count, 0.75f, 1.5f),
                        statText: "Burn Damage (of base)"
                        )
                },
                [ItemIndex.StunChanceOnHit] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => 1 - 1 / (StackFormula(count, 0.05f) + 1),
                        statText: "Stun Chance",
                        formatter: new PercentageFormatter(maxValue: 1f),
                        modifiers: Modifiers.Luck
                        ),
                    new ItemStat(
                        formula: count => 2f,
                        statText: "Duration",
                        doesNotStack: true,
                        formatter: new FloatFormatter(color: DoesNotStackColor, suffix: "s")
                        )
                },
                [ItemIndex.Firework] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => StackFormula(count, 4, 8),
                        statText: "Amount",
                        formatter: new IntFormatter()
                        ),
                    new ItemStat(
                        formula: count => 3.0f,
                        statText: "Damage (of base)",
                        doesNotStack: true,
                        formatter: new PercentageFormatter(color: DoesNotStackColor)
                        )
                },
                [ItemIndex.SprintBonus] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => StackFormula(count, 0.2f, 0.3f),
                        statText: "Sprint Speed"
                        )
                },
                [ItemIndex.SecondarySkillMagazine] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => count,
                        statText: "Additional Charges",
                        formatter: new IntFormatter()
                        )
                },
                [ItemIndex.StickyBomb] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => StackFormula(count, 0.05f),
                        statText: "Proc. Chance",
                        formatter: new PercentageFormatter(maxValue: 1f),
                        modifiers: Modifiers.Luck
                        ),
                    new ItemStat(
                        formula: count => 1.8f,
                        statText: "Damage",
                        doesNotStack: true,
                        formatter: new PercentageFormatter(color: DoesNotStackColor)
                        )
                },
                [ItemIndex.TreasureCache] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => 80f / (80f + StackFormula(count, 20f) + Mathf.Pow(count, 2f)),
                        statText: "Common Chance",
                        formatter: new PercentageFormatter(maxValue: 1f, color: ColorCatalog.ColorIndex.Tier1Item.ToHex()),
                        modifiers: Modifiers.TreasureCache
                        ),
                    new ItemStat(
                        formula: count => StackFormula(count, 20f) / (80f + StackFormula(count, 20f) + Mathf.Pow(count, 2f)),
                        statText: "Uncommon Chance",
                        formatter: new PercentageFormatter(maxValue: 1f, color: ColorCatalog.ColorIndex.Tier2Item.ToHex()),
                        modifiers: Modifiers.TreasureCache
                        ),
                    new ItemStat(
                        formula: count => Mathf.Pow(count, 2f) / (80f + StackFormula(count, 20f) + Mathf.Pow(count, 2f)),
                        statText: "Legendary Chance",
                        formatter: new PercentageFormatter(maxValue: 1f, color: ColorCatalog.ColorIndex.Tier3Item.ToHex()),
                        modifiers: Modifiers.TreasureCache
                        )
                },
                [ItemIndex.BossDamageBonus] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => StackFormula(count, 0.2f),
                        statText: "Additional Boss Damage"
                        )
                },
                [ItemIndex.BarrierOnKill] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => StackFormula(count, 15),
                        statText: "Barrier Gain",
                        formatter: new IntFormatter(suffix: " BP")
                        )
                },
                [ItemIndex.NearbyDamageBonus] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => StackFormula(count, 0.15f),
                        statText: "Damage Boost"
                        ),
                    new ItemStat(
                        formula: count => 13,
                        statText: "Range",
                        doesNotStack: true,
                        formatter: new FloatFormatter(suffix: "m", color: DoesNotStackColor)
                        ),
                }, 
                #endregion

                #region Tier 2 Items
                [ItemIndex.Missile] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => StackFormula(count, 3f),
                        statText: "Damage"
                        ),
                    new ItemStat(
                        formula: count => 0.1f,
                        statText: "Proc. Chance",
                        doesNotStack: true,
                        formatter: new PercentageFormatter(maxValue: 1f, color: DoesNotStackColor),
                        modifiers: Modifiers.Luck
                        )
                },
                [ItemIndex.ExplodeOnDeath] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => StackFormula(count, 2.4f, 12f),
                        statText: "Radius",
                        formatter: new FloatFormatter(suffix: "m")
                        ),
                    new ItemStat(
                        formula: count => StackFormula(count, 2.8f, 3.5f),
                        statText: "Damage (of base)"
                        )
                },
                [ItemIndex.Feather] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => count,
                        statText: "Additional Jumps",
                        formatter: new IntFormatter()
                        )
                },
                [ItemIndex.ChainLightning] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => StackFormula(count, 2, 3),
                        statText: "Chain Targets",
                        formatter: new IntFormatter()
                        ),
                    new ItemStat(
                        formula: count => StackFormula(count, 2f, 20f),
                        statText: "Radius",
                        formatter: new FloatFormatter(suffix: "m")
                        ),
                    new ItemStat(
                        formula: count => 0.25f,
                        statText: "Proc. Chance",
                        doesNotStack: true,
                        formatter: new PercentageFormatter(maxValue: 1f, color: DoesNotStackColor),
                        modifiers: Modifiers.Luck
                        ),
                    new ItemStat(
                        formula: count => 0.80f,
                        statText: "Damage",
                        doesNotStack: true,
                        formatter: new PercentageFormatter(color: DoesNotStackColor)
                        )
                },
                [ItemIndex.Seed] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => count,
                        statText: "Health per Hit (Proc Coeff.)",
                        formatter: new IntFormatter(suffix: " HP")
                        )
                },
                [ItemIndex.AttackSpeedOnCrit] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => StackFormula(count, 0.3f),
                        statText: "Max Attack Speed"
                        ),
                    new ItemStat(
                        formula: count => 0.1f,
                        statText: "Attack Speed on Critical",
                        doesNotStack: true,
                        formatter: new PercentageFormatter(color: DoesNotStackColor)
                        ),
                    new ItemStat(
                        formula: count => 0.05f,
                        statText: "Critical Chance",
                        doesNotStack: true,
                        formatter: new PercentageFormatter(color: DoesNotStackColor),
                        modifiers: Modifiers.Luck
                        )
                },
                [ItemIndex.SprintOutOfCombat] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => StackFormula(count, 0.3f),
                        statText: "Movement Speed"
                        )
                },
                [ItemIndex.Phasing] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => StackFormula(count, 1.5f, 3f),
                        statText: "Duration",
                        formatter: new FloatFormatter(suffix: "s")
                        ),
                    new ItemStat(
                        formula: count => 0.01f,
                        statText: "Invis. Chance",
                        doesNotStack: true,
                        formatter: new PercentageFormatter(maxValue: 1f, suffix: " per % of max health damage taken", color: DoesNotStackColor),
                        modifiers: Modifiers.Luck
                        ),
                    new ItemStat(
                        formula: count => 0.4f,
                        statText: "Invis. Extra Speed",
                        doesNotStack: true,
                        formatter: new PercentageFormatter(color: DoesNotStackColor)
                        )
                },
                [ItemIndex.HealOnCrit] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => StackFormula(count, 4, 8),
                        statText: "Health on Critical",
                        formatter: new IntFormatter()
                        ),
                    new ItemStat(
                        formula: count => 0.05f,
                        statText: "Critical Chance",
                        doesNotStack: true,
                        formatter: new PercentageFormatter(color: DoesNotStackColor),
                        modifiers: Modifiers.Luck
                        )
                },
                [ItemIndex.EquipmentMagazine] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => count,
                        statText: "Additional Charges",
                        formatter: new IntFormatter()
                        ),
                    new ItemStat(
                        formula: count => 1f - Mathf.Pow(1f - 0.15f, count),
                        statText: "Equipment Cooldown Reduction"
                        )
                },
                [ItemIndex.Infusion] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => StackFormula(count, 100),
                        statText: "Max Health Increase",
                        formatter: new IntFormatter(suffix: " HP")
                        ),
                    new ItemStat(
                        formula: count => 1,
                        statText: "Health Increase per Kill",
                        doesNotStack: true,
                        formatter: new IntFormatter(suffix: " HP", color: DoesNotStackColor)
                        )
                },
                [ItemIndex.Bandolier] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => 1 - 1 / Mathf.Pow(count + 1f, 0.33f),
                        statText: "Drop Chance",
                        formatter: new PercentageFormatter(maxValue: 1f),
                        modifiers: Modifiers.Luck
                        )
                },
                [ItemIndex.WarCryOnMultiKill] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => StackFormula(count, 4f, 6f),
                        statText: "Frenzy Duration",
                        formatter: new IntFormatter()
                        ),
                    new ItemStat(
                        formula: count => 0.5f,
                        statText: "Frenzy Movement Speed",
                        doesNotStack: true,
                        formatter: new PercentageFormatter(color: DoesNotStackColor)
                        ),
                    new ItemStat(
                        formula: count => 1f,
                        statText: "Frenzy Attack Speed",
                        doesNotStack: true,
                        formatter: new PercentageFormatter(color: DoesNotStackColor)
                        )
                },
                [ItemIndex.SprintArmor] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => StackFormula(count, 30),
                        statText: "Armor Stacks",
                        formatter: new IntFormatter()
                        ),
                    new ItemStat(
                        formula: count => StackFormula(count, 30) / (100 + StackFormula(count, 30)),
                        statText: "Sprinting Damage Reduction",
                        formatter: new PercentageFormatter(maxValue: 1f)
                        )
                },
                [ItemIndex.IceRing] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => StackFormula(count, 1.25f, 2.5f),
                        statText: "Damage"
                        ),
                    new ItemStat(
                        formula: count => 0.80f,
                        statText: "Slow Percentage",
                        doesNotStack: true,
                        formatter: new PercentageFormatter(color: DoesNotStackColor)
                        ),
                    new ItemStat(
                        formula: count => 0.08f,
                        statText: "Proc. Chance",
                        doesNotStack: true,
                        formatter: new PercentageFormatter(color: DoesNotStackColor),
                        modifiers: new AbstractModifier[] { Modifiers.Luck, Modifiers.BothBands }
                        )
                },
                [ItemIndex.FireRing] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => StackFormula(count, 2.5f, 5f),
                        statText: "Damage"
                        ),
                    new ItemStat(
                        formula: count => 0.08f,
                        statText: "Proc. Chance",
                        doesNotStack: true,
                        formatter: new PercentageFormatter(color: DoesNotStackColor),
                        modifiers: new AbstractModifier[] { Modifiers.Luck, Modifiers.BothBands }
                        )
                },
                [ItemIndex.SlowOnHit] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => StackFormula(count, 2),
                        statText: "Slow Time",
                        formatter: new IntFormatter(suffix: "s")
                        ),
                    new ItemStat(
                        formula: count => 0.6f,
                        statText: "Slow Percentage",
                        doesNotStack: true,
                        formatter: new PercentageFormatter(color: DoesNotStackColor)
                        )
                },
                [ItemIndex.JumpBoost] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => StackFormula(count, 10),
                        statText: "Jump Distance",
                        formatter: new IntFormatter(suffix: "m")
                        )
                },
                [ItemIndex.EnergizedOnEquipmentUse] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => StackFormula(count, 4, 8),
                        statText: "Effect Duration",
                        formatter: new IntFormatter(suffix: "s")
                        ),
                    new ItemStat(
                        formula: count => 0.70f,
                        statText: "Attack Speed",
                        doesNotStack: true,
                        formatter: new PercentageFormatter(color: DoesNotStackColor)
                        )
                },
                [ItemIndex.ExecuteLowHealthElite] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => 1 - 1 / (StackFormula(count, 0.2f) + 1f),
                        statText: "Execute Threshold",
                        formatter: new PercentageFormatter(maxValue: 1f)
                        )
                },
                [ItemIndex.TPHealingNova] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => StackFormula(count, 1f),
                        statText: "Number of Pulses",
                        formatter: new IntFormatter()
                        ),
                    new ItemStat(
                        formula: count => 0.5f,
                        statText: "Health",
                        doesNotStack: true,
                        formatter: new PercentageFormatter(suffix: " HP", color: DoesNotStackColor)
                        ),
                },
                #endregion

                #region Tier 3 Items
                [ItemIndex.Behemoth] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => StackFormula(count, 1.5f, 4f),
                        statText: "Explosion Radius",
                        formatter: new FloatFormatter(suffix: "m")
                        ),
                    new ItemStat(
                        formula: count => 0.6f,
                        statText: "Bonus Damage",
                        doesNotStack: true,
                        formatter: new PercentageFormatter(color: DoesNotStackColor)
                        )
                },
                [ItemIndex.Dagger] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => StackFormula(count, 1.5f),
                        statText: "Damage (of base)"
                        ),
                    new ItemStat(
                        formula: count => 3,
                        statText: "Daggers on Kill",
                        doesNotStack: true,
                        formatter: new IntFormatter(color: DoesNotStackColor)
                        )
                },
                [ItemIndex.Icicle] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => StackFormula(count, 6),
                        statText: "Max Radius",
                        formatter: new IntFormatter(suffix: "m")
                        ),
                    new ItemStat(
                        formula: count => 6f,
                        statText: "Damage",
                        doesNotStack: true,
                        formatter: new PercentageFormatter(color: DoesNotStackColor)
                        ),
                    new ItemStat(
                        formula: count => 1,
                        statText: "Radius Increase per Kill",
                        doesNotStack: true,
                        formatter: new IntFormatter(suffix: "m", color: DoesNotStackColor)
                        )
                },
                [ItemIndex.GhostOnKill] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => StackFormula(count, 30),
                        statText: "Ghost Duration",
                        formatter: new IntFormatter(suffix: "s")
                        ),
                    new ItemStat(
                        formula: count => 0.1f,
                        statText: "Spawn Chance",
                        doesNotStack: true,
                        formatter: new PercentageFormatter(maxValue: 1f, color: DoesNotStackColor),
                        modifiers: Modifiers.Luck
                        ),
                    new ItemStat(
                        formula: count => 5.0f,
                        statText: "Ghost Damage",
                        doesNotStack: true,
                        formatter: new PercentageFormatter(color: DoesNotStackColor)
                        )
                },
                [ItemIndex.FallBoots] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => Mathf.Pow(1f - 0.5f, count),
                        statText: "Recharge Time",
                        formatter: new FloatFormatter(suffix: "s", places: 3)
                        ),
                    new ItemStat(
                        formula: count => 10,
                        statText: "Explosion Radius",
                        doesNotStack: true,
                        formatter: new FloatFormatter(suffix: "m", color: DoesNotStackColor)
                        ),
                    new ItemStat(
                        formula: count => 23f,
                        statText: "Damage (of base)",
                        doesNotStack: true,
                        formatter: new FloatFormatter(suffix: " (scales with speed)", color: DoesNotStackColor)
                        )
                },
                [ItemIndex.NovaOnHeal] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => StackFormula(count, 1f),
                        statText: "Healing Stored as Soul Energy"
                        ),
                    new ItemStat(
                        formula: count => 0.1f,
                        statText: "Max Health Threshold",
                        doesNotStack: true,
                        formatter: new PercentageFormatter(maxValue: 1f, color: DoesNotStackColor)
                        ),
                    new ItemStat(
                        formula: count => 2.5f,
                        statText: "Damage (of Soul Energy Stored)",
                        doesNotStack: true,
                        formatter: new PercentageFormatter(color: DoesNotStackColor)
                        )
                },
                [ItemIndex.ShockNearby] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => StackFormula(count, 2),
                        statText: "Bounces",
                        formatter: new IntFormatter()
                        ),
                    new ItemStat(
                        formula: count => 2f,
                        statText: "Damage (of base)",
                        doesNotStack: true,
                        formatter: new PercentageFormatter(color: DoesNotStackColor)
                        ),
                    new ItemStat(
                        formula: count => 0.5f,
                        statText: "Damage Rate",
                        doesNotStack: true,
                        formatter: new FloatFormatter(suffix: "s", color: DoesNotStackColor)
                        )
                },
                [ItemIndex.Clover] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => count,
                        statText: "Luck",
                        formatter: new IntFormatter()
                        )
                },
                [ItemIndex.BounceNearby] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => 1f - 1f / (count* 0.20f + 1f),
                        statText: "Proc. Chance",
                        formatter: new PercentageFormatter(maxValue: 1f),
                        modifiers: Modifiers.Luck
                        ),
                    new ItemStat(
                        formula: count => StackFormula(count, 5, 10),
                        statText: "Hook Bounces",
                        formatter: new IntFormatter()
                        ),
                    new ItemStat(
                        formula: count => 1f,
                        statText: "Damage",
                        doesNotStack: true,
                        formatter: new PercentageFormatter(color: DoesNotStackColor)
                        )
                },
                [ItemIndex.AlienHead] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => 1f - Mathf.Pow(1f - 0.25f, count),
                        statText: "Skill Cooldown Reduction"
                        )
                },
                [ItemIndex.Talisman] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => StackFormula(count, 2f, 4f),
                        statText: "Cooldown Reduction",
                        formatter: new IntFormatter(suffix: "s")
                        )
                },
                [ItemIndex.ExtraLife] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => count,
                        statText: "Extra Lives",
                        formatter: new IntFormatter()
                        ),
                    new ItemStat(
                        formula: count => 3,
                        statText: "Invulnerability Duration",
                        doesNotStack: true,
                        formatter: new IntFormatter(suffix: "s", color: DoesNotStackColor)
                        )
                },
                [ItemIndex.UtilitySkillMagazine] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => StackFormula(count, 2),
                        statText: "Additional Utility Charges",
                        formatter: new IntFormatter()
                        ),
                    new ItemStat(
                        formula: count => 0.33f,
                        statText: "Skill Cooldown Reduction",
                        doesNotStack: true,
                        formatter: new PercentageFormatter(color: DoesNotStackColor)
                        )
                },
                [ItemIndex.HeadHunter] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => StackFormula(count, 5, 8),
                        statText: "Empowered Duration",
                        formatter: new IntFormatter(suffix: "s")
                        )
                },
                [ItemIndex.KillEliteFrenzy] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => StackFormula(count, 2, 3),
                        statText: "Frenzy Duration",
                        formatter: new IntFormatter(suffix: "s")
                        ),
                    new ItemStat(
                        formula: count => 0.5f,
                        statText: "Skill Cooldown",
                        doesNotStack: true,
                        formatter: new FloatFormatter(suffix: "s", color: DoesNotStackColor)
                        )
                },
                [ItemIndex.IncreaseHealing] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => StackFormula(count, 1f),
                        statText: "Additional Healing"
                        )
                },
                [ItemIndex.BarrierOnOverHeal] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => StackFormula(count, 0.5f),
                        statText: "Overheal (% of healing gained)"
                        )
                },
                [ItemIndex.ArmorReductionOnHit] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => StackFormula(count, 8f),
                        statText: "Reduction Duration",
                        formatter: new FloatFormatter(suffix: "s")
                        ),
                    new ItemStat(
                        formula: count => 5,
                        statText: "Hits Required",
                        doesNotStack: true,
                        formatter: new IntFormatter(color: DoesNotStackColor)
                        ),
                    new ItemStat(
                        formula: count => 60,
                        statText: "Armor Reduction",
                        doesNotStack: true,
                        formatter: new IntFormatter(color: DoesNotStackColor)
                        ),
                }, 
                #endregion

                #region Boss Items
                [ItemIndex.Knurl] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => StackFormula(count, 40),
                        statText: "Max Health Increase",
                        formatter: new IntFormatter(suffix: " HP")
                        ),
                    new ItemStat(
                        formula: count => StackFormula(count, 1.6f),
                        statText: "Additional Regeneration",
                        formatter: new FloatFormatter(suffix: " HP")
                        )
                },
                [ItemIndex.BeetleGland] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => StackFormula(count, 1),
                        statText: "Beetle Count",
                        formatter: new IntFormatter()
                        ),
                    new ItemStat(
                        formula: count => 30f,
                        statText: "Summon Timer",
                        doesNotStack: true,
                        formatter: new PercentageFormatter(suffix: "s", color: DoesNotStackColor)
                        ),
                    new ItemStat(
                        formula: count => 3f,
                        statText: "Beetle Damage",
                        doesNotStack: true,
                        formatter: new PercentageFormatter(color: DoesNotStackColor)
                        ),
                    new ItemStat(
                        formula: count => 1f,
                        statText: "Beetle Health",
                        doesNotStack: true,
                        formatter: new PercentageFormatter(suffix: " HP", color: DoesNotStackColor)
                        )
                },
                [ItemIndex.TitanGoldDuringTP] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => StackFormula(count, 0.5f, 1f),
                        statText: "Aurelionite Damage"
                        ),
                    new ItemStat(
                        formula: count => StackFormula(count, 1f),
                        statText: "Aurelionite Health",
                        formatter: new PercentageFormatter(suffix: " HP")
                        )
                },
                [ItemIndex.SprintWisp] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => StackFormula(count, 1f),
                        statText: "Wisp Damage"
                        ),
                    new ItemStat(
                        formula: count => 0.5f,
                        statText: "Fire Rate",
                        doesNotStack: true,
                        formatter: new FloatFormatter(suffix: "s", color: DoesNotStackColor)
                        )
                }, 
                #endregion

                #region Lunar Items
                [ItemIndex.LunarDagger] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => Mathf.Pow(2, count),
                        statText: "Base Damage Increase"
                        ),
                    new ItemStat(
                        formula: count => 1f - Mathf.Pow(1f - 0.5f, count),
                        statText: "Max Health Decrease",
                        doesNotStack: true,
                        formatter: new PercentageFormatter(color: NegativeColor)
                        )
                },
                [ItemIndex.GoldOnHit] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => count* 2f * Run.instance?.difficultyCoefficient ?? 1f,
                        statText: "Gold on Hit",
                        formatter: new FloatFormatter()
                        ),
                    new ItemStat(
                        formula: count => StackFormula(count, 1f),
                        statText: "Lost Gold on Hit",
                        formatter: new PercentageFormatter(suffix: " of damage taken", color: NegativeColor)
                        ),
                    new ItemStat(
                        formula: count => 0.3f,
                        statText: "Proc. Chance",
                        doesNotStack: true,
                        formatter: new PercentageFormatter(color: DoesNotStackColor),
                        modifiers: Modifiers.Luck
                        )
                },
                [ItemIndex.ShieldOnly] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => StackFormula(count, 0.25f, 0.5f),
                        statText: "Max Health Increase"
                        )
                },
                [ItemIndex.RepeatHeal] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => StackFormula(count, 1f),
                        statText: "Additional Healing"
                        ),
                    new ItemStat(
                        formula: count => 1f - Mathf.Pow(1f - 0.1f, count),
                        statText: "Healing Fraction/s"
                        )
                },
                [ItemIndex.AutoCastEquipment] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => 1 - 0.5f * Mathf.Pow(1f - 0.15f, (count - 1)),
                        statText: "Cooldown Decrease"
                        )
                },
                [ItemIndex.LunarUtilityReplacement] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => 1f - Mathf.Pow(1f - 0.25f, count),
                        statText: "Healing Received"
                        ),
                    new ItemStat(
                        formula: count => StackFormula(count, 3),
                        statText: "Duration",
                        formatter: new IntFormatter(suffix: "s")
                        ),
                    new ItemStat(
                        formula: count => 0.3f,
                        statText: "Move Speed",
                        doesNotStack: true,
                        formatter: new PercentageFormatter(color: DoesNotStackColor)
                        )
                },
                #endregion

                #region No Tier
                [ItemIndex.ExtraLifeConsumed] = new List<ItemStat>(),
                [ItemIndex.TonicAffliction] = new List<ItemStat>
                {
                    new ItemStat(
                        formula: count => StackFormula(count, 0.05f),
                        statText: "Stats Decrease",
                        formatter: new PercentageFormatter(color: NegativeColor)
                        )
                    /*new ItemStat(
                        formula: count => StackFormula(count, 0.05f),
                        statText: "Damage Decrease",
                        formatter: new PercentageFormatter(color: ColorCatalog.ColorIndex.Tier2Item.ToHex())
                        ),
                    new ItemStat(
                        formula: count => StackFormula(count, 0.05f),
                        statText: "Attack Speed Decrease",
                        formatter: new PercentageFormatter(color: ColorCatalog.ColorIndex.Tier2Item.ToHex())
                        ),
                    new ItemStat(
                        formula: count => StackFormula(count, 0.05f),
                        statText: "Max Health Decrease",
                        formatter: new PercentageFormatter(color: ColorCatalog.ColorIndex.Tier2Item.ToHex())
                        ),
                    new ItemStat(
                        formula: count => StackFormula(count, 0.05f),
                        statText: "Movement Speed Decrease",
                        formatter: new PercentageFormatter(color: ColorCatalog.ColorIndex.Tier2Item.ToHex())
                        ),*/
                },
                #endregion
            };
	    }
    }
}
