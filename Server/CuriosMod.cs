using System.Reflection;
using SPTarkov.Common.Models.Logging;
using SPTarkov.DI.Annotations;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Models.Spt.Tables;
using SPTarkov.Server.Core.Utils;
using WTTServerCommonLib.Services;
using Path = System.IO.Path;

namespace CuriosServer.Services;

[Injectable(TypePriority = OnLoadOrder.Preload + 25)]
public class CuriosMod(JsonUtil jsonUtil,
    TemplateTable templateTable,
    WTTCustomItemParentService itemParentService,
    WTTCustomItemServiceExtended itemServiceExtended,
    WTTCustomLocaleService localeService,
    WTTCustomLootspawnService lootspawnService,
    IEnumerable<IRuntimePatch> patches,
    RagfairConfig ragfairConfig) : IOnLoad
{
    public static readonly Assembly Assembly = Assembly.GetExecutingAssembly();
    public static readonly string ModPath = Path.GetDirectoryName(Assembly.Location)!;
    public static readonly string DataPath = Path.Join(ModPath, "Data");
    public static readonly string ConfigPath = Path.Join(DataPath, "config.json");
    
    public async Task OnLoadAsync(CancellationToken cancellationToken)
    {
        List<HandbookCategory>? handbookCategories = await jsonUtil.DeserializeFromFileAsync<List<HandbookCategory>>(
            Path.Join(DataPath, "handbook.json"), cancellationToken);

        if (handbookCategories == null)
        {
            throw new Exception("[Curios] Failed to load handbook.json!");
        }
        
        templateTable.Handbook.Categories.AddRange(handbookCategories);

        ragfairConfig.Dynamic.ShowAsSingleStack.Add(new MongoId("6a9b52c15dfaf97d11fc341e"));
     
        foreach (IRuntimePatch patch in patches)
            patch.Enable();
        
        await itemParentService.CreateCustomParents(Assembly, "db/Parents");
        await itemServiceExtended.CreateCustomItems(Assembly, "db/Items");
        await localeService.CreateCustomLocales(Assembly, "db/Locales");
        await lootspawnService.CreateCustomLootSpawns(Assembly, "db/LooseLoot");
    }
}