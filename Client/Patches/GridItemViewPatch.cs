using System.Reflection;
using CuriosClient.Components;
using EFT.InventoryLogic;
using EFT.UI.DragAndDrop;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace CuriosClient.Patches;

public class GridItemViewPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(GridItemView), nameof(GridItemView.UpdateInfo));
    }

    [PatchPostfix]
    public static void Postfix(GridItemView __instance)
    {
        CurioComponent? curioComponent = __instance.Item.GetItemComponent<CurioComponent>();

        if (curioComponent == null || curioComponent.Template.MaxUses < 1)
            return;
        
        __instance.SetItemValue(GridItemView.EItemValueFormat.TwoValues, 
            __instance.Examined, 
            "#ff5335",
            curioComponent.UsesLeft,
            curioComponent.Template.MaxUses);
        
        __instance.SetValueVisibility(__instance.CurrentItemValue.Length > 0 && 
                                      __instance.ItemContext.HasViewComponent(EItemViewComponent.ResourceInfo));
    }
}