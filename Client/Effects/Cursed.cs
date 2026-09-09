using System;
using System.Collections.Generic;
using CuriosClient.System;
using EFT;
using EFT.HealthSystem;

namespace CuriosClient.Effects;

public class Cursed : ActiveHealthController.Effect, IExistence
{
    private CurioController _curioController = null!;

    private float _healthLoopTime;
    private float _energyLoopTime;
    private float _hydrationLoopTime;
    private float _temperatureLoopTime;

    private float _curHealthLoopTime;
    private float _curEnergyLoopTime;
    private float _curHydrationLoopTime;
    private float _curTemperatureLoopTime;

    private Dictionary<ESkillId, int> _previousSkillChanges = [];

    public override void Started()
    {
        //I don't feel like hooking up settings for this atm, maybe later
        _healthLoopTime = EffectsSettings.Existence.EnergyLoopTime;
        _energyLoopTime = EffectsSettings.Existence.EnergyLoopTime;
        _hydrationLoopTime = EffectsSettings.Existence.HydrationLoopTime;
        _temperatureLoopTime = EffectsSettings.Existence.HydrationLoopTime;
        
        if (HealthController._inventory == null ||
            !CurioManager.InvControllerCurioTable.TryGetValue(HealthController._inventory,
                out CurioController curioController))
        {
            Plugin.PluginLogger.LogError("Failed to get curio controller for Cursed condition.");
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
    }

    private void Event_OnCurioUpdated()
    {
        SetHealthRatesPerSecond(GetHealthBoost(), GetEnergyBoost(), GetHydrationBoost(), GetTemperatureBoost());

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

        if (_curHealthLoopTime >= _healthLoopTime)
        {
            _curHealthLoopTime -= _healthLoopTime;
            float healthBoost = GetHealthBoost();
            
            foreach (EBodyPart bodyPart in HealthHelper.RealBodyParts)
            {
                bool atClamp = (!HealthController.GetBodyPartHealth(bodyPart).AtMaximum && healthBoost >= 0) ||
                               (!HealthController.GetBodyPartHealth(bodyPart).AtMinimum && healthBoost < 0);

                if (atClamp && !HealthController.IsBodyPartDestroyed(bodyPart))
                {
                    HealthController.ChangeHealth(bodyPart, healthBoost * _healthLoopTime, DamageHelper.Existence);
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
}