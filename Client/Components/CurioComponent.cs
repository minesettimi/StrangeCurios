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

        if (Template.SpeedBuff != null && Template.SpeedBuff != 0)
        {
            attributes.Add(new ItemAttribute(CurioAttributes.SpeedBuff)
            {
                Name = "SPEED BUFF".Localized(),
                StringValue = Template.SpeedBuff.ToString,
                DisplayType = () => EItemAttributeDisplayType.Compact,
                LabelVariations = EItemAttributeLabelVariations.Colored
            });
        }
        
        if (Template.JumpBuff != null && Template.JumpBuff != 0)
        {
            attributes.Add(new ItemAttribute(CurioAttributes.JumpBuff)
            {
                Name = "JUMP BUFF".Localized(),
                StringValue = Template.JumpBuff.ToString,
                DisplayType = () => EItemAttributeDisplayType.Compact,
                LabelVariations = EItemAttributeLabelVariations.Colored
            });
        }

        if (Template.DamageReduction != null && Template.DamageReduction != 0)
        {
            attributes.Add(new ItemAttribute(CurioAttributes.DamageReduction)
            {
                Name = "DAMAGE REDUCTION".Localized(),
                StringValue = Template.DamageReduction.ToString,
                DisplayType = () => EItemAttributeDisplayType.Compact,
                LabelVariations = EItemAttributeLabelVariations.Colored
            });
        }

        if (Template.PenResistance != null && Template.PenResistance != 0)
        {
            attributes.Add(new ItemAttribute(CurioAttributes.PenResistance)
            {
                Name = "PEN RESISTANCE".Localized(),
                StringValue = Template.PenResistance.ToString,
                DisplayType = () => EItemAttributeDisplayType.Compact,
                LabelVariations = EItemAttributeLabelVariations.Colored
            });
        }

        if (Template.EquipmentRepair != null && Template.EquipmentRepair != 0)
        {
            attributes.Add(new ItemAttribute(CurioAttributes.EquipmentRepair)
            {
                Name = "EQUIPMENT REPAIR".Localized(),
                StringValue = Template.EquipmentRepair.ToString,
                DisplayType = () => EItemAttributeDisplayType.Compact,
                LabelVariations = EItemAttributeLabelVariations.Colored
            });
        }

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

    public float RelativeValue => 1 - NumberOfUsages / (float)Template.MaxUses;
}