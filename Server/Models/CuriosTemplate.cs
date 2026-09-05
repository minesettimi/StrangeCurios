using System.Text.Json.Serialization;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Enums;

namespace CuriosServer.Models;

public record CuriosTemplateProperties : TemplateItemProperties
{
    [JsonPropertyName("curse")] public float Curse { get; set; } = 1.0f;
    [JsonPropertyName("energy")] public float EnergyChange { get; set; } = 0.0f;
    [JsonPropertyName("hydration")] public float HydrationChange { get; set; } = 0.0f;
    [JsonPropertyName("heal")] public float HealChange { get; set; } = 0.0f;
    [JsonPropertyName("damageReduction")] public float DamageReduction { get; set; } = 0.0f;
    [JsonPropertyName("penResistance")] public int PenResistance { get; set; } = 0;
    [JsonPropertyName("equipmentRepair")] public float EquipmentRepair { get; set; } = 0f;
    [JsonPropertyName("equipmentTargets")] public List<EquipmentSlots> EquipmentTargets { get; set; } = [];
    [JsonPropertyName("specialEffect")] public CurioSpecialEffects SpecialEffect { get; set; } = CurioSpecialEffects.None;
    [JsonPropertyName("skillIncreases")] public Dictionary<SkillTypes, int>? SkillIncreases;
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