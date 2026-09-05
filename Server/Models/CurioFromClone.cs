using System.Text.Json.Serialization;
using SPTarkov.Server.Core.Models.Spt.Mod;

namespace CuriosServer.Models;

public record CurioFromClone : NewItemFromCloneDetails
{
    [JsonPropertyName("newOverrideProperties")]
    public CuriosTemplateProperties? NewOverrideProperties { get; set; }
}