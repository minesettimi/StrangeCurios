using System;
using System.Reflection;
using CuriosClient.Models;
using CuriosClient.System;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;
using Unity.Mathematics;
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
        if (__instance.Player == null || 
            !CurioManager.InvControllerCurioTable.TryGetValue(
                __instance.Player.InventoryController, out CurioController curioController))
            return;
                
        ProfileStats stats = __instance.Profile.EftStats;

        CurseConfig curseConfig = CurioPlugin.CurioConfig.CurseConfig;
        float xpMult = math.remap(0, curseConfig.CurseXpCount,
            curseConfig.CurseMaxXp, 1f, Math.Max(curseConfig.CurseXpCount - curioController.TotalCurse, 0));
        
        CurioPlugin.PluginLogger.LogInfo($"Xp multiplier: {xpMult}");
        
        int xpReduction = Mathf.RoundToInt(Math.Clamp(xpMult, 0f, 1f));
        
        stats.TotalSessionExperience -= xpReduction;
        __instance.Profile.Info.Experience -= xpReduction;
    }
}