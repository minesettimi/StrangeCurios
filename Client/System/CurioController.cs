using System;
using Diz.Binding;
using EFT.InventoryLogic;

namespace CuriosClient.System;

public class CurioController
{
    public readonly InventoryController InventoryController;
    public Inventory Inventory;

    public readonly BindableEvent OnCurioUpdated = new();

    public CurioController(InventoryController inventoryController, IInventoryProfileInfo profile)
    {
        InventoryController = inventoryController;
        Inventory = profile.InventoryInfo;
        
        InventoryController.RemoveItemEvent += Event_CurioStats;
        InventoryController.AddItemEvent += Event_CurioStats;
        InventoryController.RefreshItemEvent += Event_CurioStats;
    }

    public void Event_CurioStats(EventArgs? e)
    {
        if (e is not ItemEventArgs { Status: CommandStatus.Succeed, Location: not null } iArgs ||
            !iArgs.Location.IsChildOf(Inventory.Equipment))
            return;
        
        OnCurioUpdated.Invoke();
    }
}