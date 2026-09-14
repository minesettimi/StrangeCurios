using System;
using System.Collections.Generic;
using CuriosClient.Models;
using EFT;
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
        AddBasicValue(attributes, Template.EquipmentRepair, CurioAttributes.EquipmentRepair, "EQUIPMENT REPAIR");
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

        if (Template.SkillIncreases != null)
        {
            foreach ((ESkillId skill, int value) in Template.SkillIncreases)
            {
                attributes.Add(new ItemAttribute(CurioAttributes.EquipmentRepair)
                {
                    Name = skill.ToString(),
                    DisplayNameFunc = () => $"{"Skill".Localized()} \"{skill.ToString().Localized()}\"",
                    DisplayType = () => EItemAttributeDisplayType.Compact,
                    LabelVariations = EItemAttributeLabelVariations.Colored,
                    StringValue = () =>
                        value >= 0
                            ? $"{"Increase".Localized()}<color=blue>"
                            : $"{"Decrease".Localized()}<color=red>" + $" { Math.Abs(value)}" + "</color>"
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
            StringValue = value.ToString,
            DisplayType = () => EItemAttributeDisplayType.Compact,
            LabelVariations = EItemAttributeLabelVariations.Colored
        });
    }

    public float RelativeValue => 1 - NumberOfUsages / (float)Template.MaxUses;
}