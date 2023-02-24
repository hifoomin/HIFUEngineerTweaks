using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HET.Skills
{
    public class TR12GaussAutoTurret : TweakBase
    {
        public static float Damage;
        public static int Charges;
        public static float Cooldown;

        public override string Name => ": Special : TR12 Gauss Auto Turret";

        public override string SkillToken => "special";

        public override string DescText => "Place a turret that <style=cIsUtility>inherits all your items.</style> Fires a cannon for <style=cIsDamage>100% damage</style>. Can place up to 2.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
        }

        public static void Changes()
        {
            var Stationary = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Engi/EngiTurretBody.prefab").WaitForCompletion().GetComponent<CharacterBody>();
            Stationary.baseDamage = 14f;

            Stationary.baseMaxHealth = 150f;
        }
    }
}