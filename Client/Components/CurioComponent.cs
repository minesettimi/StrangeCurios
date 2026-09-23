using System;
using System.Collections.Generic;
using CuriosClient.Models;
using EFT;
using EFT.HealthSystem;
using EFT.InventoryLogic;

namespace CuriosClient.Components;

public class CurioComponent : ItemComponent, IRelativeComponent
{
    public readonly CuriosTemplate Template;
    
    [Diffable]
    public int NumberOfUsages;
    
    public CurioComponent(Item item, CuriosTemplate template) : base(item)
    {
        Template = template;

        if (Template.MaxUses > 0)
        {
            Item.Attributes.Add(new ItemAttribute(EItemAttributeId.KeyUses)
            {
                Name = "CURIO USES",
                StringValue = () =>
                {
                    int uses = template.MaxUses - NumberOfUsages;
                    string usesLeft = uses == 1 ? $"<color=red>{uses}</color>" : 
                        uses.ToString();
                    return $"{usesLeft}/{template.MaxUses}";
                },
                DisplayType = () => EItemAttributeDisplayType.Compact
            });
        }
        
        SetupCurioAttributes(item);
    }

    public void SetupCurioAttributes(Item item)
    {
        List<ItemAttribute> attributes = item.Attributes;
        
        attributes.Add(new ItemAttribute(CurioAttributes.Curse)
        {
            Name = "CURSE".Localized(),
            StringValue = () => $"<color=purple>{Template.Curse}</color>",
            DisplayType = () => EItemAttributeDisplayType.Compact,
            LabelVariations = EItemAttributeLabelVariations.Colored
        });
        
        AddBasicValue(attributes, Template.JumpBuff, CurioAttributes.JumpBuff, "JUMP BUFF");
        AddBasicValue(attributes, Template.DamageReduction, CurioAttributes.DamageReduction, "DAMAGE REDUCTION");
        AddBasicValue(attributes, Template.PenResistance, CurioAttributes.PenResistance, "PEN RESISTANCE");
        AddBasicValue(attributes, Template.StaminaMax, CurioAttributes.StaminaMax, "STAMINA MAX");
        AddBasicValue(attributes, Template.StaminaRate, CurioAttributes.StaminaRate, "STAMINA RATE");
        

        if (Template.SpecialEffect != CurioSpecialEffects.None)
        {
            attributes.Add(new ItemAttribute(CurioAttributes.SpecialEffect)
            {
                Name = Template.SpecialEffect.ToString().Localized(),
                DisplayType = () => EItemAttributeDisplayType.Compact,
                LabelVariations = EItemAttributeLabelVariations.Colored
            });
        }

        if (Template.HealthEffects.Count > 0)
        {
            foreach ((EHealthFactorType healthFactorType, float value) in Template.HealthEffects)
            {
                attributes.Add(new ItemAttribute(CurioAttributes.HealthRates)
                {
                    Name = healthFactorType.ToString(),
                    DisplayNameFunc = () => $"{healthFactorType.ToString().Localized()}",
                    DisplayType = () => EItemAttributeDisplayType.Compact,
                    LabelVariations = EItemAttributeLabelVariations.Colored,
                    StringValue = () => value.ColoredWithPrefix(value > 0)
                });
            }
        }
        
        if (Template.SkillIncreases != null)
        {
            foreach ((ESkillId skill, int value) in Template.SkillIncreases)
            {
                attributes.Add(new ItemAttribute(CurioAttributes.SkillIncrease)
                {
                    Name = skill.ToString(),
                    DisplayNameFunc = () => $"{"Skill".Localized()} \"{skill.ToString().Localized()}\"",
                    DisplayType = () => EItemAttributeDisplayType.Compact,
                    LabelVariations = EItemAttributeLabelVariations.Colored,
                    StringValue = () =>
                        (value >= 0
                            ? $"{"Increase".Localized()}<color=#54c1ff>"
                            : $"{"Decrease".Localized()}<color=red>") + $" {Math.Abs(value)}" + "</color>"
                });
            }
        }
        
        if (Template.EquipmentRepair != null)
        {
            if (Template.EquipmentRepair.Count < 5)
            {
                foreach ((EquipmentSlot slot, float value) in Template.EquipmentRepair)
                {
                    attributes.Add(new ItemAttribute(CurioAttributes.EquipmentRepair)
                    {
                        Name = slot.ToString(),
                        DisplayNameFunc = () => $"{"EQUIPMENT REPAIR".Localized()} {slot.ToString().Localized()}",
                        DisplayType = () => EItemAttributeDisplayType.Compact,
                        LabelVariations = EItemAttributeLabelVariations.Colored,
                        StringValue = () => (value * CurioPlugin.CurioConfig.EffectConfig.ArmorLoopTime).ColoredWithPrefix(value > 0)
                    });
                }
            }
            else
            {
                float totalValue = 0;

                foreach ((EquipmentSlot slot, float value) in Template.EquipmentRepair)
                {
                    totalValue += value;
                }
                float avg = totalValue / Template.EquipmentRepair.Count;
                
                attributes.Add(new ItemAttribute(CurioAttributes.EquipmentRepair)
                {
                    Name = "EQUIPMENT REPAIR".Localized(),
                    DisplayType = () => EItemAttributeDisplayType.Compact,
                    LabelVariations = EItemAttributeLabelVariations.Colored,
                    StringValue = () => (avg * CurioPlugin.CurioConfig.EffectConfig.ArmorLoopTime).ColoredWithPrefix(avg > 0)
                });
            }
        }
    }

    private void AddBasicValue(List<ItemAttribute> attributes, float? value, CurioAttributes attributeEnum, string translation)
    {
        if (value == null || value == 0)
            return;
        
        attributes.Add(new ItemAttribute(attributeEnum)
        {
            Name = translation.Localized(),
            StringValue = () => ((float)value).ColoredWithPrefix(value > 0),
            DisplayType = () => EItemAttributeDisplayType.Compact,
            LabelVariations = EItemAttributeLabelVariations.Colored
        });
    }

    public float RelativeValue => 1 - NumberOfUsages / (float)Template.MaxUses;
}