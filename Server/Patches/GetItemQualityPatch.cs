using System.Reflection;
using CuriosServer.Models;
using SPTarkov.Common.Extensions;
using SPTarkov.DI.Annotations;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.Helpers.Items;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;

namespace CuriosServer.Patches;

[Injectable]
public class GetItemQualityPatch : AbstractPatch
{
    private static ItemHelper _itemHelper = null!;

    public GetItemQualityPatch(ItemHelper itemHelper)
    {
        _itemHelper = itemHelper;
    }
    
    protected override MethodBase? GetTargetMethod()
    {
        return typeof(ItemHelper).GetMethod(nameof(ItemHelper.GetItemQualityModifier));
    }

    [PatchPrefix]
    public static bool Prefix(Item item, ref double __result)
    {
        if (item.Upd is null || !item.TryGetExtensionData(out Dictionary<string, object>? extensionData) || 
            !extensionData!.TryGetValue("Curio", out object? curioData))
            return true;

        UpdCurio curioUpd = (curioData as UpdCurio)!;
        
        TemplateItem? itemDetails = _itemHelper.GetItem(item.Template).Value;
        if (itemDetails?.Properties is null)
        {
            return true;
        }

        double maxUsages = itemDetails.Properties.MaximumNumberOfUsage.GetValueOrDefault(0);
        __result = (maxUsages - curioUpd.NumberOfUsages) / maxUsages;
        return false;
    }
}