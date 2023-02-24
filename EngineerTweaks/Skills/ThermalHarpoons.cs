using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HET.Skills
{
    public class ThermalHarpoons : TweakBase
    {
        public static float Damage;
        public static float Cooldown;
        public static int Charges;
        public static int ChargesToRecharge;
        public static float Range;

        public override string Name => ": Utility :: Thermal Harpoons";

        public override string SkillToken => "skill_harpoon";

        public override string DescText => "Enter <style=cIsUtility>target painting mode</style> to launch heat-seeking harpoon missiles that deal <style=cIsDamage>" + d(Damage) + " damage</style> each. Can store up to " + Charges + ".";

        public override void Init()
        {
            Damage = ConfigOption(5f, "Damage", "Decimal. Vanilla is 5");
            Cooldown = ConfigOption(2.5f, "Cooldown", "Vanilla is 2.5");
            Charges = ConfigOption(4, "Charges", "Vanilla is 4");
            ChargesToRecharge = ConfigOption(1, "Charges to Recharge", "Vanilla is 1");
            Range = ConfigOption(6969, "Range", "Vanilla is 150");
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
        }

        public static void Changes()
        {
            var Harpoon = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Engi/EngiHarpoons.asset").WaitForCompletion();
            Harpoon.rechargeStock = ChargesToRecharge;
            Harpoon.baseMaxStock = Charges;
            Harpoon.baseRechargeInterval = Cooldown;
            On.EntityStates.Engi.EngiMissilePainter.Fire.OnEnter += (orig, self) =>
            {
                EntityStates.Engi.EngiMissilePainter.Fire.damageCoefficient = Damage;
                orig(self);
            };
            On.EntityStates.Engi.EngiMissilePainter.Paint.OnEnter += (orig, self) =>
            {
                EntityStates.Engi.EngiMissilePainter.Paint.maxDistance = Range;
                orig(self);
            };
        }
    }
}