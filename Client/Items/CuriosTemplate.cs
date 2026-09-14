using System.Collections.Generic;
using EFT;
using EFT.HealthSystem;
using EFT.InventoryLogic;
using Newtonsoft.Json;

namespace CuriosClient.Models;

public class CuriosTemplate : ItemTemplate
{
    [JsonProperty("MaxNumberOfUsage")] public int MaxUses { get; set; }
    [JsonProperty("Curse")] public float Curse { get; set; } = 1.0f;
    [JsonProperty("HealthEffects")] public Dictionary<EHealthFactorType, float> HealthEffects = [];
    [JsonProperty("DamageReduction")] public float? DamageReduction { get; set; }
    [JsonProperty("PenResistance")] public int? PenResistance { get; set; }
    [JsonProperty("EquipmentRepair")] public float? EquipmentRepair { get; set; }
    [JsonProperty("JumpBuff")] public float? JumpBuff { get; set; }
    [JsonProperty("StaminaRateBuff")] public float? StaminaRate { get; set; }
    [JsonProperty("StaminaMaxBuff")] public float? StaminaMax { get; set; }
    [JsonProperty("EquipmentTargets")] public List<EquipmentSlot>? EquipmentTargets { get; set; }
    [JsonProperty("SpecialEffect")] public CurioSpecialEffects SpecialEffect { get; set; } = CurioSpecialEffects.None;
    [JsonProperty("SkillIncreases")] public Dictionary<ESkillId, int>? SkillIncreases;
}

public enum CurioSpecialEffects
{
    None,
    HeadshotProt,
    ExfilTp,
    DoorBreaker,
    Reflect,
    NewLife
}

public enum CurioAttributes
{
    Curse,
    DamageReduction,
    PenResistance,
    EquipmentRepair,
    SpecialEffect,
    JumpBuff,
    StaminaMax,
    StaminaRate
}