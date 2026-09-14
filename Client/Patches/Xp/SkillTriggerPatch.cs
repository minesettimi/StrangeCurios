using System;
using System.Reflection;
using Comfort.Common;
using CuriosClient.Models;
using CuriosClient.System;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;
using Unity.Mathematics;

namespace CuriosClient.Patches.Stats;

public class SkillTriggerPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(Skill), nameof(Skill.OnTrigger));
    }

    [PatchPrefix]
    public static void Prefix(Skill __instance, ref float val)
    {
        if (!Singleton<AbstractGame>.Instance.InRaid || !Singleton<GameWorld>.Instantiated)
            return;

        GameWorld gameWorld = Singleton<GameWorld>.Instance;
        if (gameWorld.MainPlayer == null)
            return;
        
        if (!CurioManager.InvControllerCurioTable.TryGetValue(gameWorld.MainPlayer.InventoryController,
                out CurioController curioController))
            return;
        
        CurseConfig curseConfig = CurioPlugin.CurioConfig.CurseConfig;

        if (curioController.TotalCurse < curseConfig.CurseSkillCountMin)
            return;
        
        float xpMult = math.remap(curseConfig.CurseSkillCountMin, curseConfig.CurseSkillCount,
            curseConfig.CurseMaxSkill, 1f, Math.Max(curseConfig.CurseSkillCount - curioController.TotalCurse, 0));
        
        val *= Math.Clamp(1.0f - xpMult, 0f, 1f);
    }
}