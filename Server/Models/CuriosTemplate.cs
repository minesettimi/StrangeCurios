using System.Text.Json.Serialization;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Enums;

namespace CuriosServer.Models;

public record CuriosTemplateProperties : TemplateItemProperties
{
    [JsonPropertyName("Curse")] public float? Curse { get; set; } = 1.0f;
    [JsonPropertyName("HealthEffects")] public Dictionary<HealthFactor, EffectsHealthProperties>? HealthEffects;
    [JsonPropertyName("DamageEffects")] public List<DamageEffectType>? DamageEffects;
    [JsonPropertyName("DamageReduction")] public float? DamageReduction { get; set; }
    [JsonPropertyName("PenResistance")] public int? PenResistance { get; set; } = 0;
    [JsonPropertyName("EquipmentRepair")] public float? EquipmentRepair { get; set; } = 0f;
    [JsonPropertyName("EquipmentTargets")] public List<EquipmentSlots> EquipmentTargets { get; set; } = [];
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
    Reflect
}