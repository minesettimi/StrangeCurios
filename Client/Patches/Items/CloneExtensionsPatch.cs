using System.Reflection;
using CuriosClient.Extensions;
using CuriosClient.Models;
using EFT;
using EFT.BinarySerialization;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace CuriosClient.Patches;

public class CloneExtensionsPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(BinaryCloneExtensions), nameof(BinaryCloneExtensions.ClonePolymorph), 
            generics: [typeof(ItemComponentDescriptor)]);
    }

    [PatchPrefix]
    public static bool Prefix(object source, ref ItemComponentDescriptor __result)
    {
        if (source is CurioComponentDescriptor curioComponentDescriptor)
        {
            __result = curioComponentDescriptor.Clone()!;
            return false;
        }

        
        return true;
    }
}