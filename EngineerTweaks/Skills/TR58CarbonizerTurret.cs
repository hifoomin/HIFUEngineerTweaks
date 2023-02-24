using RoR2;
using RoR2.CharacterAI;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HET.Skills
{
    public class TR58CarbonizerTurret : TweakBase
    {
        public static float Damage;
        public static int Charges;
        public static float Cooldown;

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
            var Turret = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Engi/EngiWalkerTurretMaster.prefab").WaitForCompletion();
            Component[] array = Turret.GetComponents<AISkillDriver>();

            AISkillDriver AI = (from x in Turret.GetComponents<AISkillDriver>()
                                where x.customName == "ReturnToLeader"
                                select x).First();
            AI.shouldSprint = true;
            AI.minDistance = 35f;

            var Walking = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Engi/EngiWalkerTurretBody.prefab").WaitForCompletion().GetComponent<CharacterBody>();
            Walking.baseDamage = 14f;
            Walking.baseMoveSpeed = 11f;
            Walking.baseMaxHealth = 100f;
        }
    }
}