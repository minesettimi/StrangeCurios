using System.Reflection;
using CuriosClient.Models;
using CuriosClient.System;
using EFT;
using EFT.Interactive;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace CuriosClient.Patches.World;

public class StartDoorBreakPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(MovementContext), nameof(MovementContext.StartDoorBreak));
    }

    [PatchPrefix]
    public static void Prefix(MovementContext __instance, Player ____player, WorldInteractiveObject interactive, out bool __state)
    {
        __state = false;
        if (!CurioManager.InvControllerCurioTable.TryGetValue(____player.InventoryController,
                out CurioController curioController))
            return;
        
        Door door = (interactive as Door)!;
        if (door.DoorState != EDoorState.Locked || !door.Operatable)
            return;
        
        if (curioController.UseSpecialEffect(CurioSpecialEffects.DoorBreaker))
        {
            door.CanBeBreached = true;
            __state = true;
        }
    }

    [PatchPostfix]
    public static void Postfix(MovementContext __instance, bool __state)
    {
        if (!__state)
            return;
        
        __instance.NextBreachResult = true;
        __instance.PlayerAnimatorSetKickSucceed(true);
    }
}