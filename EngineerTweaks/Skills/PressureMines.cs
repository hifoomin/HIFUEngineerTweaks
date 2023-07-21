using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HIFUEngineerTweaks.Skills
{
    public class PressureMines : TweakBase
    {
        public static float damageScale;
        public static float aoeScale;
        public static int charges;
        public static float cooldown;

        public override string Name => ": Secondary : Pressure Mines";

        public override string SkillToken => "secondary";

        public override string DescText => "Place a two-stage mine that deals <style=cIsDamage>300% damage</style>, or <style=cIsDamage>" + Mathf.Round(300f * damageScale) + "% damage</style> if fully armed. Can hold up to " + charges + ".";

        public override void Init()
        {
            damageScale = ConfigOption(3f + 2f / 3f, "Damage Scale", "Decimal. Vanilla is 3");
            aoeScale = ConfigOption(2f, "Area of Effect Scale", "Vanilla is 2");
            charges = ConfigOption(2, "Charges", "Vanilla is 4");
            cooldown = ConfigOption(4.5f, "Cooldown", "Vanilla is 8");
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
            On.RoR2.CharacterMaster.GetDeployableSameSlotLimit += CharacterMaster_GetDeployableSameSlotLimit;
            On.EntityStates.Engi.Mine.BaseMineArmingState.OnEnter += BaseMineArmingState_OnEnter;
            On.EntityStates.Engi.Mine.MineArmingWeak.FixedUpdate += MineArmingWeak_FixedUpdate;
        }

        private void MineArmingWeak_FixedUpdate(On.EntityStates.Engi.Mine.MineArmingWeak.orig_FixedUpdate orig, EntityStates.Engi.Mine.MineArmingWeak self)
        {
            EntityStates.Engi.Mine.MineArmingWeak.duration = 1.5f;
            orig(self);
        }

        private void BaseMineArmingState_OnEnter(On.EntityStates.Engi.Mine.BaseMineArmingState.orig_OnEnter orig, EntityStates.Engi.Mine.BaseMineArmingState self)
        {
            if (self is EntityStates.Engi.Mine.MineArmingFull)
            {
                self.damageScale = damageScale;
                self.blastRadiusScale = aoeScale;
                self.forceScale = 6f;
            }
            orig(self);
        }

        private int CharacterMaster_GetDeployableSameSlotLimit(On.RoR2.CharacterMaster.orig_GetDeployableSameSlotLimit orig, CharacterMaster self, DeployableSlot slot)
        {
            if (slot is DeployableSlot.EngiMine)
            {
                return 8 + self.GetBody().inventory.GetItemCount(RoR2Content.Items.SecondarySkillMagazine);
            }
            else
            {
                return orig(self, slot);
            }
        }

        private void Changes()
        {
            var pressure = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Engi/EngiBodyPlaceMine.asset").WaitForCompletion();
            pressure.baseMaxStock = charges;
            pressure.baseRechargeInterval = cooldown;
        }
    }
}