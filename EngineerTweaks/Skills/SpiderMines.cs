using RoR2;
using RoR2.Skills;
using UnityEngine.AddressableAssets;

namespace HET.Skills
{
    public class SpiderMines : TweakBase
    {
        public static float Damage;
        public static int Charges;
        public static float Cooldown;

        public override string Name => ": Secondary :: Spider Mines";

        public override string SkillToken => "spidermine";

        public override string DescText => "Place a robot mine that deals <style=cIsDamage>" + d(Damage) + " damage</style> when an enemy walks nearby. Can hold up to " + Charges + ".";

        public override void Init()
        {
            Damage = ConfigOption(7f, "Damage", "Decimal. Vanilla is 6");
            Charges = ConfigOption(2, "Charges", "Vanilla is 4");
            Cooldown = ConfigOption(4f, "Cooldown", "Vanilla is 8");
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
        }

        public static void Changes()
        {
            var Spider = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Engi/EngiBodyPlaceSpiderMine.asset").WaitForCompletion();
            Spider.baseMaxStock = Charges;
            Spider.baseRechargeInterval = Cooldown;
            On.EntityStates.Engi.EngiWeapon.FireMines.OnEnter += (orig, self) =>
            {
                if (self is EntityStates.Engi.EngiWeapon.FireSpiderMine)
                {
                    self.damageCoefficient = Damage;
                }
                orig(self);
            };
            On.RoR2.CharacterMaster.GetDeployableSameSlotLimit += (On.RoR2.CharacterMaster.orig_GetDeployableSameSlotLimit orig, CharacterMaster self, DeployableSlot slot) =>
            {
                if (slot is DeployableSlot.EngiSpiderMine)
                {
                    return 8 + self.inventory.GetItemCount(RoR2Content.Items.SecondarySkillMagazine);
                }
                else
                {
                    return orig(self, slot);
                }
            };
        }
    }
}