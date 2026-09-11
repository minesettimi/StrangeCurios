using System.ComponentModel;
using System.Text.Json.Serialization;

namespace CuriosServer.Models;

public record CurioConfig
{
    [JsonPropertyName("effects")] public EffectConfig EffectConfig { get; set; } = new();
    [JsonPropertyName("curse")] public CurseConfig CurseConfig { get; set; } = new();
}

public record EffectConfig
{
    [JsonPropertyName("cumulativePenResist")] public bool CumulativePen { get; set; } = false;
    [JsonPropertyName("cumulativeJumpHeight")] public bool CumulativeJump { get; set; } = false;
    [JsonPropertyName("energyLoopTime")] public int EnergyLoopTime { get; set; } = 60;
    [JsonPropertyName("healthLoopTime")] public int HealthLoopTime { get; set; } = 60;
    [JsonPropertyName("hydrationLoopTime")] public int HydrationLoopTime { get; set; } = 60;
    [JsonPropertyName("temperatureLoopTime")] public int TemperatureLoopTime { get; set; } = 60;
    [JsonPropertyName("armorLoopTime")] public int ArmorLoopTime { get; set; } = 45;
}

public record CurseConfig
{
    [JsonPropertyName("minCurseRunThrough")] public int CurseRunThrough { get; set; } = 100;
    [JsonPropertyName("maxCurseXpMult")] public float CurseMaxXp { get; set; } = 0.5f;
    [JsonPropertyName("maxCurseMedFailRate")] public int CurseFailRate { get; set; } = 50;
    [JsonPropertyName("maxCurseMedFailNum")] public int CurseFailCount { get; set; } = 100;
    [JsonPropertyName("maxCurseXpNum")] public int CurseXpCount { get; set; } = 100;
    [JsonPropertyName("cursePerExtraKeys")] public int CurseKeys { get; set; } = 50;
}