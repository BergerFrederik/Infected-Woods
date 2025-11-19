using UnityEngine;

public class PlayerStats : MonoBehaviour
{

    [Header("Primary Stats")]
    public float playerMaxHP = 0f;  
    public float playerHPRegeneration = 0f;
    public float playerLifeSteal = 0f;
    public float playerDamage = 0f;
    public float playerMeeleDamage = 0f;
    public float playerRangedDamage = 0f;
    public float playerMysticDamage = 0f;
    public float playerAttackSpeed = 0f;
    public float playerCritChance = 0f;
    public float playerAttackRange = 0f;
    public float playerArmor = 0f;
    public float playerDodge = 0f;
    public float playerMovespeed = 0f;
    public float playerLuck = 0f;
    public float playerCooldown = 0f;
    public float playerLevel = 0f;


    [Header("Secondary Stats")]
    public float playerKnockback = 0f;
    public float playerLightAbsorption = 0f;
    public float playerDashCooldownReduction = 0f;
    public float playerAbilityCooldown = 0f;



    [Header("Helper Stats")]
    public float playerLastLifesteal = 0f;
    public bool playerAbilityOnCooldown = false;
    public float playerCurrentHP = 0f;
    public float playerBaseMovespeed = 0f;
    public float playerLightAmount = 0f;
    public float playerOverallXP = 0f;
    public float playerCurrentXP = 0f;   
    public float playerBaseXP = 0f;
    public float playerLevelMultiplier = 0f;
    public float playerAbilityDuration = 0f;
    public float playerLevelsGained = 0f;
}
