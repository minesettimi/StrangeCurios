using System.Text.Json.Serialization;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Spt.Mod;

namespace CuriosServer.Models;

public record CurioFromClone : NewItemFromCloneDetails
{
    [JsonPropertyName("newOverrideProperties")]
    public CuriosTemplateProperties? NewOverrideProperties { get; set; }
}

public record CurioTemplateItem : TemplateItem
{
    [JsonPropertyName("_customProps")] public CuriosTemplateProperties CustomProperties { get; set; }
}