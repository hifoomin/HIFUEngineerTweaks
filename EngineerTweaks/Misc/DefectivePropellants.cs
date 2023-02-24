using R2API;
using RoR2;
using RoR2.Skills;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HET.Misc
{
    public class DefectivePropellants : MiscBase
    {
        public static float BaseDamage;
        public static float BaseJumpPower;
        public static float BaseMoveSpeed;
        public static SkillDef defectivePropellants;
        public override string Name => ":: Misc : Base Stats";

        public override void Init()
        {
            BaseDamage = ConfigOption(12f, "Base Damage", "Decimal. Vanilla is 14");
            BaseJumpPower = ConfigOption(20f, "Base Jump Power", "Vanilla is 15");
            BaseMoveSpeed = ConfigOption(7.5f, "Base Move Speed", "Vanilla is 7");
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
        }

        public static void Changes()
        {
            var engiBody = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Engi/EngiBody.prefab").WaitForCompletion().GetComponent<CharacterBody>();
            engiBody.baseDamage = BaseDamage;

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

            engiBody.baseJumpPower = BaseJumpPower;
            engiBody.baseMoveSpeed = BaseMoveSpeed;
            // for passive, though i wouldnt know how to check for it, maybe just genericskill checking jank + recalcstats?
        }
    }
}