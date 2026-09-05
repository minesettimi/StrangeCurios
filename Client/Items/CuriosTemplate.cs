using System.Collections.Generic;
using EFT;
using EFT.InventoryLogic;
using Newtonsoft.Json;

namespace CuriosClient.Models;

public class CuriosTemplate : ItemTemplate
{
    [JsonProperty("curse")] public float Curse { get; set; }
    [JsonProperty("energy")] public float EnergyChange { get; set; }
    [JsonProperty("hydration")] public float HydrationChange { get; set; }
    [JsonProperty("heal")] public float HealChange { get; set; }
    [JsonProperty("damageReduction")] public float DamageReduction { get; set; }
    [JsonProperty("penResistance")] public int PenResistance { get; set; }
    [JsonProperty("equipmentRepair")] public float EquipmentRepair { get; set; }
    [JsonProperty("equipmentTargets")] public List<EquipmentSlot> EquipmentTargets { get; set; } = [];
    [JsonProperty("specialEffect")] public CurioSpecialEffects SpecialEffect { get; set; } = CurioSpecialEffects.None;
    [JsonProperty("skillIncreases")] public Dictionary<ESkillId, int>? SkillIncreases;
}

public enum CurioSpecialEffects
{
    None,
    HeadshotProt,
    ExfilTp,
    DoorBreaker,
    Reflect
}