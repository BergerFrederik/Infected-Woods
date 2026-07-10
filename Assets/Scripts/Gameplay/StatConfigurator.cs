using System.Collections.Generic;
using UnityEngine;

public class StatConfigurator : MonoBehaviour
{
    public enum StatType
    {
        MaxHp,
        HpRegeneration,
        Lifesteal,
        Damage,
        MeleeDamage, 
        RangedDamage,
        MysticDamage,
        AttackSpeed,
        Crit,
        Range,
        Armor,
        Dodge,
        MoveSpeed,
        Luck,
        Cooldown,
        MaxMp,
        MpRegeneration
    }

    public enum Identifier
    {
        RootAndBud,
        Blossom
    }

    [System.Serializable]
    public class Stat
    {
        [SerializeField] private StatType statType; 
        [SerializeField] private float minValue;
        [SerializeField] private float maxValue;

        public float GetRandomValue() => Random.Range(minValue, maxValue);
        public float GetMaxValue() => maxValue;
        public float GetMinValue() => minValue;
        public string GetStatName() => ToStatNameString(statType);
    }
    
    public List<Stat> allStats;

    public static string ToStatNameString(StatType type)
    {
        return type switch
        {
            StatType.MaxHp => "Max HP",
            StatType.HpRegeneration => "HP Regeneration",
            StatType.Lifesteal => "Lifesteal",
            StatType.Damage => "Damage",
            StatType.MeleeDamage => "Melee Damage", 
            StatType.RangedDamage => "Ranged Damage",
            StatType.MysticDamage => "Mystic Damage",
            StatType.AttackSpeed => "Attackspeed",
            StatType.Crit => "Crit",
            StatType.Range => "Range",
            StatType.Armor => "Armor",
            StatType.Dodge => "Dodge",
            StatType.MoveSpeed => "Movespeed",
            StatType.Luck => "Luck",
            StatType.Cooldown => "Cooldown",
            StatType.MaxMp => "Max MP",
            StatType.MpRegeneration => "MP Regeneration",
            _ => ""
        };
    }
}