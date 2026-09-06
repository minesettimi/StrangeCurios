using System.Text.Json.Serialization;

namespace CuriosServer.Models;

public record UpdCurio
{
    [JsonPropertyName("NumberOfUsages")]
    public int NumberOfUsages;
}