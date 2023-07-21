using RoR2;
using RoR2.CharacterAI;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HIFUEngineerTweaks.Skills
{
    public class TR58CarbonizerTurret : TweakBase
    {
        public override string Name => ": Special :: TR58 Carbonizer Turret";

        public override string SkillToken => "special_alt1";

        public override string DescText => "Place a <style=cIsUtility>mobile</style> turret that <style=cIsUtility>inherits all your items.</style> Fires a laser for <style=cIsDamage>200% damage per second</style> that <style=cIsUtility>slows enemies</style>. Can place up to 2.";

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
            var turret = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Engi/EngiWalkerTurretMaster.prefab").WaitForCompletion();
            Component[] array = turret.GetComponents<AISkillDriver>();

            AISkillDriver AI = (from x in turret.GetComponents<AISkillDriver>()
                                where x.customName == "ReturnToLeader"
                                select x).First();
            AI.shouldSprint = true;
            AI.minDistance = 35f;

            var walking = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Engi/EngiWalkerTurretBody.prefab").WaitForCompletion().GetComponent<CharacterBody>();
            walking.baseDamage = 14f;
            walking.baseMoveSpeed = 11f;
            walking.baseMaxHealth = 100f;
        }
    }
}