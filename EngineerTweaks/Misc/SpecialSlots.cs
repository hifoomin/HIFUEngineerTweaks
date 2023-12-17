using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

[assembly: HG.Reflection.SearchableAttribute.OptIn]

namespace HIFUEngineerTweaks.Misc
{
    public static class SpecialSlots
    {
        public static SkillFamily specialSlot1Family;
        public static SkillFamily specialSlot2Family;
        public static Dictionary<SkillDef, GameObject> skillToTurretMap = new();
        public static BodyIndex engiBodyIndex;
        public static GameObject engineer;

        public static void Init()
        {
            specialSlot2Family = ScriptableObject.CreateInstance<SkillFamily>();

            specialSlot1Family = Addressables.LoadAssetAsync<SkillFamily>("RoR2/Base/Engi/EngiBodySpecialFamily.asset").WaitForCompletion();

            engineer = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Engi/EngiBody.prefab").WaitForCompletion();
            engineer.AddComponent<EngiTurretController>();

            var specialSkill2 = engineer.AddComponent<GenericSkill>();
            specialSkill2._skillFamily = specialSlot1Family;
            specialSkill2.skillName = "PlaceTurret2";

            On.RoR2.Run.HandlePlayerFirstEntryAnimation += Run_HandlePlayerFirstEntryAnimation;
            On.EntityStates.Engi.EngiWeapon.PlaceTurret.FixedUpdate += PlaceTurret_FixedUpdate;
        }

        [SystemInitializer(typeof(SurvivorCatalog))]
        public static void InitDictAndBodyIndex()
        {
            engiBodyIndex = engineer.GetComponent<CharacterBody>().bodyIndex;

            var allTurrets = specialSlot1Family.variants;

            Main.HETLogger.LogDebug("all turrets have this many variants: " + allTurrets.Length);

            for (int i = 0; i < allTurrets.Length; i++)
            {
                try
                {
                    var skill = specialSlot1Family.variants[i].skillDef;
                    var master = ((EntityStates.Engi.EngiWeapon.PlaceTurret)Activator.CreateInstance(skill.activationState.stateType)).turretMasterPrefab;
                    Main.HETLogger.LogDebug("adding skill to map: " + skill);
                    Main.HETLogger.LogDebug("adding master to map: " + master);
                    skillToTurretMap.Add(skill, master);
                    if (master.GetComponent<EngiTurretIdentifier>() == null)
                    {
                        master.AddComponent<EngiTurretIdentifier>();
                    }
                }
                catch
                {
                    Main.HETLogger.LogError("Error! An Engineer skill mod doesn't fill skilldef or master data at the right time!");
                }
            }
        }

        private static void Run_HandlePlayerFirstEntryAnimation(On.RoR2.Run.orig_HandlePlayerFirstEntryAnimation orig, Run self, CharacterBody body, Vector3 spawnPosition, Quaternion spawnRotation)
        {
            if (body.bodyIndex == engiBodyIndex && body.GetComponent<EngiTurretController>())
            {
                var engiTurretController = body.GetComponent<EngiTurretController>();
                engiTurretController.special1 = body.GetComponents<GenericSkill>().Where(x => x.skillName == "PlaceTurret").First().skillDef;
                engiTurretController.special2 = body.GetComponents<GenericSkill>().Where(x => x.skillName == "PlaceTurret2").First().skillDef;
            }
            orig(self, body, spawnPosition, spawnRotation);
        }

        private static void PlaceTurret_FixedUpdate(On.EntityStates.Engi.EngiWeapon.PlaceTurret.orig_FixedUpdate orig, EntityStates.Engi.EngiWeapon.PlaceTurret self)
        {
            if (self.isAuthority)
            {
                self.entryCountdown -= Time.fixedDeltaTime;
                if (self.exitPending)
                {
                    self.exitCountdown -= Time.fixedDeltaTime;
                    if (self.exitCountdown <= 0f)
                    {
                        self.outer.SetNextStateToMain();

                        return;
                    }
                }
                else if (self.inputBank && self.entryCountdown <= 0f)
                {
                    if (self.inputBank.skill4.justPressed)
                    {
                        self.DestroyBlueprints();
                        self.exitPending = true;
                    }
                    if (self.inputBank.skill1.down && self.currentPlacementInfo.ok)
                    {
                        if (self.characterBody)
                        {
                            var engiTurretController = self.characterBody.GetComponent<EngiTurretController>();
                            // self.characterBody.SendConstructTurret(self.characterBody, self.currentPlacementInfo.position, self.currentPlacementInfo.rotation, MasterCatalog.FindMasterIndex(skillToTurretMap[engiTurretController.special1]));
                            self.characterBody.SendConstructTurret(self.characterBody, self.currentPlacementInfo.position, self.currentPlacementInfo.rotation, MasterCatalog.FindMasterIndex(self.turretMasterPrefab));
                            if (self.skillLocator)
                            {
                                GenericSkill skill = self.skillLocator.GetSkill(SkillSlot.Special);
                                if (skill)
                                {
                                    skill.DeductStock(1);
                                }
                            }
                        }
                        Util.PlaySound(self.placeSoundString, self.gameObject);
                        self.DestroyBlueprints();
                        self.exitPending = true;
                    }
                    if (self.inputBank.skill2.down && self.currentPlacementInfo.ok)
                    {
                        if (self.characterBody)
                        {
                            var engiTurretController = self.characterBody.GetComponent<EngiTurretController>();
                            self.characterBody.SendConstructTurret(self.characterBody, self.currentPlacementInfo.position, self.currentPlacementInfo.rotation, MasterCatalog.FindMasterIndex(skillToTurretMap[engiTurretController.special2])); // what turret here?????? what?
                            if (self.skillLocator)
                            {
                                GenericSkill skill = self.skillLocator.GetSkill(SkillSlot.Special);
                                if (skill)
                                {
                                    skill.DeductStock(1);
                                }
                            }
                        }
                        Util.PlaySound(self.placeSoundString, self.gameObject);
                        self.DestroyBlueprints();
                        self.exitPending = true;
                    }
                }
            }
        }
    }

    public class EngiTurretController : MonoBehaviour
    {
        public SkillDef special1;
        public SkillDef special2;
    }

    public class EngiTurretIdentifier : MonoBehaviour
    {
    }
}