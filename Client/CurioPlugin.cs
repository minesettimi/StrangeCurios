using System;
using BepInEx;
using BepInEx.Logging;
using CuriosClient.Effects;
using CuriosClient.Models;
using CuriosClient.Patches;
using CuriosClient.System;
using EFT.BinarySerialization;
using EFT.HealthSystem;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace CuriosClient;

[BepInPlugin("com.minesettimi.curios", "Strange Curios", "1.0.0")]
public class CurioPlugin : BaseUnityPlugin
{
    public static ManualLogSource PluginLogger = null!;
    public static CurioConfig CurioConfig = null!;

    private PatchManager _patchManager = null!;

    private void Awake()
    {
        _patchManager = new PatchManager(this, true);
        _patchManager.EnablePatches();

        PluginLogger = Logger;
        SyncConfig();
    }

    private static void SyncConfig()
    {
        try
        {
            CurioConfig = ConfigSync.GetConfig();
        }
        catch (Exception e)
        {
            PluginLogger.LogError($"Failed to get config with error: {e.Message}");
        }
    }
    
    private static readonly Type[] CustomEffects = [typeof(Cursed), typeof(IUnkillable)];

    private void Start()
    {
        //add to static classes
        BinarySerializationMirrorExtensions._types.Add(typeof(CurioComponentDescriptor));
        MirrorExtensionReadPatch.CurioIndex = BinarySerializationMirrorExtensions._types.Count - 1;

        HealthHelper.EffectTypeCode._effectTypes.AddRangeToArray(CustomEffects);
        foreach (Type type in CustomEffects)
        {
            byte index = (byte)Array.IndexOf(HealthHelper.EffectTypeCode._effectTypes, type);
            HealthHelper.EffectTypeCode._typeToByte[type.Name] = index;
            HealthHelper.EffectTypeCode._byteToType[index] = type.Name;
        }
    }
}