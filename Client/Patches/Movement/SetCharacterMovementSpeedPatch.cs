using System.Reflection;
using CuriosClient.System;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;
using NotImplementedException = System.NotImplementedException;

namespace CuriosClient.Patches.Movement;

public class SetCharacterMovementSpeedPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(MovementContext), nameof(MovementContext.SetCharacterMovementSpeed));
    }
    
    [PatchPrefix]
    public static void Prefix(MovementContext __instance, Player ____player, ref float characterMovementSpeed)
    {
        if (!CurioManager.InvControllerCurioTable.TryGetValue(____player.InventoryController,
                out CurioController curioController))
            return;

        characterMovementSpeed += curioController.HighestSpeedBuff;
    }
}