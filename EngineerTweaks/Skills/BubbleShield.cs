using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using RoR2.Projectile;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HET.Skills
{
    public class BubbleShield : TweakBase
    {
        public static float Damage;
        public static float Cooldown;
        public static float Duration;
        public static float AoE;
        public static float Ticks;
        public static float ProcCoefficient;
        public static bool ChangeShape;
        public static float Size;
        public static int Charges;
        public static int ChargesToConsume;
        public static int ChargesToRecharge;

        public override string Name => ": Utility : Bubble Shield";

        public override string SkillToken => "utility";

        public override string DescText => "Place an <style=cIsUtility>impenetrable shield</style> that blocks all incoming damage" +
                                           (Damage > 0 ? " and deals <style=cIsDamage>" + d(Damage / Ticks) + "</style> damage per second to nearby enemies." : ".");

        public override void Init()
        {
            Damage = ConfigOption(0.1f, "Damage per Tick", "Decimal. Vanilla is 0");
            Cooldown = ConfigOption(9f, "Cooldown", "Vanilla is 25");
            Duration = ConfigOption(8f, "Duration", "Vanilla is 15");
            AoE = ConfigOption(30f, "Damage Area of Effect", "Default is 30");
            Ticks = ConfigOption(0.2f, "Time per Tick", "Default is 1");
            ProcCoefficient = ConfigOption(0f, "Proc Coefficient", "Default is 0");
            Size = ConfigOption(35f, "Size", "Vanilla is 20, for 10m Radius\nKnockback area is equal to this as well");
            Charges = ConfigOption(2, "Charges", "Vanilla is 1");
            ChargesToConsume = ConfigOption(2, "Charges Required", "Vanilla is 1");
            ChargesToRecharge = ConfigOption(1, "Charges to Recharge", "Vanilla is 1\nThis is so Bandolier and Hardlight Afterburner aren't as cracked");

            base.Init();
        }

        public override void Hooks()
        {
            Changes();
        }

        public static void Changes()
        {
            var Shield = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Engi/EngiBodyPlaceBubbleShield.asset").WaitForCompletion();
            Shield.baseRechargeInterval = Cooldown;
            Shield.baseMaxStock = Charges;
            Shield.stockToConsume = ChargesToConsume;
            Shield.rechargeStock = ChargesToRecharge;
            Shield.requiredStock = ChargesToConsume;

            float timer = 0f;

            On.EntityStates.Engi.EngiBubbleShield.Deployed.OnEnter += (orig, self) =>
            {
                EntityStates.Engi.EngiBubbleShield.Deployed.lifetime = Duration;
                orig(self);
                ChildLocator component = self.GetComponent<ChildLocator>();
                if (component)
                {
                    component.FindChild(EntityStates.Engi.EngiBubbleShield.Deployed.childLocatorString).gameObject.transform.localScale = new Vector3(Size, Size, Size);
                }
            };

            On.EntityStates.Engi.EngiBubbleShield.Deployed.FixedUpdate += (orig, self) =>
            {
                orig(self);
                timer += Time.fixedDeltaTime;
                if (timer > Ticks)
                {
                    if (self != null && self.gameObject != null && self.gameObject.GetComponent<Deployable>() != null && self.gameObject.GetComponent<Deployable>().ownerMaster != null && self.gameObject.GetComponent<Deployable>().ownerMaster.GetBodyObject() != null && self.gameObject.GetComponent<Deployable>().ownerMaster.GetBody() != null)
                    {
                        var owner = self.gameObject.GetComponent<Deployable>().ownerMaster;

                        new BlastAttack
                        {
                            attacker = owner.GetBodyObject(),
                            baseDamage = owner.GetBody().damage * Damage,
                            baseForce = 0f,
                            crit = owner.GetBody().RollCrit(),
                            damageType = DamageType.SlowOnHit,
                            procCoefficient = ProcCoefficient,
                            radius = AoE,
                            falloffModel = BlastAttack.FalloffModel.None,
                            position = self.transform.position,
                            attackerFiltering = AttackerFiltering.NeverHitSelf,
                            teamIndex = owner.GetBody().teamComponent.teamIndex
                        }.Fire();
                        new BlastAttack
                        {
                            attacker = owner.GetBodyObject(),
                            baseDamage = 0f,
                            baseForce = 2500f,
                            crit = false,
                            damageType = DamageType.Stun1s,
                            procCoefficient = 0f,
                            radius = Size / 2f,
                            position = self.transform.position,
                            attackerFiltering = AttackerFiltering.NeverHitSelf,
                            teamIndex = owner.GetBody().teamComponent.teamIndex,
                            falloffModel = BlastAttack.FalloffModel.None
                        }.Fire();
                        timer = 0f;
                    }
                }
            };

            On.RoR2.CharacterMaster.GetDeployableSameSlotLimit += (On.RoR2.CharacterMaster.orig_GetDeployableSameSlotLimit orig, CharacterMaster self, DeployableSlot slot) =>
            {
                if (slot is DeployableSlot.EngiBubbleShield)
                {
                    return 2 + self.inventory.GetItemCount(RoR2Content.Items.UtilitySkillMagazine);
                }
                else
                {
                    return orig(self, slot);
                }
            };
        }
    }
}