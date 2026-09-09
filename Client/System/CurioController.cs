using System;
using System.Collections.Generic;
using System.Linq;
using CuriosClient.Components;
using CuriosClient.Models;
using Diz.Binding;
using Diz.LanguageExtensions;
using EFT;
using EFT.HealthSystem;
using EFT.InventoryLogic;
using Newtonsoft.Json;

namespace CuriosClient.System;

public class CurioController
{
    public readonly InventoryController InventoryController;
    public event Action? OnCurioUpdated;
    public Inventory Inventory;


    //TODO: More dynamic system like done with the stimulator
    public float TotalCurse = 0f;
    public float TotalDamageReduction = 0f;
    public int HighestPenResistance = 0;
    public Dictionary<EHealthFactorType, float> HealthEffects = [];
    public Dictionary<EquipmentSlot, float> EquipmentRepair = [];
    public Dictionary<CurioSpecialEffects, CurioComponent> ItemSpecialEffects = [];
    public Dictionary<ESkillId, int> SkillAdjustments = [];

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
            
            if (template.DamageReduction != null)
                TotalDamageReduction += (float)template.DamageReduction;

            if (template.PenResistance > HighestPenResistance)
                HighestPenResistance = (int)template.PenResistance;

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
                
                Plugin.PluginLogger.LogInfo(JsonConvert.SerializeObject(EquipmentRepair));
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