using System.Reflection;
using CommonAssets.Scripts.Audio;
using CuriosClient.Effects;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace CuriosClient.Patches.Health;

public class ThresholdAudioCheckerPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(PlayerSelfInflictedDamageThresholdAudioChecker),
            nameof(PlayerSelfInflictedDamageThresholdAudioChecker.FillSelfInflictedHealthEffects));
    }

    [PatchPostfix]
    public static void Postfix(PlayerSelfInflictedDamageThresholdAudioChecker __instance)
    {
        __instance._selfInflictedHealthEffects.AddRange([typeof(ICursed), typeof(IUnkillable)]);
    }
}