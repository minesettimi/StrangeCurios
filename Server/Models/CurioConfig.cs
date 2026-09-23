using System.Text.Json.Serialization;

namespace CuriosServer.Models;

public class CurioConfig
{
    [JsonPropertyName("effects")] public EffectConfig EffectConfig { get; set; } = new();
    [JsonPropertyName("curse")] public CurseConfig CurseConfig { get; set; } = new();
}

public class EffectConfig
{
    [JsonPropertyName("cumulativePenResist")] public bool CumulativePen { get; set; } = false;
    [JsonPropertyName("cumulativeDamageResist")] public bool CumulativeDamage { get; set; } = false;
    [JsonPropertyName("cumulativeJumpHeight")] public bool CumulativeJump { get; set; } = false;
    [JsonPropertyName("cumulativeStamina")] public bool CumulativeStamina { get; set; } = true;
    [JsonPropertyName("energyLoopTime")] public int EnergyLoopTime { get; set; } = 60;
    [JsonPropertyName("healthLoopTime")] public int HealthLoopTime { get; set; } = 60;
    [JsonPropertyName("hydrationLoopTime")] public int HydrationLoopTime { get; set; } = 60;
    [JsonPropertyName("temperatureLoopTime")] public int TemperatureLoopTime { get; set; } = 60;
    [JsonPropertyName("armorLoopTime")] public int ArmorLoopTime { get; set; } = 45;
    [JsonPropertyName("reflectMult")] public float ReflectMult { get; set; } = 0.5f;
    [JsonPropertyName("reflectDmgMult")] public float ReflectDmgMult { get; set; } = 4f;
    [JsonPropertyName("penResistMult")] public float PenMult { get; set; } = 0.25f;
    [JsonPropertyName("unkillableLength")] public float UnkillableLength { get; set; } = 5f;
}

public class CurseConfig
{
    [JsonPropertyName("minCurseRunThrough")] public int CurseRunThrough { get; set; } = 100;
    [JsonPropertyName("maxCurseXpMult")] public float CurseMaxXp { get; set; } = 0.5f;
    [JsonPropertyName("maxCurseXpNum")] public int CurseXpCount { get; set; } = 100;
    [JsonPropertyName("maxCurseXpNumMin")] public int CurseXpCountMin { get; set; } = 0;
    [JsonPropertyName("maxCurseSkillMult")] public float CurseMaxSkill { get; set; } = 0.4f;
    [JsonPropertyName("maxCurseSkillNum")] public int CurseSkillCount { get; set; } = 80;
    [JsonPropertyName("maxCurseSkillNumMin")] public int CurseSkillCountMin { get; set; } = 20;
    [JsonPropertyName("curseExtraKeyUse")] public int CurseKeys { get; set; } = 30;
    [JsonPropertyName("curseExtraKeyUseMin")] public int CurseKeysMin { get; set; } = 10;
    [JsonPropertyName("curseExtraMedsUse")] public int CurseMeds { get; set; } = 40;
    [JsonPropertyName("curseExtraMedsUseMin")] public int CurseMedsMin { get; set; } = 0;
}