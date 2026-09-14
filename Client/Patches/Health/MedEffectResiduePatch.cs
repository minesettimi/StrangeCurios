using System.Reflection;
using CuriosClient.System;
using EFT.HealthSystem;
using EFT.InventoryLogic;
using HarmonyLib;
using SPT.Reflection.Patching;
using UnityEngine;

namespace CuriosClient.Patches.Health;

public class MedEffectResiduePatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(ActiveHealthController.MedEffect),
            nameof(ActiveHealthController.MedEffect.Residue));
    }

    [PatchPrefix]
    public static void Prefix(ActiveHealthController.MedEffect __instance)
    {
        if (!CurioManager.InvControllerCurioTable.TryGetValue(__instance.HealthController._inventory,
                out CurioController curioController))
            return;

        MedKitComponent medkit = __instance._medKit;
        FoodDrinkComponent foodDrink = __instance._foodDrink;
        
        int minCurse = CurioPlugin.CurioConfig.CurseConfig.CurseMedsMin;
        
        if (curioController.TotalCurse < minCurse)
            return;
        
        //TODO: Separate uses for food
        float extraUses = Mathf.Floor((curioController.TotalCurse - minCurse) / CurioPlugin.CurioConfig.CurseConfig.CurseMeds);
        
        if (medkit is { HpResource: > 0 })
        {
            medkit.HpResource -= extraUses;
            
        }

        if (foodDrink is { HpPercent: > 0 })
        {
            foodDrink.HpPercent -= extraUses;
        }
    }
}