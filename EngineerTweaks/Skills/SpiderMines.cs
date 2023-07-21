using RoR2;
using RoR2.Skills;
using UnityEngine.AddressableAssets;

namespace HIFUEngineerTweaks.Skills
{
    public class SpiderMines : TweakBase
    {
        public static float damage;
        public static int charges;
        public static float cooldown;

        public override string Name => ": Secondary :: Spider Mines";

        public override string SkillToken => "spidermine";

        public override string DescText => "Place a robot mine that deals <style=cIsDamage>" + d(damage) + " damage</style> when an enemy walks nearby. Can hold up to " + charges + ".";

        public override void Init()
        {
            damage = ConfigOption(7f, "Damage", "Decimal. Vanilla is 6");
            charges = ConfigOption(2, "Charges", "Vanilla is 4");
            cooldown = ConfigOption(4f, "Cooldown", "Vanilla is 8");
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
            On.RoR2.CharacterMaster.GetDeployableSameSlotLimit += CharacterMaster_GetDeployableSameSlotLimit;
            On.EntityStates.Engi.EngiWeapon.FireMines.OnEnter += FireMines_OnEnter;
        }

        private void FireMines_OnEnter(On.EntityStates.Engi.EngiWeapon.FireMines.orig_OnEnter orig, EntityStates.Engi.EngiWeapon.FireMines self)
        {
            if (self is EntityStates.Engi.EngiWeapon.FireSpiderMine)
            {
                self.damageCoefficient = damage;
            }
            orig(self);
        }

        private int CharacterMaster_GetDeployableSameSlotLimit(On.RoR2.CharacterMaster.orig_GetDeployableSameSlotLimit orig, CharacterMaster self, DeployableSlot slot)
        {
            if (slot is DeployableSlot.EngiSpiderMine)
            {
                return 8 + self.inventory.GetItemCount(RoR2Content.Items.SecondarySkillMagazine);
            }
            else
            {
                return orig(self, slot);
            }
        }

        private void Changes()
        {
            var spider = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Engi/EngiBodyPlaceSpiderMine.asset").WaitForCompletion();
            spider.baseMaxStock = charges;
            spider.baseRechargeInterval = cooldown;
        }
    }
}