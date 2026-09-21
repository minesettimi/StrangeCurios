using System.Text.Json.Serialization;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Enums;

namespace CuriosServer.Models;

public record CuriosTemplateProperties : TemplateItemProperties
{
    [JsonPropertyName("Curse")] public float? Curse { get; set; } = 1.0f;
    [JsonPropertyName("HealthEffects")] public Dictionary<HealthFactor, int>? HealthEffects;
    [JsonPropertyName("DamageReduction")] public float? DamageReduction { get; set; }
    [JsonPropertyName("PenResistance")] public int? PenResistance { get; set; }
    [JsonPropertyName("JumpBuff")] public float? JumpBuff { get; set; }
    [JsonPropertyName("StaminaRateBuff")] public float? StaminaRate { get; set; }
    [JsonPropertyName("StaminaMaxBuff")] public float? StaminaMax { get; set; }
    [JsonPropertyName("EquipmentRepair")] public Dictionary<EquipmentSlots, float>? EquipmentRepair { get; set; }
    [JsonPropertyName("SpecialEffect")] public CurioSpecialEffects SpecialEffect { get; set; } = CurioSpecialEffects.None;
    [JsonPropertyName("SkillIncreases")] public Dictionary<SkillTypes, int>? SkillIncreases;
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CurioSpecialEffects
{
    None,
    HeadshotProt,
    ExfilTp,
    DoorBreaker,
    Reflect,
    NewLife,
    Lucky
}