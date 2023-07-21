using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HIFUEngineerTweaks.Misc
{
    public class BaseStats : MiscBase
    {
        public static float baseDamage;
        public static float baseJumpPower;
        public static float baseMoveSpeed;
        public static SkillDef defectivePropellants;
        public override string Name => ":: Misc : Base Stats";

        public override void Init()
        {
            baseDamage = ConfigOption(12f, "Base Damage", "Decimal. Vanilla is 14");
            baseJumpPower = ConfigOption(20f, "Base Jump Power", "Vanilla is 15");
            baseMoveSpeed = ConfigOption(7.5f, "Base Move Speed", "Vanilla is 7");
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
        }

        public static void Changes()
        {
            var engiBody = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Engi/EngiBody.prefab").WaitForCompletion().GetComponent<CharacterBody>();
            engiBody.baseDamage = baseDamage;
            /*
            defectivePropellants = ScriptableObject.CreateInstance<SkillDef>();
            defectivePropellants.skillNameToken = "HET_ENGI_PASSIVE_NAME";
            (defectivePropellants as ScriptableObject).name = "DefectivePropellantsPassive";
            defectivePropellants.skillDescriptionToken = "HET_ENGI_DESCRIPTION";
            defectivePropellants.icon = null;
            defectivePropellants.keywordTokens = null;
            defectivePropellants.baseRechargeInterval = 0;
            defectivePropellants.activationState = new(typeof(EntityStates.GenericCharacterMain));
            defectivePropellants.activationStateMachineName = "Body";
            // idk copied from randomlyawesome

            ContentAddition.AddSkillDef(defectivePropellants);

            for (int i = 0; i < engiBody.skillLocator.allSkills.Length; i++)
            {
                var skillFamily = engiBody.skillLocator.allSkills[i].skillFamily;
                if (skillFamily.name.ToLower().Contains("passive"))
                {
                    Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
                    skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant
                    {
                        skillDef = defectivePropellants,
                        unlockableName = "",
                        viewableNode = new ViewablesCatalog.Node(defectivePropellants.skillNameToken, false, null)
                    };
                }
            }
            // idk copied from randomlyawesome
            */
            engiBody.baseJumpPower = baseJumpPower;
            engiBody.baseMoveSpeed = baseMoveSpeed;
            // for passive, though i wouldnt know how to check for it, maybe just genericskill checking jank + recalcstats?
        }
    }
}