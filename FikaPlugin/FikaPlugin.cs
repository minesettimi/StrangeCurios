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

public class ExtractPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(BaseGameController), nameof(BaseGameController.Extract));
    }

    [PatchPrefix]
    public static void Prefix(FikaPlayer player, IFikaGame ____fikaGame)
    {
        if (____fikaGame is not CoopGame coopGame)
        {
            return;
        }
        
        if (!CurioManager.InvControllerCurioTable.TryGetValue(player.InventoryController,
                out CurioController curioController))
            return;
        
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
    }
}