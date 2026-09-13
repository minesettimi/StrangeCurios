using Newtonsoft.Json;

namespace CuriosClient.Models;

public record CurioConfig
{
    [JsonProperty("effects")] public EffectConfig EffectConfig { get; set; } = null!;
    [JsonProperty("curse")] public CurseConfig CurseConfig { get; set; } = null!;
}

public record EffectConfig
{
    [JsonProperty("cumulativePenResist")] public bool CumulativePen { get; set; }
    [JsonProperty("cumulativeJumpHeight")] public bool CumulativeJump { get; set; }
    [JsonProperty("energyLoopTime")] public int EnergyLoopTime { get; set; }
    [JsonProperty("healthLoopTime")] public int HealthLoopTime { get; set; }
    [JsonProperty("hydrationLoopTime")] public int HydrationLoopTime { get; set; }
    [JsonProperty("temperatureLoopTime")] public int TemperatureLoopTime { get; set; }
    [JsonProperty("armorLoopTime")] public int ArmorLoopTime { get; set; }
}

public record CurseConfig
{
    [JsonProperty("minCurseRunThrough")] public int CurseRunThrough { get; set; }
    [JsonProperty("maxCurseXpMult")] public float CurseMaxXp { get; set; }
    [JsonProperty("maxCurseMedFailRate")] public int CurseFailRate { get; set; }
    [JsonProperty("maxCurseMedFailNum")] public int CurseFailCount { get; set; }
    [JsonProperty("maxCurseXpNum")] public int CurseXpCount { get; set; }
    [JsonProperty("cursePerExtraKeyUse")] public int CurseKeys { get; set; }
}