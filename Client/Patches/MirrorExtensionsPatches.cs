using System.Reflection;
using CuriosClient.Extensions;
using CuriosClient.Models;
using EFT;
using EFT.BinarySerialization;
using HarmonyLib;
using Mirror;
using SPT.Reflection.Patching;

namespace CuriosClient.Patches;

public class MirrorExtensionsPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(BinarySerializationMirrorExtensions),
            nameof(BinarySerializationMirrorExtensions.WritePolymorph), 
            generics: [typeof(ItemComponentDescriptor)]);
    }
    
    [PatchPrefix]
    public static bool Prefix(NetworkWriter writer, ItemComponentDescriptor target)
    {
        if (target is CurioComponentDescriptor curioComponentDescriptor)
        {
            writer.WriteCurioComponentDescriptor(curioComponentDescriptor);
            return false;
        }

        return true;
    }
}

public class MirrorExtensionReadPatch : ModulePatch
{
    public static int CurioIndex;
    
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(BinarySerializationMirrorExtensions),
            nameof(BinarySerializationMirrorExtensions.ReadPolymorph), 
            generics: [typeof(ItemComponentDescriptor)]);
    }

    [PatchPrefix]
    public static bool Prefix(NetworkReader reader, ref ItemComponentDescriptor __result)
    {
        byte b = reader.ReadByte();

        if (b == CurioIndex)
        {
            __result = reader.ReadCurioComponentDescriptor();
            return false;
        }
        
        return true;
    }
}