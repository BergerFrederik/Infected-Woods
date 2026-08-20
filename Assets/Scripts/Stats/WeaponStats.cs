using System;
using UnityEngine;

public class WeaponStats : MonoBehaviour
{
    public event Action<float> OnWeaponLevelChanged;
    
    [SerializeField] private WeaponData weaponData;
    
    public enum weaponTypeOptions
    {
        Melee,
        Ranged,
        Ability
    }

    public enum WeaponAttackTypes
    {
        Stab,
        Ranged,
        Boomerang,
        Dot
    }

    private float _weaponLevel;

    [HideInInspector]
    public float WeaponLevel
    {
        get => _weaponLevel;
        set
        {
            _weaponLevel = value;
            OnWeaponLevelChanged?.Invoke(value);
        }
    }//starts at 0
    
    [HideInInspector] public float currentTotalDamage = 0f;
    
    [Header("Information")]
    public string weaponName;
    public string weaponID;
    public string weaponSubtitle;
    public WeaponTier weaponTier = WeaponTier.Root;
    public weaponTypeOptions weaponWeaponType;
    public WeaponAttackTypes weaponAttackType;
    public float weaponClass = 0f;
    public float weaponPrice = 0f;
    
    [TextArea(3, 5)]
    public string passiveDescription;
    
    [TextArea(3, 5)]
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
    private float _dps;

    public void CopyFrom(WeaponStats other)
    {
        if (other == null) return;

        this.weaponName = other.weaponName;
        this.weaponID = other.weaponID;
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
        this.passiveDescription = other.passiveDescription;
        this.weaponLore = other.weaponLore;
    }

    public void ApplyStats()
    {
        var stats = weaponData.levels[(int)WeaponLevel];
        
        this.weaponProjectileSpeed = stats.weaponProjectileSpeed;
        this.weaponBaseDamage = stats.weaponBaseDamage;
        this.weaponMeleeDamageScale = stats.weaponMeleeDamageScale;
        this.weaponRangedDamageScale = stats.weaponRangedDamageScale;
        this.weaponMysticDamageScale = stats.weaponMysticDamageScale;
        this.weaponAttackSpeedCooldown = stats.weaponAttackSpeedCooldown;
        this.weaponCritChance = stats.weaponCritChance;
        this.weaponCritDamage = stats.weaponCritDamage;
        this.weaponRange = stats.weaponRange;
        this.weaponKnockback = stats.weaponKnockback;
        this.weaponLifesteal = stats.weaponLifesteal;
    }
    
    public string GetStatsAsText()
    {
        string formattedStats = "";
    
        if (weaponBaseDamage > 0) 
            formattedStats += $"Base Damage: {weaponBaseDamage}\n";

        if (weaponWeaponType == weaponTypeOptions.Melee)
            formattedStats += $"Melee Scaling: {weaponMeleeDamageScale}%\n";
        else if (weaponWeaponType == weaponTypeOptions.Ranged)
            formattedStats += $"Ranged Scaling: {weaponRangedDamageScale}%\n";

        if (weaponAttackSpeedCooldown > 0)
            formattedStats += $"Attack Speed: {weaponAttackSpeedCooldown}s\n";

        if (weaponCritChance > 0)
            formattedStats += $"Crit Chance: {weaponCritChance}%\n";
        
        if (weaponCritDamage > 0)
            formattedStats += $"Crit Damage: {weaponCritDamage}%\n";

        if (weaponRange > 0)
            formattedStats += $"Range: {weaponRange}\n";

        if (weaponLifesteal > 0)
            formattedStats += $"Lifesteal: {weaponLifesteal}%\n";

        formattedStats += $"Dps: {GetCurrentDps():F1}\n";

        return formattedStats;
    }

    private float GetCurrentDps()
    {
        PlayerStats playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        float playerPercentDamage = playerStats.playerDamage;
        float playerMeleeDamage = playerStats.playerMeleeDamage;
        float playerRangedDamage = playerStats.playerRangedDamage;
        float playerMysticDamage = playerStats.playerMysticDamage;

        float increaseByMeleeScaling = playerMeleeDamage * (weaponMeleeDamageScale / 100f);
        float increaseByRangedScaling = playerRangedDamage * (weaponRangedDamageScale / 100f);
        float increaseByMysticScaling = playerMysticDamage * (weaponMysticDamageScale / 100f);

        float newWeaponBaseDamage = weaponBaseDamage + increaseByMeleeScaling + increaseByMysticScaling + increaseByRangedScaling;

        float increaseByPlayerDamage = (playerPercentDamage / 100f) * newWeaponBaseDamage;

        float normalDamage = newWeaponBaseDamage + increaseByPlayerDamage;

        float critWeaponDamage = normalDamage * weaponCritDamage;
        float critDamage = critWeaponDamage + (critWeaponDamage * playerStats.PlayerCritDamage / 100f);

        float critChance = Mathf.Clamp01((playerStats.PlayerCritChance + weaponCritChance) / 100f);
        float averageDamage = normalDamage * (1f - critChance) + critDamage * critChance;

        return averageDamage / weaponAttackSpeedCooldown;
    }
}