using System.Reflection;
using CuriosClient.System;
using Diz.LanguageExtensions;
using EFT;
using EFT.Interactive;
using EFT.InventoryLogic;
using HarmonyLib;
using SPT.Reflection.Patching;
using Unity.Mathematics.Geometry;
using UnityEngine;
using Math = System.Math;

namespace CuriosClient.Patches.World;

public class UnlockKeycardPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(KeycardDoor), nameof(KeycardDoor.UnlockOperation));
    }

    [PatchPostfix]
    public static void PatchPostfix(ref Option<UnlockResult> __result, KeyComponent key, Player player)
    {
        UseExtraKeyUses(ref __result, key, player);
    }

    public static void UseExtraKeyUses(ref Option<UnlockResult> result, KeyComponent key, Player player)
    {
        if (!CurioManager.InvControllerCurioTable.TryGetValue(player.InventoryController,
                out CurioController curioController))
            return;

        int minCurse = CurioPlugin.CurioConfig.CurseConfig.CurseKeysMin;
        if (!result.Succeeded || key.NumberOfUsages >= key.Template.MaximumNumberOfUsage || curioController.TotalCurse < minCurse) return;
        
        int extraKeyUses = Mathf.FloorToInt((curioController.TotalCurse - minCurse) / Math.Max(CurioPlugin.CurioConfig.CurseConfig.CurseKeys, 1));
        key.NumberOfUsages += extraKeyUses;
        
        CurioPlugin.PluginLogger.LogInfo($"Total curse keys; {curioController.TotalCurse}");

        if (key.NumberOfUsages >= key.Template.MaximumNumberOfUsage && key.Template.MaximumNumberOfUsage > 0)
        {
            OperationResult<DiscardResult> operationResult = ItemManipulator.Discard(key.Item, (ItemController)key.Item.Parent.GetOwner());
            if (operationResult.Failed)
            {
                result = operationResult.Error;
                return;
            }

            result.Value._discardResult = operationResult.Value;
        }
    }
}

public class UnlockKeyPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(WorldInteractiveObject), nameof(WorldInteractiveObject.UnlockOperation));
    }
    
    [PatchPostfix]
    public static void PatchPostfix(ref Option<UnlockResult> __result, KeyComponent key, Player player)
    {
        UnlockKeycardPatch.UseExtraKeyUses(ref __result, key, player);
    }
}