using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HIFUEngineerTweaks.Skills
{
    public class BubbleShield : TweakBase
    {
        public static float damage;
        public static float cooldown;
        public static float duration;
        public static float aoe;
        public static float ticks;
        public static float procCoefficient;
        public static bool changeShape;
        public static float size;
        public static int charges;
        public static int chargesToConsume;
        public static int chargesToRecharge;

        public override string Name => ": Utility : Bubble Shield";

        public override string SkillToken => "utility";

        public override string DescText => "Place an <style=cIsUtility>impenetrable shield</style> that blocks all incoming damage" +
                                           (damage > 0 ? " and deals <style=cIsDamage>" + d(damage / ticks) + "</style> damage per second to nearby enemies." : ".");

        public override void Init()
        {
            damage = ConfigOption(0.1f, "Damage per Tick", "Decimal. Vanilla is 0");
            cooldown = ConfigOption(9f, "Cooldown", "Vanilla is 25");
            duration = ConfigOption(8f, "Duration", "Vanilla is 15");
            aoe = ConfigOption(30f, "Damage Area of Effect", "Default is 30");
            ticks = ConfigOption(0.2f, "Time per Tick", "Default is 1");
            procCoefficient = ConfigOption(0f, "Proc Coefficient", "Default is 0");
            size = ConfigOption(35f, "Size", "Vanilla is 20, for 10m Radius\nKnockback area is equal to this as well");
            charges = ConfigOption(2, "Charges", "Vanilla is 1");
            chargesToConsume = ConfigOption(2, "Charges Required", "Vanilla is 1");
            chargesToRecharge = ConfigOption(1, "Charges to Recharge", "Vanilla is 1\nThis is so Bandolier and Hardlight Afterburner aren't as cracked");

            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.CharacterMaster.GetDeployableSameSlotLimit += CharacterMaster_GetDeployableSameSlotLimit;
            On.EntityStates.Engi.EngiBubbleShield.Deployed.OnEnter += Deployed_OnEnter;
            Changes();
        }

        private void Deployed_OnEnter(On.EntityStates.Engi.EngiBubbleShield.Deployed.orig_OnEnter orig, EntityStates.Engi.EngiBubbleShield.Deployed self)
        {
            EntityStates.Engi.EngiBubbleShield.Deployed.lifetime = duration;
            orig(self);
            ChildLocator component = self.GetComponent<ChildLocator>();
            if (component)
            {
                component.FindChild(EntityStates.Engi.EngiBubbleShield.Deployed.childLocatorString).gameObject.transform.localScale = new Vector3(size, size, size);
            }
        }

        private int CharacterMaster_GetDeployableSameSlotLimit(On.RoR2.CharacterMaster.orig_GetDeployableSameSlotLimit orig, CharacterMaster self, DeployableSlot slot)
        {
            if (slot is DeployableSlot.EngiBubbleShield)
            {
                return 2 + self.inventory.GetItemCount(RoR2Content.Items.UtilitySkillMagazine);
            }
            else
            {
                return orig(self, slot);
            }
        }

        private void Changes()
        {
            var shield = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Engi/EngiBodyPlaceBubbleShield.asset").WaitForCompletion();
            shield.baseRechargeInterval = cooldown;
            shield.baseMaxStock = charges;
            shield.stockToConsume = chargesToConsume;
            shield.rechargeStock = chargesToRecharge;
            shield.requiredStock = chargesToConsume;

            float timer = 0f;

            On.EntityStates.Engi.EngiBubbleShield.Deployed.FixedUpdate += (orig, self) =>
            {
                orig(self);
                timer += Time.fixedDeltaTime;
                if (timer > ticks)
                {
                    if (self != null && self.gameObject != null && self.gameObject.GetComponent<Deployable>() != null && self.gameObject.GetComponent<Deployable>().ownerMaster != null && self.gameObject.GetComponent<Deployable>().ownerMaster.GetBodyObject() != null && self.gameObject.GetComponent<Deployable>().ownerMaster.GetBody() != null)
                    {
                        var owner = self.gameObject.GetComponent<Deployable>().ownerMaster;

                        if (self.isAuthority)
                        {
                            new BlastAttack
                            {
                                attacker = owner.GetBodyObject(),
                                baseDamage = owner.GetBody().damage * damage,
                                baseForce = 0f,
                                crit = owner.GetBody().RollCrit(),
                                damageType = DamageType.SlowOnHit,
                                procCoefficient = procCoefficient,
                                radius = aoe,
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
                                radius = size / 2f,
                                position = self.transform.position,
                                attackerFiltering = AttackerFiltering.NeverHitSelf,
                                teamIndex = owner.GetBody().teamComponent.teamIndex,
                                falloffModel = BlastAttack.FalloffModel.None
                            }.Fire();
                            timer = 0f;
                        }
                    }
                }
            };
        }
    }
}