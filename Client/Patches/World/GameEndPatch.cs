using System.Reflection;
using CuriosClient.Models;
using CuriosClient.System;
using EFT;
using HarmonyLib;
using Newtonsoft.Json;
using SPT.Reflection.Patching;

namespace CuriosClient.Patches.World;

public class GameEndPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(BaseLocalGame<EftGamePlayerOwner>), nameof(BaseLocalGame<EftGamePlayerOwner>.GameEnd));
    }

    [PatchPrefix]
    public static void Prefix(BaseLocalGame<EftGamePlayerOwner> __instance, ref ExitStatus exitStatus)
    {
        ProfileDescriptor profileDescriptor = new(__instance.Profile, FullySearchedSearchController.Instance);
        Plugin.PluginLogger.LogInfo(JsonConvert.SerializeObject(profileDescriptor.Health));
        
        if (!CurioManager.InvControllerCurioTable.TryGetValue(__instance.PlayerOwner.Player.InventoryController,
                out CurioController curioController))
            return;

        if ((exitStatus == ExitStatus.MissingInAction || exitStatus == ExitStatus.Left) && curioController.UseSpecialEffect(CurioSpecialEffects.ExfilTp))
            exitStatus = exitStatus == ExitStatus.Left ? ExitStatus.Runner : ExitStatus.Survived;
    }
}