using UnityEngine;

public class WeaponStats : MonoBehaviour
{
    [SerializeField] private WeaponData weaponData;
    
    public enum weaponTypeOptions
    {
        Melee,
        Ranged,
        Ability
    }
    
    [HideInInspector] public float weaponLevel;
    [HideInInspector] public float currentTotalDamage = 0f;
    
    [Header("Information")]
    public string weaponName;
    public string weaponSubtitle;
    public float weaponTier = 0f;
    public weaponTypeOptions weaponWeaponType;
    public string weaponAttackType;
    public float weaponClass = 0f;
    public float weaponPrice = 0f;
    public string weaponSpecialAbility;
    public string weaponLore;
    
    [Header("Stats")]
    public float weaponProjectileSpeed = 0f;
    public float weaponBaseDamage = 0f;
    public float weaponMeleeDamageScale = 0f;
    public float weaponRangedDamageScale = 0f;
    public float weaponMysticDamageScale = 0f;
    public float weaponAttackSpeedCooldown = 0f; // in seconds
    public float weaponCritChance = 0f;
    public float weaponCritDamage = 0f;
    public float weaponRange = 0f;
    public float weaponKnockback = 0f;
    public float weaponLifesteal = 0f;
    
    
    public void CopyFrom(WeaponStats other)
    {
        if (other == null) return;

        this.weaponName = other.weaponName;
        this.weaponSubtitle = other.weaponSubtitle;
        this.weaponProjectileSpeed = other.weaponProjectileSpeed;
        this.weaponBaseDamage = other.weaponBaseDamage;
        this.weaponMeleeDamageScale = other.weaponMeleeDamageScale;
        this.weaponRangedDamageScale = other.weaponRangedDamageScale;
        this.weaponMysticDamageScale = other.weaponMysticDamageScale;
        this.weaponTier = other.weaponTier;
        this.weaponWeaponType = other.weaponWeaponType;
        this.weaponAttackType = other.weaponAttackType;
        this.weaponClass = other.weaponClass;
        this.weaponAttackSpeedCooldown = other.weaponAttackSpeedCooldown;
        this.weaponCritChance = other.weaponCritChance;
        this.weaponCritDamage = other.weaponCritDamage;
        this.weaponRange = other.weaponRange;
        this.weaponKnockback = other.weaponKnockback;
        this.weaponLifesteal = other.weaponLifesteal;
        this.weaponPrice = other.weaponPrice;
        this.currentTotalDamage = other.currentTotalDamage;
        this.weaponSpecialAbility = other.weaponSpecialAbility;
        this.weaponLore = other.weaponLore;
    }

    public void ApplyStats()
    {
        var stats = weaponData.levels[(int)weaponLevel];
        
        this.weaponProjectileSpeed = stats.weaponProjectileSpeed;
        this.weaponBaseDamage = stats.weaponBaseDamage;
        this.weaponMeleeDamageScale = stats.weaponMeeleDamageScale;
        this.weaponRangedDamageScale = stats.weaponRangedDamageScale;
        this.weaponMysticDamageScale = stats.weaponMysticDamageScale;
        this.weaponAttackSpeedCooldown = stats.weaponAttackSpeedCooldown;
        this.weaponCritChance = stats.weaponCritChance;
        this.weaponCritDamage = stats.weaponCritDamage;
        this.weaponRange = stats.weaponRange;
        this.weaponKnockback = stats.weaponKnockback;
        this.weaponLifesteal = stats.weaponLifesteal;
    }
}