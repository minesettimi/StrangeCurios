using System.Reflection;
using CuriosClient.Effects;
using EFT;
using EFT.HealthSystem;
using EFT.InventoryLogic;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace CuriosClient.Patches.Health;

public class PlayerInitPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(Player), nameof(Player.Init));
    }

    [PatchPostfix]
    public static void Postfix(Player __instance)
    {
        if (!__instance.IsYourPlayer || __instance.HealthController is not ActiveHealthController healthController)
            return;
        
        healthController.AddEffect<Cursed>(EBodyPart.Head);
    }
}