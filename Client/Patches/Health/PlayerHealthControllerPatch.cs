using System.Reflection;
using CuriosClient.Effects;
using EFT;
using EFT.HealthSystem;
using EFT.InventoryLogic;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace CuriosClient.Patches.Health;

public class PlayerHealthControllerPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Constructor(typeof(PlayerHealthController),
        [
            typeof(Profile.HealthInfo), typeof(Player), typeof(InventoryController), typeof(SkillManager), typeof(bool)
        ]);
    }

    [PatchPostfix]
    public static void Postfix(PlayerHealthController __instance)
    {
        __instance.AddEffect<Cursed>(EBodyPart.Head);
    }
}