using System;
using System.Collections.Generic;
using CuriosClient.System;
using EFT;
using EFT.HealthSystem;
using EFT.InventoryLogic;

namespace CuriosClient.Effects;

public class CustomActiveEffects
{
    public class Cursed : ActiveHealthController.Effect, ICursed
    {
        private CurioController _curioController = null!;

        private float _healthLoopTime;
        private float _energyLoopTime;
        private float _hydrationLoopTime;
        private float _temperatureLoopTime;
        private float _armorLoopTime;

        private float _curHealthLoopTime;
        private float _curEnergyLoopTime;
        private float _curHydrationLoopTime;
        private float _curTemperatureLoopTime;
        private float _curArmorLoopTime;

        private Dictionary<ESkillId, int> _previousSkillChanges = [];

        private float _previousStaminaRate = 0;
        private float _previousStaminaMax = 0;

        public override void Started()
        {
            _healthLoopTime = CurioPlugin.CurioConfig.EffectConfig.HealthLoopTime;
            _energyLoopTime = CurioPlugin.CurioConfig.EffectConfig.EnergyLoopTime;
            _hydrationLoopTime = CurioPlugin.CurioConfig.EffectConfig.HydrationLoopTime;
            _temperatureLoopTime = CurioPlugin.CurioConfig.EffectConfig.TemperatureLoopTime;
            _armorLoopTime = CurioPlugin.CurioConfig.EffectConfig.ArmorLoopTime;

            if (HealthController._inventory == null ||
                !CurioManager.InvControllerCurioTable.TryGetValue(HealthController._inventory,
                    out CurioController curioController))
            {
                CurioPlugin.PluginLogger.LogError("Failed to get curio controller for Cursed condition.");
                ForceRemove();
                return;
            }

            _curioController = curioController;
            _curioController.OnCurioUpdated += Event_OnCurioUpdated;

            SetHealthRatesPerSecond(GetHealthBoost(), GetEnergyBoost(), GetHydrationBoost(), GetTemperatureBoost());

            if (_curioController.SkillAdjustments.Count > 0)
            {
                UpdateSkills(_curioController.SkillAdjustments);
            }

            UpdateBasicStats();
        }

        public override void Removed()
        {
            _curioController.OnCurioUpdated -= Event_OnCurioUpdated;

            Dictionary<ESkillId, int> skillChanges = [];
            foreach ((ESkillId skillId, int skillVal) in _previousSkillChanges)
            {
                skillChanges[skillId] = -skillVal;
            }

            UpdateSkills(skillChanges);
            UpdateBasicStats();
        }

        private void Event_OnCurioUpdated()
        {
            SetHealthRatesPerSecond(GetHealthBoost(), GetEnergyBoost(), GetHydrationBoost(), GetTemperatureBoost());
            UpdateBasicStats();

            Dictionary<ESkillId, int> skillChanges = [];
            foreach ((ESkillId skillId, int skillVal) in _previousSkillChanges)
            {
                skillChanges[skillId] = -skillVal;
            }

            foreach ((ESkillId skillId, int currSkillVal) in _curioController.SkillAdjustments)
            {
                if (!skillChanges.TryGetValue(skillId, out int previousSkillVal))
                {
                    skillChanges[skillId] = currSkillVal;
                    continue;
                }

                if (currSkillVal == -previousSkillVal)
                {
                    skillChanges.Remove(skillId);
                    continue;
                }

                skillChanges[skillId] = currSkillVal + previousSkillVal;
            }

            UpdateSkills(skillChanges);
        }

