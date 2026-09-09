using System.Reflection;
using CuriosClient.System;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace CuriosClient.Patches.Movement;

//TODO: Better method, this is unoptimized
public class ClampedSpeedPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.PropertyGetter(typeof(MovementContext), nameof(MovementContext.ClampedSpeed));
    }

    [PatchPostfix]
    public static void Postfix(MovementContext __instance, Player ____player, ref float __result)
    {
        if (!CurioManager.InvControllerCurioTable.TryGetValue(____player.InventoryController,
                out CurioController curioController))
            return;

        __result += curioController.HighestSpeedBuff;
    }
}

public class SprintSpeedPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.PropertySetter(typeof(MovementContext), nameof(MovementContext.SprintSpeed));
    }

    [PatchPrefix]
    public static void Prefix(MovementContext __instance, Player ____player, ref float value)
    {
        if (!CurioManager.InvControllerCurioTable.TryGetValue(____player.InventoryController,
                out CurioController curioController))
            return;

        value += curioController.HighestSpeedBuff;
    }
}

public class MaxSpeedPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.PropertyGetter(typeof(MovementContext), nameof(MovementContext.MaxSpeed));
    }

    [PatchPostfix]
    public static void Postfix(MovementContext __instance, Player ____player, ref float __result)
    {
        if (!CurioManager.InvControllerCurioTable.TryGetValue(____player.InventoryController,
                out CurioController curioController))
            return;

        __result += curioController.HighestSpeedBuff;
    }
}