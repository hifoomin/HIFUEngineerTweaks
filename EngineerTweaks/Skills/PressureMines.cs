using RoR2;
using RoR2.Skills;
using UnityEngine.AddressableAssets;

namespace HET.Skills
{
    public class PressureMines : TweakBase
    {
        public static float DamageScale;
        public static float AoEScale;
        public static int Charges;
        public static float Cooldown;

        public override string Name => ": Secondary : Pressure Mines";

        public override string SkillToken => "secondary";

        public override string DescText => "Place a two-stage mine that deals <style=cIsDamage>300% damage</style>, or <style=cIsDamage>" + d(3f * DamageScale) + " damage</style> if fully armed. Can hold up to " + Charges + ".";

        public override void Init()
        {
            DamageScale = ConfigOption(3.5f, "Damage Scale", "Decimal. Vanilla is 3");
            AoEScale = ConfigOption(2f, "Area of Effect Scale", "Vanilla is 2");
            Charges = ConfigOption(2, "Charges", "Vanilla is 4");
            Cooldown = ConfigOption(4.5f, "Cooldown", "Vanilla is 8");
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
        }

        public static void Changes()
        {
            var Pressure = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Engi/EngiBodyPlaceMine.asset").WaitForCompletion();
            Pressure.baseMaxStock = Charges;
            Pressure.baseRechargeInterval = Cooldown;
            On.EntityStates.Engi.Mine.MineArmingWeak.FixedUpdate += (orig, self) =>
            {
                EntityStates.Engi.Mine.MineArmingWeak.duration = 1.5f;
                orig(self);
            };
            On.EntityStates.Engi.Mine.BaseMineArmingState.OnEnter += (orig, self) =>
            {
                if (self is EntityStates.Engi.Mine.MineArmingFull)
                {
                    self.damageScale = DamageScale;
                    self.blastRadiusScale = AoEScale;
                    self.forceScale = 6f;
                }
                orig(self);
            };
            On.RoR2.CharacterMaster.GetDeployableSameSlotLimit += (On.RoR2.CharacterMaster.orig_GetDeployableSameSlotLimit orig, CharacterMaster self, DeployableSlot slot) =>
            {
                if (slot is DeployableSlot.EngiMine)
                {
                    return 8 + self.GetBody().inventory.GetItemCount(RoR2Content.Items.SecondarySkillMagazine);
                }
                else
                {
                    return orig(self, slot);
                }
            };
        }
    }
}