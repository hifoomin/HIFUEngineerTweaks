using R2API.Utils;
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
        public static GenericSkill specialSlot1Skill;
        public static GenericSkill specialSlot2Skill;
        public static Dictionary<SkillDef, GameObject> skillToTurretMap;

        [SystemInitializer(typeof(SurvivorCatalog))]
        public static void Init()
        {
            specialSlot1Family = ScriptableObject.CreateInstance<SkillFamily>();
            specialSlot2Family = ScriptableObject.CreateInstance<SkillFamily>();

            var allTurrets = Addressables.LoadAssetAsync<SkillFamily>("RoR2/Base/Engi/EngiBodySpecialFamily.asset").WaitForCompletion().variants;

            specialSlot1Family.variants = allTurrets;
            specialSlot2Family.variants = allTurrets;

            for (int i = 0; i < allTurrets.Length; i++)
            {
                var skill = specialSlot1Family.variants[i].skillDef;
                skillToTurretMap.Add(skill, ((EntityStates.Engi.EngiWeapon.PlaceTurret)Activator.CreateInstance(skill.activationState.stateType)).turretMasterPrefab);
            }

            var engi = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Engi/EngiBody.prefab").WaitForCompletion();
            var specialSkill = engi.GetComponents<GenericSkill>().Where(x => x.skillName == "PlaceTurret").First();
            specialSkill._skillFamily = specialSlot1Family;

            var specialSkill2 = engi.AddComponent<GenericSkill>();
            specialSkill2._skillFamily = specialSlot2Family;
            specialSkill2.skillName = "PlaceTurret2";

            On.RoR2.Run.HandlePlayerFirstEntryAnimation += Run_HandlePlayerFirstEntryAnimation;
            On.EntityStates.Engi.EngiWeapon.PlaceTurret.FixedUpdate += PlaceTurret_FixedUpdate;
        }

        private static void Run_HandlePlayerFirstEntryAnimation(On.RoR2.Run.orig_HandlePlayerFirstEntryAnimation orig, Run self, CharacterBody body, Vector3 spawnPosition, Quaternion spawnRotation)
        {
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
                            self.characterBody.SendConstructTurret(self.characterBody, self.currentPlacementInfo.position, self.currentPlacementInfo.rotation, MasterCatalog.FindMasterIndex(skillToTurretMap[self.outer.GetComponents<GenericSkill>().Where(x => x.skillName == "PlaceTurret2").First().skillDef])); // what turret here?????? what?
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
}