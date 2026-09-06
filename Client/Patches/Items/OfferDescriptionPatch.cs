using System.Reflection;
using CuriosClient.Components;
using EFT.InventoryLogic;
using EFT.UI.Ragfair;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace CuriosClient.Patches;

public class OfferDescriptionPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(OfferItemDescription), nameof(OfferItemDescription.SetItemName));
    }

    [PatchPostfix]
    public static void Postfix(OfferItemDescription __instance, Offer ____offer)
    {
        Item item = ____offer.Item;

        if (item.TryGetItemComponent(out CurioComponent curioComponent))
        {
            int uses = curioComponent.Template.MaxUses - curioComponent.NumberOfUsages;
            string color = curioComponent.RelativeValue > .1f ? "#d80000" : "#9AA3A8";
            __instance.AddCounter(EItemAttributeId.Durability, uses, curioComponent.Template.MaxUses,
                color);
        }
    }
}