using System.Reflection;
using CuriosClient.System;
using EFT.HealthSystem;
using HarmonyLib;
using SPT.Reflection.Patching;
using NotImplementedException = System.NotImplementedException;

namespace CuriosClient.Patches.Health;

public class SetHealthRatesPerSecond : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(ActiveHealthController.Effect),
            nameof(ActiveHealthController.Effect.SetHealthRatesPerSecond));
    }

    [PatchPrefix]
    public static bool Prefix(ActiveHealthController.Effect __instance, ref float health, ref float energy,
        ref float hydration, ref float temperature)
    {
        if (__instance is not ActiveHealthController.Existence)
            return true;
        
        if (__instance.HealthController._inventory == null ||
            !CurioManager.InvControllerCurioTable.TryGetValue(__instance.HealthController._inventory,
                out CurioController curioController))
        {
            return true;
        }

        if (curioController.HealthEffects.TryGetValue(EHealthFactorType.Energy, out float energyVal))
        {
            energy += energyVal;
        }

        if (curioController.HealthEffects.TryGetValue(EHealthFactorType.Health, out float healthVal))
            health += healthVal;
        
        if (curioController.HealthEffects.TryGetValue(EHealthFactorType.Hydration, out float hydrationVal))
            hydration += hydrationVal;
        
        if (curioController.HealthEffects.TryGetValue(EHealthFactorType.Temperature, out float temperatureVal))
            temperature += temperatureVal;

        return true;
    }
}