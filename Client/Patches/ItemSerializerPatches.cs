using System.Reflection;
using CuriosClient.Components;
using CuriosClient.Models;
using EFT;
using EFT.InventoryLogic;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace CuriosClient.Patches;

public class ItemDeserializerPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(ItemBinarySerializer), nameof(ItemBinarySerializer.DeserializeComponent));
    }

    [PatchPrefix]
    public static bool Prefix(ItemComponentDescriptor descriptor, Item item)
    {
        if (descriptor is CurioComponentDescriptor curioDescriptor)
        {
            item.GetItemComponent<CurioComponent>()!.NumberOfUsages = curioDescriptor.NumberOfUsages;
            return false;
        }

        return true;
    }
}

public class ItemSerializerPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(ItemBinarySerializer), nameof(ItemBinarySerializer.SerializeComponent));
    }

    [PatchPrefix]
    public static bool Prefix(IItemComponent component, ref ItemComponentDescriptor __result)
    {
        if (component is CurioComponent curioComponent)
        {
            __result = new CurioComponentDescriptor
            {
                NumberOfUsages = curioComponent.NumberOfUsages
            };
            
            return false;
        }
        
        return true;
    }
}