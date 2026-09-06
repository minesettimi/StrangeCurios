using CuriosClient.Components;
using EFT.InventoryLogic;
using Newtonsoft.Json;
using WTTClientCommonLib.Attributes;

namespace CuriosClient.Models;

[CustomParent("6a9b52c15dfaf97d11fc341e", typeof(Curio), typeof(CuriosTemplate))]
public class Curio : Item
{
    public Curio(string id, CuriosTemplate template) : base(id, template)
    {
        Components.Add(CurioComponent = new CurioComponent(this, template));
    }

    public override bool Compare(Item item)
    {
        return base.Compare(item) && item is Curio;
    }

    [JsonProperty("Curio")] [EFT.Component]
    public readonly CurioComponent CurioComponent;
}