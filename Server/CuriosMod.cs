using System.Reflection;
using CuriosServer.Models;
using SPTarkov.Common.Models.Logging;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
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
    ISptLogger<CuriosMod> logger) : IOnLoad
{
    public static readonly Assembly Assembly = Assembly.GetExecutingAssembly();
    public static readonly string ModPath = Path.GetDirectoryName(Assembly.Location)!;
    public static readonly string DataPath = Path.Join(ModPath, "Data");
    
    public async Task OnLoadAsync(CancellationToken cancellationToken)
    {
        List<HandbookCategory>? handbookCategories = await jsonUtil.DeserializeFromFileAsync<List<HandbookCategory>>(
            Path.Join(DataPath, "handbook.json"), cancellationToken);

        if (handbookCategories == null)
        {
            throw new Exception("[Curios] Failed to load handbook.json!");
        }
        
        templateTable.Handbook.Categories.AddRange(handbookCategories);
        
        await itemParentService.CreateCustomParents(Assembly, "db/Parents");
        await itemServiceExtended.CreateCustomItems(Assembly, "db/Items");
        await localeService.CreateCustomLocales(Assembly, "db/Locales");
    }
}