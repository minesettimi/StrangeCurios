using System.Collections.Generic;
using CuriosClient.Models;
using EFT.HealthSystem;
using EFT.InventoryLogic;
using JsonType;

namespace CuriosClient.Components;

public class CurioEffectsComponent : ItemComponent
{
    private CuriosTemplate _template;
    
    public CurioEffectsComponent(Item item, CuriosTemplate template) : base(item)
    {
        _template = template;
        
        Item.CreateAttributesFromDictionary(_template.HealthEffects,
            EItemAttributeDisplayType.Compact,
            EItemAttributeLabelVariations.Colored);
        SetupCurioAttributes(item);
    }

    public void SetupCurioAttributes(Item item)
    {
        List<ItemAttribute> attributes = item.Attributes;
        
        attributes.Add(new ItemAttribute(CurioAttributes.Curse)
        {
            Name = "CURSE",
            StringValue = () => $"<color=purple>{_template.Curse}</color>",
            DisplayType = () => EItemAttributeDisplayType.Compact,
            LabelVariations = EItemAttributeLabelVariations.Colored
        });

        foreach (EDamageEffectType damageEffectType in _template.DamageEffects)
        {
            ItemAttribute itemAttribute = new(damageEffectType)
            {
                Name = damageEffectType.ToString(),
                DisplayType = () => EItemAttributeDisplayType.Compact,
                IsTextValueDisplayable = false
            };
            
            attributes.Add(itemAttribute);
        }

        if (_template.DamageReduction > 0)
        {
            attributes.Add(new ItemAttribute(CurioAttributes.DamageReduction)
            {
                Name = "DAMAGE REDUCTION",
                StringValue = _template.DamageReduction.ToString,
                DisplayType = () => EItemAttributeDisplayType.Compact
            });
        }
    }
}