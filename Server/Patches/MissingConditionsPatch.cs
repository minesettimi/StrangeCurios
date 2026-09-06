using System.Reflection;
using CuriosServer.Models;
using SPTarkov.Common.Extensions;
using SPTarkov.DI.Annotations;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.Generators.Ragfair;
using SPTarkov.Server.Core.Helpers.Items;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;

namespace CuriosServer.Patches;

[Injectable]
public class MissingConditionsPatch : AbstractPatch
{
    private static ItemHelper _itemHelper = null!;
    
    public MissingConditionsPatch(ItemHelper itemHelper)
    {
        _itemHelper = itemHelper;
    }
    
    protected override MethodBase? GetTargetMethod()
    {
        return typeof(RagfairOfferGenerator).GetMethod("AddMissingConditions",
            BindingFlags.NonPublic | BindingFlags.Instance);
    }

    [PatchPrefix]
    public static bool Prefix(Item item)
    {
        TemplateItemProperties? props = _itemHelper.GetItem(item.Template).Value!.Properties;

        if (props is not CuriosTemplateProperties)
            return true;

        item.Upd?.AddToExtensionData("Curio", new UpdCurio { NumberOfUsages = 0 });
        
        return false;
    }
}