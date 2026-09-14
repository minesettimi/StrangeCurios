using System;
using System.Collections.Generic;
using CuriosClient.Components;
using CuriosClient.Models;
using Diz.LanguageExtensions;
using EFT;
using EFT.HealthSystem;
using EFT.InventoryLogic;

namespace CuriosClient.System;

public class CurioController
{
    public readonly InventoryController InventoryController;
    public event Action? OnCurioUpdated;
    public Inventory Inventory;


    //TODO: More dynamic system like done with the stimulator (Maybe)
    public float TotalCurse;
    public float TotalDamageReduction;
    public int HighestPenResistance;
    public float HighestJumpBuff;
    public float TotalStaminaMax;
    public float TotalStaminaRate;
    public readonly Dictionary<EHealthFactorType, float> HealthEffects = [];
    public readonly Dictionary<EquipmentSlot, float> EquipmentRepair = [];
    public readonly Dictionary<CurioSpecialEffects, CurioComponent> ItemSpecialEffects = [];
    public readonly Dictionary<ESkillId, int> SkillAdjustments = [];

    public CurioController(InventoryController inventoryController, IInventoryProfileInfo profile)
    {
        InventoryController = inventoryController;
        Inventory = profile.InventoryInfo;
        
        InventoryController.RemoveItemEvent += Event_CurioStats;
        InventoryController.AddItemEvent += Event_CurioStats;
        InventoryController.RefreshItemEvent += Event_CurioStats;
        
        RefreshCurioStats();
    }

    public void Event_CurioStats(EventArgs? e)
    {
        if (e is not ItemEventArgs { Status: CommandStatus.Succeed, Location: not null } iArgs ||
            !iArgs.Location.IsChildOf(Inventory.Equipment))
            return;
        
        RefreshCurioStats();
    }

    private void RefreshCurioStats()
    {
        TotalCurse = 0f;
        TotalDamageReduction = 0f;
        HighestPenResistance = 0;
        HighestJumpBuff = 0f;
        TotalStaminaMax = 0f;
        TotalStaminaRate = 0f;
        HealthEffects.Clear();
        EquipmentRepair.Clear();
        ItemSpecialEffects.Clear();
        SkillAdjustments.Clear();

        IEnumerable<CurioComponent> curiosToScan = Inventory.Equipment.GetItemComponentsInChildren<CurioComponent>(false);
        
        foreach (CurioComponent curioComponent in curiosToScan)
        {
            CuriosTemplate template = curioComponent.Template;
            
            TotalCurse += template.Curse;

            foreach ((EHealthFactorType healthFactor, float value) in template.HealthEffects)
            {
                HealthEffects.TryAdd(healthFactor, 0f);
                HealthEffects[healthFactor] += value;
            }
            
            TotalDamageReduction += template.DamageReduction ?? 0;
            EffectConfig effectConfig = CurioPlugin.CurioConfig.EffectConfig;

            SetValueCumulative(effectConfig.CumulativeJump, ref HighestJumpBuff, template.JumpBuff);
            SetValueCumulative(effectConfig.CumulativePen, ref HighestPenResistance, template.PenResistance);
            SetValueCumulative(effectConfig.CumulativeStamina, ref TotalStaminaMax, template.StaminaMax);
            SetValueCumulative(effectConfig.CumulativeStamina, ref TotalStaminaRate, template.StaminaRate);

            if (template.EquipmentRepair != null && template.EquipmentRepair != 0)
            {
                List<EquipmentSlot> equipSlots;

                if (template.EquipmentTargets == null || template.EquipmentTargets.Count == 0)
                    equipSlots = [.. Inventory.ArmorSlots];
                else
                    equipSlots = template.EquipmentTargets;

                float repairAmount = (float)template.EquipmentRepair;

                foreach (EquipmentSlot slot in equipSlots)
                {
                    EquipmentRepair.TryAdd(slot, 0);
                    EquipmentRepair[slot] += repairAmount;
                }
            }
            
            if (template.SpecialEffect != CurioSpecialEffects.None)
                ItemSpecialEffects.TryAdd(template.SpecialEffect, curioComponent);

            if (template.SkillIncreases != null)
            {
                foreach ((ESkillId skill, int increase) in template.SkillIncreases)
                {
                    SkillAdjustments.TryAdd(skill, 0);
                    SkillAdjustments[skill] += increase;
                }
            }
        }
        
        OnCurioUpdated?.Invoke();
    }

    private void SetValueCumulative(bool cumulative, ref float sourceVal, float? newValue)
    {
        if (cumulative)
        {
            if (newValue > sourceVal)
                sourceVal = (float)newValue;
        }
        else if (newValue != null && newValue != 0)
        {
            sourceVal += (float)newValue;
        }
    }

    private void SetValueCumulative(bool cumulative, ref int sourceVal, int? newValue)
    {
        if (cumulative)
        {
            if (newValue > sourceVal)
                sourceVal = (int)newValue;
        }
        else if (newValue != null && newValue != 0)
        {
            sourceVal += (int)newValue;
        }
    }

    public bool UseSpecialEffect(CurioSpecialEffects effect)
    {
        if (effect == CurioSpecialEffects.None || !ItemSpecialEffects.TryGetValue(effect, out CurioComponent? curioComponent))
            return false;

        if (curioComponent.Template.MaxUses < 1)
            return true;

        curioComponent.NumberOfUsages++;

        if (curioComponent.NumberOfUsages < curioComponent.Template.MaxUses) return true;

        OperationResult<DiscardResult> operationResult = ItemManipulator.Discard(curioComponent.Item, 
            (ItemController)curioComponent.Item.Parent.GetOwner());

        if (!operationResult.Failed)
        {
            RefreshCurioStats();
            return true;
        }
        
        return false;
    }
}