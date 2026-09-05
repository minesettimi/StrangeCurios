using CuriosClient.Components;
using EFT.InventoryLogic;
using Newtonsoft.Json;
using WTTClientCommonLib.Attributes;

namespace CuriosClient.Models;

[CustomParent("6a9b52c15dfaf97d11fc341e", typeof(Curios), typeof(CuriosTemplate))]
public class Curios : Item
{
    public Curios(string id, CuriosTemplate template) : base(id, template)
    {
        Components.Add(CurioComponent = new CurioComponent(this, template));
    }

    public override bool Compare(Item item)
    {
        return base.Compare(item) && item is Curios;
    }

    [JsonProperty("curioEffects")] [EFT.Component]
    public readonly CurioComponent CurioComponent;
}