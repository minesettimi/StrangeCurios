using System.Reflection;
using CuriosServer.Models;
using MonoMod.Utils;
using SPTarkov.Common.Models.Logging;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Spt.Tables;
using SPTarkov.Server.Core.Services.Items;
using SPTarkov.Server.Core.Services.Modding.Custom;
using SPTarkov.Server.Core.Utils;
using Path = System.IO.Path;

namespace CuriosServer.Services;

[Injectable(TypePriority = OnLoadOrder.Preload + 25)]
public class TemplateService(JsonUtil jsonUtil, 
    CustomItemService customItemService,
    TemplateTable templateTable,
    ItemBaseClassService itemBaseClassService,
    ISptLogger<TemplateService> logger) : IOnLoad
{
    public static readonly string ModPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
    public static readonly string DataPath = Path.Join(ModPath, "Data");
    
    public async Task OnLoadAsync(CancellationToken cancellationToken)
    {
        Dictionary<MongoId, CurioTemplateItem>? baseItems =
            await jsonUtil.DeserializeFromFileAsync<Dictionary<MongoId, CurioTemplateItem>>(
                Path.Join(DataPath, "baseItems.json"), cancellationToken);
        
        if (baseItems == null)
        {
            throw new Exception("[Curios] Failed to load baseItems.json!");
        }

        templateTable.Items["6a9b52c15dfaf97d11fc341e"] = new TemplateItem()
        {
            Id = "6a9b52c15dfaf97d11fc341e",
            Name = "Curios",
            Parent = "54009119af1c881c07000029",
            Properties = new CuriosTemplateProperties(),
            Type = "Node"
        };
        
        foreach ((MongoId id, CurioTemplateItem item) in baseItems)
        {
            item.Properties = item.CustomProperties;
            
            templateTable.Items.TryAdd(id, item);
            itemBaseClassService.AddItemToCache(id);
        }

        List<HandbookCategory>? handbookCategories = await jsonUtil.DeserializeFromFileAsync<List<HandbookCategory>>(
            Path.Join(DataPath, "handbook.json"), cancellationToken);

        if (handbookCategories == null)
        {
            throw new Exception("[Curios] Failed to load handbook.json!");
        }
        
        templateTable.Handbook.Categories.AddRange(handbookCategories);
        
        

        List<CurioFromClone>? cloneList =
            await jsonUtil.DeserializeFromFileAsync<List<CurioFromClone>>(Path.Join(DataPath, "cloneItems.json"), cancellationToken);

        if (cloneList == null)
        {
            throw new Exception("[Curios] Failed to load cloneItems.json!");
        }

        foreach (CurioFromClone clone in cloneList)
        {
            clone.OverrideProperties = clone.NewOverrideProperties;

            customItemService.CreateItemFromClone(clone);
        }
    }
}