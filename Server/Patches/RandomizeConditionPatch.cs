using System.Reflection;
using CuriosServer.Models;
using SPTarkov.Common.Extensions;
using SPTarkov.DI.Annotations;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.Generators.Ragfair;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Utils;

namespace CuriosServer.Patches;

[Injectable]
public class RandomizeConditionPatch : AbstractPatch
{
    private static RagfairConfig _ragfairConfig = null!;
    private static RandomUtil _randomUtil = null!;

    public RandomizeConditionPatch(RagfairConfig ragfairConfig, RandomUtil randomUtil)
    {
        _ragfairConfig = ragfairConfig;
        _randomUtil = randomUtil;
    }
    
    protected override MethodBase? GetTargetMethod()
    {
        return typeof(RagfairOfferGenerator).GetMethod("RandomiseItemCondition", BindingFlags.Instance |
            BindingFlags.NonPublic);
    }

    [PatchPrefix]
    public static bool Prefix(MongoId conditionSettingsId, TemplateItem itemDetails, IEnumerable<Item> itemWithMods)
    {
        Item rootItem = itemWithMods.First();
        
        Condition itemConditionValues =_ragfairConfig.Dynamic.Condition[conditionSettingsId];
        double maxMultiplier = _randomUtil.GetDouble(itemConditionValues.Max.Min, itemConditionValues.Max.Min);

        if (!rootItem.TryGetExtensionData(out Dictionary<string, object>? extensionData) ||
            extensionData == null ||
            extensionData!.TryGetValue("Curio", out object? curioObj))
        {
            return true;
        }

        if (curioObj is not UpdCurio curioUpd)
            return true;
        
        curioUpd.NumberOfUsages = (int)Math.Round(itemDetails.Properties?.MaximumNumberOfUsage ?? 0 * (1 - maxMultiplier));

        return false;
    }
}