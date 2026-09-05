using EFT.InventoryLogic;

namespace CuriosClient.Models;

public class Curios : Item
{
    public Curios(string id, CuriosTemplate template) : base(id, template)
    {
    }
}