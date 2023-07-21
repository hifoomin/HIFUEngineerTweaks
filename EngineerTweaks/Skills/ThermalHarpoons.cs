using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HIFUEngineerTweaks.Skills
{
    public class ThermalHarpoons : TweakBase
    {
        public static float damage;
        public static float cooldown;
        public static int charges;
        public static int chargesToRecharge;
        public static float range;

        public override string Name => ": Utility :: Thermal Harpoons";

        public override string SkillToken => "skill_harpoon";

        public override string DescText => "Enter <style=cIsUtility>target painting mode</style> to launch heat-seeking harpoon missiles that deal <style=cIsDamage>" + d(damage) + " damage</style> each. Can store up to " + charges + ".";

        public override void Init()
        {
            damage = ConfigOption(5f, "Damage", "Decimal. Vanilla is 5");
            cooldown = ConfigOption(2.5f, "Cooldown", "Vanilla is 2.5");
            charges = ConfigOption(4, "Charges", "Vanilla is 4");
            chargesToRecharge = ConfigOption(1, "Charges to Recharge", "Vanilla is 1");
            range = ConfigOption(6969, "Range", "Vanilla is 150");
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
            On.EntityStates.Engi.EngiMissilePainter.Fire.OnEnter += Fire_OnEnter;
            On.EntityStates.Engi.EngiMissilePainter.Paint.OnEnter += Paint_OnEnter;
        }

        private void Paint_OnEnter(On.EntityStates.Engi.EngiMissilePainter.Paint.orig_OnEnter orig, EntityStates.Engi.EngiMissilePainter.Paint self)
        {
            EntityStates.Engi.EngiMissilePainter.Paint.maxDistance = range;
            orig(self);
        }

        private void Fire_OnEnter(On.EntityStates.Engi.EngiMissilePainter.Fire.orig_OnEnter orig, EntityStates.Engi.EngiMissilePainter.Fire self)
        {
            EntityStates.Engi.EngiMissilePainter.Fire.damageCoefficient = damage;
            orig(self);
        }

        private void Changes()
        {
            var harpoon = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Engi/EngiHarpoons.asset").WaitForCompletion();
            harpoon.rechargeStock = chargesToRecharge;
            harpoon.baseMaxStock = charges;
            harpoon.baseRechargeInterval = cooldown;
        }
    }
}