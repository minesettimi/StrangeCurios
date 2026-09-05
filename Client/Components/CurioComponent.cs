using System.Collections.Generic;
using CuriosClient.Models;
using EFT.HealthSystem;
using EFT.InventoryLogic;
using JsonType;

namespace CuriosClient.Components;

public class CurioComponent : ItemComponent, IRelativeComponent
{
    public readonly CuriosTemplate Template;
    public int UsesLeft = 0;
    
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
                    string usesLeft = UsesLeft == 1 ? $"<color=red>{UsesLeft}</color>" : UsesLeft.ToString();
                    return $"{usesLeft}/{template.MaxUses}";
                },
                DisplayType = () => EItemAttributeDisplayType.Compact
            });
        }
        
        SetupCurioAttributes(item);
        Item.CreateAttributesFromDictionary(Template.HealthEffects,
            EItemAttributeDisplayType.Compact,
            EItemAttributeLabelVariations.Colored);
    }

    public void SetupCurioAttributes(Item item)
    {
        List<ItemAttribute> attributes = item.Attributes;
        
        attributes.Add(new ItemAttribute(CurioAttributes.Curse)
        {
            Name = "CURSE",
            StringValue = () => $"<color=purple>{Template.Curse}</color>",
            DisplayType = () => EItemAttributeDisplayType.Compact,
            LabelVariations = EItemAttributeLabelVariations.Colored
        });

        foreach (EDamageEffectType damageEffectType in Template.DamageEffects)
        {
            ItemAttribute itemAttribute = new(damageEffectType)
            {
                Name = damageEffectType.ToString(),
                DisplayType = () => EItemAttributeDisplayType.Compact,
                IsTextValueDisplayable = false
            };
            
            attributes.Add(itemAttribute);
        }

        if (Template.DamageReduction != null)
        {
            attributes.Add(new ItemAttribute(CurioAttributes.DamageReduction)
            {
                Name = "DAMAGE REDUCTION",
                StringValue = Template.DamageReduction.ToString,
                DisplayType = () => EItemAttributeDisplayType.Compact,
                LabelVariations = EItemAttributeLabelVariations.Colored
            });
        }

        if (Template.PenResistance != null && Template.PenResistance != 0)
        {
            attributes.Add(new ItemAttribute(CurioAttributes.PenResistance)
            {
                Name = "PEN RESISTANCE",
                StringValue = Template.PenResistance.ToString,
                DisplayType = () => EItemAttributeDisplayType.Compact,
                LabelVariations = EItemAttributeLabelVariations.Colored
            });
        }

        if (Template.EquipmentRepair != null && Template.EquipmentRepair != 0)
        {
            attributes.Add(new ItemAttribute(CurioAttributes.EquipmentRepair)
            {
                Name = "EQUIPMENT REPAIR",
                StringValue = Template.EquipmentRepair.ToString,
                DisplayType = () => EItemAttributeDisplayType.Compact,
                LabelVariations = EItemAttributeLabelVariations.Colored
            });
        }
        
        
    }

    public float RelativeValue => UsesLeft / (float)Template.MaxUses;
}