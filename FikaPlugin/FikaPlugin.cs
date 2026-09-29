using System;
using System.Reflection;
using BepInEx;
using CuriosClient;
using CuriosClient.Models;
using CuriosClient.System;
using EFT;
using Fika.Core.Main.GameMode;
using Fika.Core.Main.Players;
using Fika.Core.Networking;
using Fika.Core.Networking.LiteNetLib.Utils;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace StrangeCuriosFika;

[BepInPlugin("com.minesettimi.curiosfika", "Strange Curios Fika", "1.0.0")]
[BepInDependency("com.minesettimi.curios", "1.1.0")]
[BepInDependency("com.fika.core", "2.4.2")]
public class FikaPlugin : BaseUnityPlugin
{
    private PatchManager _patchManager = null!;
    
    private void Awake()
    {
        _patchManager = new PatchManager(this, true);
        _patchManager.EnablePatches();
        
        EFTSerializationExtensions.RegisterPolymorphicType(CurioSerialization.PutCurioDescriptor, CurioSerialization.ReadCurioDescriptor);
    }
}

public class HostExtractPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(HostGameController), nameof(HostGameController.Extract));
    }

    [PatchPrefix]
    public static void Prefix(FikaPlayer player, IFikaGame ____fikaGame)
    {
        if (____fikaGame is not CoopGame coopGame)
        {
            return;
        }
        
        ChangeExtract(player, coopGame);
    }

    public static void ChangeExtract(FikaPlayer player, CoopGame coopGame)
    {
        if (!CurioManager.InvControllerCurioTable.TryGetValue(player.InventoryController,
                out CurioController curioController))
            return;
        
        CurioPlugin.PluginLogger.LogInfo($"Old exit status: {coopGame.ExitStatus}");
        
        switch (coopGame.ExitStatus)
        {
            case ExitStatus.MissingInAction or ExitStatus.Left when
                curioController.UseSpecialEffect(CurioSpecialEffects.ExfilTp):
                coopGame.ExitStatus = coopGame.ExitStatus == ExitStatus.Left ? ExitStatus.Runner : ExitStatus.Survived;
                return;
            case ExitStatus.Survived when curioController.TotalCurse > CurioPlugin.CurioConfig.CurseConfig.CurseRunThrough:
                coopGame.ExitStatus = ExitStatus.Left;
                break;
        }
        
        CurioPlugin.PluginLogger.LogInfo($"New exit status: {coopGame.ExitStatus}");
    }
}

public class ClientExtractPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(ClientGameController), nameof(ClientGameController.Extract));
    }

    [PatchPrefix]
    public static void Prefix(FikaPlayer player, IFikaGame ____fikaGame)
    {
        if (____fikaGame is not CoopGame coopGame)
        {
            return;
        }

        HostExtractPatch.ChangeExtract(player, coopGame);
    }
}