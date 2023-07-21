using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HIFUEngineerTweaks.Skills
{
    public class BouncingGrenades : TweakBase
    {
        public static float damage;
        public static int maximumGrenadesCount;
        public static float chargeTime;
        public static float aoe;

        public override string Name => ": Primary : Bouncing Grenades";

        public override string SkillToken => "primary";

        public override string DescText => "Fire <style=cIsDamage>" + maximumGrenadesCount + "</style> grenades that deal <style=cIsDamage>" + d(damage) + " damage</style> each.";

        public override void Init()
        {
            damage = ConfigOption(1.6f, "Damage", "Decimal. Vanilla is 1");
            maximumGrenadesCount = ConfigOption(3, "Grenade Count", "Vanilla is 8");
            aoe = ConfigOption(4.5f, "Area of Effect", "Vanilla is 3.5");
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
            On.EntityStates.Engi.EngiWeapon.ChargeGrenades.OnEnter += ChargeGrenades_OnEnter;
            On.EntityStates.Engi.EngiWeapon.FireGrenades.OnEnter += FireGrenades_OnEnter;
        }

        private void FireGrenades_OnEnter(On.EntityStates.Engi.EngiWeapon.FireGrenades.orig_OnEnter orig, EntityStates.Engi.EngiWeapon.FireGrenades self)
        {
            EntityStates.Engi.EngiWeapon.FireGrenades.damageCoefficient = damage;
            EntityStates.Engi.EngiWeapon.FireGrenades.fireDuration = 0.2f;
            EntityStates.Engi.EngiWeapon.FireGrenades.baseDuration = 1f;
            orig(self);
        }

        private void ChargeGrenades_OnEnter(On.EntityStates.Engi.EngiWeapon.ChargeGrenades.orig_OnEnter orig, EntityStates.Engi.EngiWeapon.ChargeGrenades self)
        {
            EntityStates.Engi.EngiWeapon.ChargeGrenades.baseMaxChargeTime = 0f;
            EntityStates.Engi.EngiWeapon.ChargeGrenades.baseTotalDuration = 0f;
            EntityStates.Engi.EngiWeapon.ChargeGrenades.maxCharges = 0;
            EntityStates.Engi.EngiWeapon.ChargeGrenades.maxGrenadeCount = maximumGrenadesCount;
            EntityStates.Engi.EngiWeapon.ChargeGrenades.minGrenadeCount = maximumGrenadesCount;
            orig(self);
        }

        private void Changes()
        {
            var grenade = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Engi/EngiGrenadeProjectile.prefab").WaitForCompletion();
            var projectileImpactExplosion = grenade.GetComponent<ProjectileImpactExplosion>();
            projectileImpactExplosion.blastRadius = aoe;
        }
    }
}