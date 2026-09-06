using System.Runtime.CompilerServices;
using EFT;
using EFT.InventoryLogic;

namespace CuriosClient.System;

//artificially connect the player inventory manager to the curio manager
public static class CurioManager
{
    //entries are deleted if the inventory controller is gone
    public static ConditionalWeakTable<InventoryController, CurioController> InvControllerToCurio = new();
}