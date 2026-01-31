using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterStats : MonoBehaviour
{
    [Header("Information")]
    public string startWeaponID;
    
    [Header("Stats")]
    public float characterMaxHP = 0f;
    public float characterMaxMP = 0f;
    public float characterHPRegeneration = 0f;
    public float characterArmor = 0f;
    public float characterLifeSteal = 0f;
    public float characterDamage = 0f;
    public float characterMeeleDamage = 0f;
    public float characterRangedDamage = 0f;
    public float characterMysticDamage = 0f;
    public float characterAttackSpeed = 0f;
    public float characterKnockback = 0f;
    public float characterCooldown = 0f;
    public float characterCritChance = 0f;
    public float characterAttackRange = 0f;
    public float characterDodge = 0f;
    public float characterBaseMovespeed = 0f;
    public float characterLuck = 0f;
    public float characterLightAbsorption = 0f;

    [Header("Ability")]
    public float ability_cooldown = 5f;
    public float ability_duration = 10f;
    public float ability_manaCost = 25f;
    public string abilityDescription;

    [Header("Passive")] 
    public string[] characterPassiveEffects;

    private GameInput gameInput;
    private PlayerStats playerStats;

    public float actualMaxCooldown;
    public float remainingCooldown;
    public bool abilityReady = true;
    public bool cooldownStarted;
    
    private float _reducedCooldown;
    private float _clampedCooldown;

    public event Action OnExecuteAbility;
    public event Action OnStopAbility;
    
    private void Awake()
    {
        if (gameInput == null)
        {
            gameInput = FindFirstObjectByType<GameInput>();
        }
    }
    
    private void Start()
    {
        gameInput.OnAbilityStarted += StartAbility;
        gameInput.OnAbilityCanceled += EndAbility;
        
        playerStats = this.transform.GetComponentInParent<PlayerStats>();

        playerStats.playerMaxHP = characterMaxHP;
        playerStats.playerMaxMP = characterMaxMP;
        playerStats.playerHPRegeneration = characterHPRegeneration;
        playerStats.playerArmor = characterArmor;
        playerStats.playerLifeSteal = characterLifeSteal;
        playerStats.playerDamage = characterDamage;
        playerStats.playerMeeleDamage = characterMeeleDamage;
        playerStats.playerRangedDamage = characterRangedDamage;
        playerStats.playerMysticDamage = characterMysticDamage;
        playerStats.playerAttackSpeed = characterAttackSpeed;
        playerStats.playerKnockback = characterKnockback;
        playerStats.playerCooldown = characterCooldown;
        playerStats.playerCritChance = characterCritChance;
        playerStats.playerAttackRange = characterAttackRange;
        playerStats.playerDodge = characterDodge;
        playerStats.playerMovespeed = characterBaseMovespeed;
        playerStats.playerLuck = characterLuck;
        playerStats.playerLightPickupRange = characterLightAbsorption;

        playerStats.playerAbilityCooldown = ability_cooldown;
        playerStats.playerAbilityDuration = ability_duration;

        playerStats.playerCurrentHP = characterMaxHP;
        playerStats.playerCurrentMP = characterMaxMP;
    }

    private void OnDestroy()
    {
        gameInput.OnAbilityStarted -= StartAbility;
        gameInput.OnAbilityCanceled -= EndAbility;
    }

    private void Update()
    {
        HandleCooldown();
    }

    private void HandleCooldown()
    {
        if (cooldownStarted)
        {
            _reducedCooldown = ability_cooldown * (1f - playerStats.playerCooldown / 100f);
            _clampedCooldown = Mathf.Clamp(_reducedCooldown, 0.01f, ability_cooldown);
            remainingCooldown = _clampedCooldown;
            actualMaxCooldown = _clampedCooldown;
            cooldownStarted = false;
        }
        if (!abilityReady)
        {
            remainingCooldown -= Time.deltaTime;                      
            if (remainingCooldown <= 0)
            {
                abilityReady = true;
            }
        }
    }

    private void StartAbility()
    {
        OnExecuteAbility?.Invoke();
    }

    private void EndAbility()
    {
        OnStopAbility?.Invoke();
    }
}