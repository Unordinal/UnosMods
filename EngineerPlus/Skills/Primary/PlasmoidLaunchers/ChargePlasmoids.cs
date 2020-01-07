using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using RoR2;
using EntityStates;
using EngineerPlus;
using RoR2.Projectile;
using RoR2.Skills;
using EntityStates.Engi.EngiWeapon;

namespace EngineerPlus.Skills.Primary.PlasmoidLaunchers
{
    public class ChargePlasmoids : BaseState
    {
        private float stopwatch;
        private float chargeDuration;
        private float windDownDuration;
        private bool fireFromAlt;
        private bool hasFired;
        private GameObject muzzleChargeEffect;
        private uint soundID;

        public static float BaseChargeDuration { get; set; } = 1f;
        public static float BaseWindDownDuration { get; set; } = 0.05f;
        public static float DamageCoefficient { get; set; } = 3.5f;
        public static float RecoilAmplitude { get; set; } = 1.2f;
        public static float Force { get; set; } = 500f;
        public static float SelfForce { get; set; } = 500f;
        public static string ChargeSoundString { get; set; } = ChargeGrenades.chargeStockSoundString;
        public static string FireSoundString { get; set; } = EntityStates.EngiTurret.EngiTurretWeapon.FireGauss.attackSoundString;
        public static GameObject ChargeEffectPrefab { get; set; } = ChargeGrenades.chargeEffectPrefab;
        public static GameObject MuzzleFlashEffectPrefab { get; set; } = FireGrenades.effectPrefab;
        public static GameObject ProjectilePrefab { get; set; } // = Resources.Load<GameObject>("Prefabs/Projectiles/PaladinRocket");

        static ChargePlasmoids()
        {
            var plasmoidProjectile = ProjectileCatalog.GetProjectilePrefab(69);
            //var plasmoidProjectile = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Projectiles/MageIceBolt")); // MageIceBolt
            var projDamage = plasmoidProjectile.GetComponent<ProjectileDamage>();
            var projSimple = plasmoidProjectile.GetComponent<ProjectileSimple>();
            var projController = plasmoidProjectile.GetComponent<ProjectileController>();
            var projCollider = plasmoidProjectile.GetComponent<SphereCollider>();

            projDamage.damage = DamageCoefficient;
            projDamage.damageType = DamageType.Generic;
            projSimple.velocity = 120f;
            projSimple.lifetime = 3f;
            projController.ghostPrefab = Resources.Load<GameObject>("Prefabs/ProjectileGhosts/MageIceBoltGhost");
            projController.ghostPrefab.transform.localScale = new Vector3(1.25f, 1.25f, 1.25f);
            projController.procCoefficient = 0.9f;
            projCollider.radius = 0.75f;

            ProjectilePrefab = plasmoidProjectile;
        }

        public override void OnEnter()
        {
            PluginEntry.Logger.LogWarning($"{nameof(ChargePlasmoids)}: {nameof(OnEnter)}");
            base.OnEnter();

            stopwatch = 0f;
            chargeDuration = BaseChargeDuration / attackSpeedStat;
            windDownDuration = BaseWindDownDuration / attackSpeedStat;
            soundID = Util.PlayScaledSound(ChargeSoundString, base.gameObject, attackSpeedStat);

            base.characterBody.SetAimTimer(chargeDuration + windDownDuration + 1f);

            ChildLocator childLocator = base.GetModelAnimator()?.GetComponent<ChildLocator>();
            if (childLocator)
            {
                string muzzleString = fireFromAlt ? "MuzzleRight" : "MuzzleLeft";
                Transform muzzleTransform = childLocator.FindChild(muzzleString);

                if (muzzleTransform && ChargeEffectPrefab)
                {
                    muzzleChargeEffect = UnityEngine.Object.Instantiate(ChargeEffectPrefab, muzzleTransform.position, muzzleTransform.rotation);
                    muzzleChargeEffect.transform.parent = muzzleTransform;

                    var sclPrtclSysDur = muzzleChargeEffect.GetComponent<ScaleParticleSystemDuration>();
                    var objScaleCurve = muzzleChargeEffect.GetComponent<ObjectScaleCurve>();

                    if (sclPrtclSysDur)
                        sclPrtclSysDur.newDuration = chargeDuration;

                    if (objScaleCurve)
                        objScaleCurve.timeMax = chargeDuration;
                }
            }
        }

        public override void OnExit()
        {
            PluginEntry.Logger.LogWarning($"{nameof(ChargePlasmoids)}: {nameof(OnExit)}");

            if (!outer.destroying && !hasFired)
                base.PlayAnimation("Gesture, Additive", "Empty");

            EntityState.Destroy(muzzleChargeEffect);
            AkSoundEngine.StopPlayingID(soundID);
            outer.SetNextStateToMain();
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            base.StartAimMode();

            stopwatch += Time.fixedDeltaTime;

            if (base.isAuthority)
            {
                if (!hasFired && stopwatch >= chargeDuration)
                {
                    FirePlasmoid();
                }
                if (hasFired && stopwatch >= windDownDuration)
                {
                    if (!base.inputBank.skill1.down)
                    {
                        outer.SetNextStateToMain();
                    }
                    else
                    {
                        hasFired = false;
                    }
                    return;
                }
            }
        }

        private void FirePlasmoid()
        {
            hasFired = true;
            string muzzleString = fireFromAlt ? "MuzzleRight" : "MuzzleLeft";
            string muzzleSideString = fireFromAlt ? "Right" : "Left";

            Util.PlayScaledSound(FireSoundString, base.gameObject, attackSpeedStat);
            base.PlayCrossfade($"Gesture {muzzleSideString} Cannon, Additive", $"FireGrenade{muzzleSideString}", 0.1f);

            Ray projectileRay = base.GetAimRay();
            ChildLocator childLocator = base.GetModelTransform()?.GetComponent<ChildLocator>();
            if (childLocator)
            {
                Transform muzzleTransform = childLocator.FindChild(muzzleString);
                if (muzzleTransform)
                    projectileRay.origin = muzzleTransform.position;
            }

            EntityState.Destroy(muzzleChargeEffect);

            if (MuzzleFlashEffectPrefab)
            {
                EffectManager.SimpleMuzzleFlash(MuzzleFlashEffectPrefab, base.gameObject, muzzleString, false);
            }

            if (base.isAuthority)
            {
                if (ProjectilePrefab)
                {
                    ProjectileManager.instance.FireProjectile(
                        ProjectilePrefab,
                        projectileRay.origin,
                        Util.QuaternionSafeLookRotation(projectileRay.direction),
                        base.gameObject,
                        damageStat * DamageCoefficient,
                        Force,
                        Util.CheckRoll(critStat, base.characterBody.master)
                        );
                }

                base.characterMotor?.ApplyForce(projectileRay.direction * -SelfForce);
            }

            stopwatch = 0f;
            fireFromAlt = !fireFromAlt;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
