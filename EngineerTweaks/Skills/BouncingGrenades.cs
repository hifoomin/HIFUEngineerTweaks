namespace HET.Skills
{
    public class BouncingGrenades : TweakBase
    {
        public static float Damage;
        public static int MaximumGrenadesCount;
        public static float ChargeTime;
        public static float AoE;

        public override string Name => ": Primary : Bouncing Grenades";

        public override string SkillToken => "primary";

        public override string DescText => "Fire <style=cIsDamage>" + MaximumGrenadesCount + "</style> grenades that deal <style=cIsDamage>" + d(Damage) + " damage</style> each.";

        public override void Init()
        {
            Damage = ConfigOption(1.6f, "Damage", "Decimal. Vanilla is 1");
            MaximumGrenadesCount = ConfigOption(3, "Grenade Count", "Vanilla is 8");
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
        }

        public static void Changes()
        {
            On.EntityStates.Engi.EngiWeapon.ChargeGrenades.OnEnter += (orig, self) =>
            {
                EntityStates.Engi.EngiWeapon.ChargeGrenades.baseMaxChargeTime = 0f;
                EntityStates.Engi.EngiWeapon.ChargeGrenades.baseTotalDuration = 0f;
                EntityStates.Engi.EngiWeapon.ChargeGrenades.maxCharges = 0;
                EntityStates.Engi.EngiWeapon.ChargeGrenades.maxGrenadeCount = MaximumGrenadesCount;
                EntityStates.Engi.EngiWeapon.ChargeGrenades.minGrenadeCount = MaximumGrenadesCount;
                orig(self);
            };
            On.EntityStates.Engi.EngiWeapon.FireGrenades.OnEnter += (orig, self) =>
            {
                EntityStates.Engi.EngiWeapon.FireGrenades.damageCoefficient = Damage;
                EntityStates.Engi.EngiWeapon.FireGrenades.fireDuration = 0.2f;
                EntityStates.Engi.EngiWeapon.FireGrenades.baseDuration = 1f;
                orig(self);
            };
        }
    }
}