using System.Reflection;
using CuriosClient.System;
using EFT.InventoryLogic;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace CuriosClient.Patches.Inventory;

public class InventoryControllerPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Constructor(typeof(InventoryController), [typeof(IInventoryProfileInfo), typeof(bool)]);
    }

    [PatchPostfix]
    public static void Postfix(InventoryController __instance, IInventoryProfileInfo profile)
    {
        CurioManager.InvControllerCurioTable.AddOrUpdate(__instance, new CurioController(__instance, profile));
    }
}