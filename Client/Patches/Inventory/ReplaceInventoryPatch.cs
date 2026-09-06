using System.Reflection;
using CuriosClient.System;
using EFT.InventoryLogic;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace CuriosClient.Patches.Inventory;

public class ReplaceInventoryPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(InventoryController),
            nameof(InventoryController.ReplaceInventory));
    }

    [PatchPrefix]
    public static void Prefix(InventoryController __instance, EFT.InventoryLogic.Inventory newInventory)
    {
        if (!CurioManager.InvControllerToCurio.TryGetValue(__instance, out CurioController curioController))
            return;

        curioController.Inventory = newInventory;
    }
}