using RoR2;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HIFUEngineerTweaks.Misc
{
    public class DetonateTurrets : MiscBase
    {
        public override string Name => ":: Misc ::: Detonate Turrets";
        public static float damage;
        public static float radius;
        public static float procCoefficient;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            var engi = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Engi/EngiBody.prefab").WaitForCompletion();
            engi.AddComponent<TurretController>();
        }
    }

    public class TurretController : MonoBehaviour
    {
        public CharacterMaster master;
        public CharacterBody body;
        public MinionOwnership minionOwnership;
        public InputBankTest inputBank;
        public float holdTimer = 0f;
        public bool shouldDestroy = false;

        public float holdThreshold = 1f;
        public MinionOwnership[] turrets = null;
        public float updateInterval = 0.3f;
        public float updateTimer = 0f;

        public void Start()
        {
            inputBank = GetComponent<InputBankTest>();
            body = inputBank.characterBody;
            master = body.master;
            minionOwnership = master.minionOwnership;
        }

        public void FixedUpdate()
        {
            updateTimer += Time.fixedDeltaTime;
            if (updateTimer >= updateInterval)
            {
                turrets = minionOwnership.group.members.Where(x => x.GetComponent<EngiTurretIdentifier>() != null).ToArray();
                updateTimer = 0f;
            }

            if (turrets.Length > 0 && shouldDestroy)
            {
                for (int i = 0; i < turrets.Length; i++)
                {
                    var turret = turrets[i];
                    var turretMaster = turret.GetComponent<CharacterMaster>();
                    var turretBody = turretMaster.GetBody();
                    if (turretBody)
                    {
                        var turretPosition = turretBody.corePosition;

                        if (Util.HasEffectiveAuthority(gameObject))
                        {
                            new BlastAttack()
                            {
                                attacker = gameObject,
                                attackerFiltering = AttackerFiltering.Default,
                                baseDamage = body.damage * DetonateTurrets.damage,
                                baseForce = 3000f,
                                bonusForce = new Vector3(0f, 1000f, 0f),
                                crit = body.RollCrit(),
                                damageType = DamageType.Generic,
                                falloffModel = BlastAttack.FalloffModel.None,
                                position = turretPosition,
                                procCoefficient = DetonateTurrets.procCoefficient,
                                radius = DetonateTurrets.radius,
                                teamIndex = TeamIndex.Neutral
                            }.Fire();
                        }

                        turretBody.healthComponent.Suicide();
                    }
                }
                shouldDestroy = false;
            }
        }

        public void Update()
        {
            holdTimer = 0f;
            if (inputBank.skill4.down)
            {
                holdTimer += Time.deltaTime;
                if (holdTimer >= holdThreshold)
                {
                    shouldDestroy = true;
                }
            }
        }
    }
}