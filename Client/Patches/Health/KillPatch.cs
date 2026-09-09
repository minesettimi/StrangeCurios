using System.Reflection;
using CuriosClient.Models;
using CuriosClient.System;
using EFT.HealthSystem;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace CuriosClient.Patches.Health;

public class KillPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(ActiveHealthController), nameof(ActiveHealthController.Kill));
    }

    [PatchPrefix]
    public static bool Prefix(ActiveHealthController __instance)
    {
        if (!CurioManager.InvControllerCurioTable.TryGetValue(__instance.Player.InventoryController,
                out CurioController curioController))
            return true;

        if (!curioController.UseSpecialEffect(CurioSpecialEffects.NewLife))
            return true;

        __instance.RestoreFullHealth();

        return false;
    }
}