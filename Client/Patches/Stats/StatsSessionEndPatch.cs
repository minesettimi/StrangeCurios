using System;
using System.Reflection;
using CuriosClient.System;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;
using UnityEngine;

namespace CuriosClient.Patches.Stats;

public class StatsSessionEndPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(BaseStatisticsManager), nameof(BaseStatisticsManager.EndStatisticsSession));
    }

    [PatchPostfix]
    public static void Postfix(BaseStatisticsManager __instance)
    {
        if (!CurioManager.InvControllerCurioTable.TryGetValue(__instance.Player.InventoryController,
                out CurioController curioController))
            return;
                
        ProfileStats stats = __instance.Profile.EftStats;
                
        //TODO: Config based xp rates
        float xpReduction = stats.TotalSessionExperience * (1 - curioController.TotalCurse / 100);
        int trueXpReduction = Mathf.RoundToInt(Math.Clamp(xpReduction, 0f, 1f));
        
        stats.TotalSessionExperience -= trueXpReduction;
        __instance.Profile.Info.Experience -= trueXpReduction;
    }
}