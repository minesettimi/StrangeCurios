using System.Reflection;
using CuriosClient.System;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace CuriosClient.Patches.Movement;

public class JumpStateEnterPatch : ModulePatch
{
    private static FieldInfo _playerField;
    
    protected override MethodBase GetTargetMethod()
    {
        _playerField = AccessTools.Field(typeof(MovementContext), "_player");
        return AccessTools.Method(typeof(JumpPlayerState), nameof(JumpPlayerState.Enter));
    }

    [PatchPostfix]
    public static void Postfix(JumpPlayerState __instance)
    {
        Player? player = _playerField.GetValue(__instance.MovementContext) as Player;
        if (player == null || !CurioManager.InvControllerCurioTable.TryGetValue(player.InventoryController,
                out CurioController curioController))
            return;

        __instance._skilledBoost += curioController.HighestJumpBuff;
    }
}