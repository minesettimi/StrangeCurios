using System;
using System.Collections.Generic;
using CuriosClient.Components;
using CuriosClient.Models;
using Diz.Binding;
using Diz.LanguageExtensions;
using EFT;
using EFT.InventoryLogic;

namespace CuriosClient.System;

public class CurioController
{
    public readonly InventoryController InventoryController;
    public readonly BindableEvent OnCurioUpdated = new();
    public Inventory Inventory;


    public float TotalCurse = 0f;
    public float TotalDamageReduction = 0f;
    public int HighestPenResistance = 0;
    public Dictionary<float, List<EquipmentSlot>> EquipmentRepair = [];
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
        OnCurioUpdated.Invoke();
    }

    private void RefreshCurioStats()
    {
        TotalCurse = 0f;
        TotalDamageReduction = 0f;
        HighestPenResistance = 0;
        EquipmentRepair.Clear();
        ItemSpecialEffects.Clear();
        SkillAdjustments.Clear();

        IEnumerable<CurioComponent> curiosToScan = Inventory.Equipment.GetItemComponentsInChildren<CurioComponent>(false);
        
        foreach (CurioComponent curioComponent in curiosToScan)
        {
            CuriosTemplate template = curioComponent.Template;
            
            TotalCurse += template.Curse;

            if (template.DamageReduction != null)
                TotalDamageReduction += (float)template.DamageReduction;

            if (template.PenResistance > HighestPenResistance)
                HighestPenResistance = (int)template.PenResistance;

            if (template.EquipmentRepair != null)
            {
                List<EquipmentSlot> equipSlots = template.EquipmentTargets ?? [EquipmentSlot.ArmorVest];
                
                EquipmentRepair.Add((float)template.EquipmentRepair, equipSlots);
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
        
        Plugin.PluginLogger.LogInfo($"New total curse value: {TotalCurse}");
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

        return !operationResult.Failed;
    }
}