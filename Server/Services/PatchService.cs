using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Spt.Mod;

namespace CuriosServer.Services;

[Injectable(TypePriority = OnLoadOrder.PostLoad + 30)]
public class PatchService(ClientEnumDefinitions clientEnumDefinitions) : IOnLoad
{
    public Task OnLoadAsync(CancellationToken cancellationToken)
    {
        clientEnumDefinitions.Add("com.minesettimi.curios",
        new EnumEntryDefinition
        {
            EnumType = "EFT.EItemType",
            ConstantName = "Curios",
            ConstantValue = 20
        });
        
        return Task.CompletedTask;
    }
}