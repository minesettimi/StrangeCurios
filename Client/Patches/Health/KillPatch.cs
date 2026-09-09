using System.Reflection;
using CuriosClient.Models;
using CuriosClient.System;
using EFT;
using EFT.HealthSystem;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace CuriosClient.Patches.Health;

public class KillPatch : ModulePatch
{
    //ignore landmines and snipers
    private static EDamageType _damageMask =
        DamageHelper.SelfInflictedDamage | DamageHelper.EnvironmentalDamage | EDamageType.Bullet | EDamageType.Melee | EDamageType.Blunt;
    
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(ActiveHealthController), nameof(ActiveHealthController.Kill));
    }

    [PatchPrefix]
    public static bool Prefix(ActiveHealthController __instance, EDamageType damageType)
    {
        if (!CurioManager.InvControllerCurioTable.TryGetValue(__instance.Player.InventoryController,
                out CurioController curioController))
            return true;

        if ((damageType & _damageMask) > 0 && !curioController.UseSpecialEffect(CurioSpecialEffects.NewLife))
            return true;

        __instance.RestoreFullHealth();

        return false;
    }
}