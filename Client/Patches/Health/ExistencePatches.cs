using System.Reflection;
using CuriosClient.System;
using EFT.HealthSystem;
using HarmonyLib;
using SPT.Reflection.Patching;
using NotImplementedException = System.NotImplementedException;

namespace CuriosClient.Patches.Health;

public class ExistencePatches : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(ActiveHealthController.Existence),
            nameof(ActiveHealthController.Existence.Started));
    }

    [PatchPrefix]
    public static bool Prefix(ActiveHealthController.Existence __instance)
    {
        __instance._energyLoopTime = ActiveHealthController.Effect.EffectsSettings.Existence.EnergyLoopTime;
        __instance._hydrationLoopTime = ActiveHealthController.Effect.EffectsSettings.Existence.HydrationLoopTime;

        if (__instance._destroyedStomach)
        {
            __instance._energyLoopTime /= ActiveHealthController.Effect.EffectsSettings.Existence.DestroyedStomachEnergyTimeFactor;
            __instance._hydrationLoopTime /= ActiveHealthController.Effect.EffectsSettings.Existence.DestroyedStomachEnergyTimeFactor;
        }

        float healthLoopTime = ActiveHealthController.Effect.EffectsSettings.Existence.EnergyLoopTime;
        float tempLoopTime = ActiveHealthController.Effect.EffectsSettings.Existence.EnergyLoopTime;
        if (__instance.HealthController._inventory != null &&
            CurioManager.InvControllerCurioTable.TryGetValue(__instance.HealthController._inventory,
                out CurioController curioController))
        {
            if (curioController.HealthEffects.TryGetValue(EHealthFactorType.Energy, out float energyVal))
                __instance._energyLoopTime += energyVal;

            if (curioController.HealthEffects.TryGetValue(EHealthFactorType.Health, out float healthVal))
                healthLoopTime += healthVal;
        
            if (curioController.HealthEffects.TryGetValue(EHealthFactorType.Hydration, out float hydrationVal))
                __instance._hydrationLoopTime += energyVal;
        
            if (curioController.HealthEffects.TryGetValue(EHealthFactorType.Temperature, out float temperatureVal))
                tempLoopTime += temperatureVal;
        }

        //__instance.SetHealthRatesPerSecond(curioController);

        return true;
    }
}