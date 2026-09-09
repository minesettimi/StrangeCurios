using System.Reflection;
using CuriosClient.Models;
using CuriosClient.System;
using EFT;
using EFT.Ballistics;
using EFT.HealthSystem;
using HarmonyLib;
using SPT.Reflection.Patching;
using UnityEngine;

namespace CuriosClient.Patches.Health;

public class ApplyDamagePatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(ActiveHealthController), nameof(ActiveHealthController.ApplyDamage));
    }

    [PatchPrefix]
    public static bool Prefix(ActiveHealthController __instance, EBodyPart bodyPart, ref float damage,
        DamageInfo damageInfo, ref float __result)
    {
        if (!__instance.IsAlive || __instance.DamageCoeff <= 0)
            return true;
        
        if (!CurioManager.InvControllerCurioTable.TryGetValue(__instance.Player.InventoryController,
                out CurioController curioController))
            return true;

        damage = Mathf.Max(damage - curioController.TotalDamageReduction, 0);

        if (curioController.HighestPenResistance * 10 > damageInfo.PenetrationPower)
        {
            damage *= .5f;
        }

        bool isEnemy = damageInfo.DamageType.IsEnemyDamage();
        if (isEnemy)
        {
            IObserverToPlayerBridge? otherPlayer = damageInfo.Player;

            if (otherPlayer is { iPlayer: Player enemyPlayer } && curioController.UseSpecialEffect(CurioSpecialEffects.Reflect))
            {
                enemyPlayer.ApplyDamageInfo(new DamageInfo { DamageType = EDamageType.Bullet }, bodyPart,
                    damageInfo.BodyPartColliderType, 0f);
            }
        }
        
        ValueStruct bodyPartHealth = __instance.GetBodyPartHealth(bodyPart);
        if (bodyPart == EBodyPart.Head && isEnemy && bodyPartHealth.Current - damage <= bodyPartHealth.Minimum &&
            curioController.UseSpecialEffect(CurioSpecialEffects.HeadshotProt))
        {
            __result = 0;
            return false;
        }

        return true;
    }
}