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
    [JsonProperty("cumulativeDamageResist")] public bool CumulativeDamage { get; set; }
    [JsonProperty("cumulativeJumpHeight")] public bool CumulativeJump { get; set; }
    [JsonProperty("cumulativeStamina")] public bool CumulativeStamina { get; set; }
    [JsonProperty("energyLoopTime")] public int EnergyLoopTime { get; set; }
    [JsonProperty("healthLoopTime")] public int HealthLoopTime { get; set; }
    [JsonProperty("hydrationLoopTime")] public int HydrationLoopTime { get; set; }
    [JsonProperty("temperatureLoopTime")] public int TemperatureLoopTime { get; set; }
    [JsonProperty("armorLoopTime")] public int ArmorLoopTime { get; set; }
    [JsonProperty("reflectMult")] public float ReflectMult { get; set; }
    [JsonProperty("reflectDmgMult")] public float ReflectDmgMult { get; set; }
    [JsonProperty("penResistMult")] public float PenMult { get; set; }
    [JsonProperty("unkillableLength")] public float UnkillableLength { get; set; }
}

public record CurseConfig
{
    [JsonProperty("minCurseRunThrough")] public int CurseRunThrough { get; set; }
    [JsonProperty("maxCurseXpMult")] public float CurseMaxXp { get; set; }
    [JsonProperty("maxCurseXpNum")] public int CurseXpCount { get; set; }
    [JsonProperty("maxCurseXpNumMin")] public int CurseXpCountMin { get; set; }
    [JsonProperty("maxCurseSkillMult")] public float CurseMaxSkill { get; set; }
    [JsonProperty("maxCurseSkillNum")] public int CurseSkillCount { get; set; }
    [JsonProperty("maxCurseSkillNumMin")] public int CurseSkillCountMin { get; set; }
    [JsonProperty("curseExtraKeyUse")] public int CurseKeys { get; set; }
    [JsonProperty("curseExtraKeyUseMin")] public int CurseKeysMin { get; set; }
    [JsonProperty("curseExtraMedsUse")] public int CurseMeds { get; set; }
    [JsonProperty("curseExtraMedsUseMin")] public int CurseMedsMin { get; set; }
}