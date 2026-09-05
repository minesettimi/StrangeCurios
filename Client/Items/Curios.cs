using EFT.InventoryLogic;
using WTTClientCommonLib.Attributes;

namespace CuriosClient.Models;

[CustomParent("6a9b52c15dfaf97d11fc341e", typeof(Curios), typeof(CuriosTemplate))]
public class Curios : Item
{
    public Curios(string id, CuriosTemplate template) : base(id, template)
    {
    }
}