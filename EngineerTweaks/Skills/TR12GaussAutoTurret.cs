using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HIFUEngineerTweaks.Skills
{
    public class TR12GaussAutoTurret : TweakBase
    {
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
            var stationary = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Engi/EngiTurretBody.prefab").WaitForCompletion().GetComponent<CharacterBody>();
            stationary.baseDamage = 14f;

            stationary.baseMaxHealth = 150f;
        }
    }
}