        public override void RegularUpdate(float deltaTime)
        {
            _curHealthLoopTime += deltaTime;
            _curEnergyLoopTime += deltaTime;
            _curHydrationLoopTime += deltaTime;
            _curTemperatureLoopTime += deltaTime;

            if (_curioController.EquipmentRepair.Count > 1)
                _curArmorLoopTime += deltaTime;

            if (_curHealthLoopTime >= _healthLoopTime)
            {
                _curHealthLoopTime -= _healthLoopTime;
                float healthBoost = GetHealthBoost();

                if (healthBoost != 0)
                {
                    foreach (EBodyPart bodyPart in HealthHelper.RealBodyParts.Randomize())
                    {
                        bool atClamp = (!HealthController.GetBodyPartHealth(bodyPart).AtMaximum && healthBoost >= 0) ||
                                       (!HealthController.GetBodyPartHealth(bodyPart).AtMinimum && healthBoost < 0);

                        if (atClamp && !HealthController.IsBodyPartDestroyed(bodyPart))
                        {
                            HealthController.ChangeHealth(bodyPart, healthBoost * _healthLoopTime,
                                DamageHelper.Existence);
                            break;
                        }
                    }
                }
            }

            if (_curEnergyLoopTime >= _energyLoopTime)
            {
                _curEnergyLoopTime -= _energyLoopTime;
                HealthController.ChangeEnergy(GetEnergyBoost() * _energyLoopTime);
            }

            if (_curHydrationLoopTime >= _hydrationLoopTime)
            {
                _curHydrationLoopTime -= _hydrationLoopTime;
                HealthController.ChangeHydration(GetHydrationBoost() * _hydrationLoopTime);
            }

            if (_curTemperatureLoopTime >= _temperatureLoopTime)
            {
                _curTemperatureLoopTime -= _temperatureLoopTime;
                HealthController.ChangeTemperature(GetTemperatureBoost() * _temperatureLoopTime);
            }

            if (_curArmorLoopTime >= _armorLoopTime)
            {
                _curArmorLoopTime -= _armorLoopTime;

                foreach ((EquipmentSlot equipmentSlot, float amount) in _curioController.EquipmentRepair)
                {
                    if (amount == 0)
                        continue;

                    Slot slot = HealthController._inventory.Inventory.Equipment.GetSlot(equipmentSlot);
                    if (slot.ContainedItem == null)
                        continue;

                    List<ArmorComponent> armorList = [];
                    slot.ContainedItem.GetItemComponentsInChildrenNonAlloc(armorList, false);

                    foreach (ArmorComponent armor in armorList.Randomize())
                    {
                        float damageAmount = amount * _armorLoopTime;
                        if (armor.Repairable.Durability >= armor.Repairable.MaxDurability)
                            continue;
                        damageAmount = Math.Clamp(damageAmount, -armor.Repairable.Durability,
                            armor.Repairable.MaxDurability - armor.Repairable.Durability);
                        
                        armor.ApplyDurabilityDamage(-damageAmount, armorList);
                        
                        break;
                    }
                }
            }
        }

        public float GetEnergyBoost()
        {
            return _curioController.HealthEffects.GetValueOrDefault(EHealthFactorType.Energy, 0f);
        }

        public float GetHydrationBoost()
        {
            return _curioController.HealthEffects.GetValueOrDefault(EHealthFactorType.Hydration, 0f);
        }

        public float GetTemperatureBoost()
        {
            return _curioController.HealthEffects.GetValueOrDefault(EHealthFactorType.Temperature, 0f);
        }

        public float GetHealthBoost()
        {
            return _curioController.HealthEffects.GetValueOrDefault(EHealthFactorType.Health, 0f);
        }

        private void UpdateSkills(Dictionary<ESkillId, int> skillChanges)
        {
            if (skillChanges.Count == 0)
                return;

            _previousSkillChanges.Clear();

            foreach (Skill skill in HealthController._skills.Skills)
            {
                if (!skillChanges.TryGetValue(skill.Id, out int valueChange))
                    continue;

                int clampedVal = Math.Clamp(valueChange, -skill.Level, 60 - skill.Level);

                skill.Buff += clampedVal;

                if (_curioController.SkillAdjustments.TryGetValue(skill.Id, out int skillVal))
                    _previousSkillChanges[skill.Id] = skillVal;
            }
        }

        private void UpdateBasicStats()
        {
            HealthController.Player.Physical.RestoreRateBuff +=
                _curioController.TotalStaminaRate - _previousStaminaRate;
            HealthController.Player.Physical.CapacityBuff += _curioController.TotalStaminaMax - _previousStaminaMax;

            _previousStaminaRate = _curioController.TotalStaminaRate;
            _previousStaminaMax = _curioController.TotalStaminaMax;
        }
    }

    public class Unkillable : ActiveHealthController.Effect, IUnkillable
    {
    }
}