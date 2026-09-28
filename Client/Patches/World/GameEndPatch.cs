using System.Reflection;
using CuriosClient.Models;
using CuriosClient.System;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace CuriosClient.Patches.World;

public class GameEndPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(BaseLocalGame<EftGamePlayerOwner>), nameof(BaseLocalGame<>.GameEnd));
    }

    [PatchPrefix]
    public static void Prefix(BaseLocalGame<EftGamePlayerOwner> __instance, ref ExitStatus exitStatus)
    {
        if (CurioPlugin.IsFika)
            return;
        
        if (!CurioManager.InvControllerCurioTable.TryGetValue(__instance.PlayerOwner.Player.InventoryController,
                out CurioController curioController))
            return;

        switch (exitStatus)
        {
            case ExitStatus.MissingInAction or ExitStatus.Left when
                curioController.UseSpecialEffect(CurioSpecialEffects.ExfilTp):
                exitStatus = exitStatus == ExitStatus.Left ? ExitStatus.Runner : ExitStatus.Survived;
                return;
            case ExitStatus.Survived when curioController.TotalCurse > CurioPlugin.CurioConfig.CurseConfig.CurseRunThrough:
                exitStatus = ExitStatus.Left;
                break;
        }
    }
}