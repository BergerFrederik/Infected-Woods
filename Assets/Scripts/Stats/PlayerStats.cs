using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public event Action OnMovespeedChanged;
    public event Action<float> OnCurrentMPChanged;
    public event Action<float> OnPlayerInfused;
    public event Action<float> OnMaxMPChanged;
    public event Action<float> OnCurrentHPChanged;
    public event Action<float> OnPlayerHealed;
    public event Action<float> OnMaxHPChanged;
    public event Action<float> OnLightPickupRangeChanged;
    public event Action<float> OnNumWeaponSlotsChanged;
    public event Action<float> OnCritChanceChanged;
    public event Action<float> OnPlayerArmorChanged;

    [Header("Primary Stats")]
    [SerializeField] private float _playerMaxHP;
    public float playerMaxHP
    {
        get { return _playerMaxHP; }
        set { _playerMaxHP = value;
            // Max HP can drop (e.g. selling an item) - current HP must not stay above it
            if (_playerCurrentHP > value) playerCurrentHP = value;
            OnMaxHPChanged?.Invoke(value);
        }
    }

    [SerializeField] private float _playerMaxMP;
    public float playerMaxMP
    {
        get => _playerMaxMP;
        set { _playerMaxMP = value;
            // Max MP can drop (e.g. selling an item) - current MP must not stay above it
            if (_playerCurrentMP > value) playerCurrentMP = value;
            OnMaxMPChanged?.Invoke(value);
        }
    }

    public float playerHPRegeneration = 0f;
    public float playerMPRegeneration = 0f;
    public float playerLifeSteal = 0f;
    public float playerDamage = 0f;
    public float playerMeleeDamage = 0f;
    public float playerRangedDamage = 0f;
    public float playerMysticDamage = 0f;
    public float playerAttackSpeed = 0f;

    [SerializeField] private float _playerCritChance;
    public float PlayerCritChance
    {
        get { return _playerCritChance; }
        set
        {
            _playerCritChance = value;
            OnCritChanceChanged?.Invoke(value);
        }
    }
    
    [SerializeField] private float _playerCritDamage;
    public float PlayerCritDamage
    {
        get { return _playerCritDamage; }
        set { _playerCritDamage = value;}
    }
    
    public float playerAttackRange = 0f;

    [SerializeField] private float _playerArmor;
    public float PlayerArmor
    {
        get => _playerArmor;
        set
        {
            _playerArmor = value;
            OnPlayerArmorChanged?.Invoke(value);
        }
    }
    public float playerDodge = 0f;
    public float playerMovespeed = 0f;
    public float playerLuck = 0f;
    public float playerCooldown = 0f;
    public float playerLevel = 0f;


    [Header("Secondary Stats")]
    public float playerKnockback = 0f;
    [SerializeField] private float _plyerLightPickupRange;

    public float playerLightPickupRange
    {
        get => _plyerLightPickupRange;
        set { _plyerLightPickupRange = value;            
            OnLightPickupRangeChanged?.Invoke(value);
        }
    }
    public float playerDashCooldownReduction = 0f;
    public float playerAbilityCooldown = 0f;
    public float playerHealPower = 0f;
    public float playerShieldPower = 0f;
    public float playerStackCapIncreasePercent = 0f; // raises the cap of every capped stackable, see StackCounter
    [SerializeField] private float _playerWeaponSlots;
    public float PlayerWeaponSlots
    {
        get => _playerWeaponSlots;
        set
        {
            _playerWeaponSlots = value;
            OnNumWeaponSlotsChanged?.Invoke(value);
        } 
    }



    [Header("Helper Stats")]
    public float playerLastLifesteal = 0f;
    
    [SerializeField] private float _playerCurrentHP = 0f;
    public float playerCurrentHP
    {
        get { return _playerCurrentHP; }
        set {
            value = Mathf.Min(value, playerMaxHP);
            if (value > _playerCurrentHP && _playerCurrentHP > 0)
            {
                OnPlayerHealed?.Invoke(value - _playerCurrentHP);
            }
            _playerCurrentHP = value;
            OnCurrentHPChanged?.Invoke(value);
        }
    }


    [SerializeField] private float _playerCurrentMP;
    public float playerCurrentMP
    {
        get { return _playerCurrentMP; }
        set
        {
            value = Mathf.Min(value, playerMaxMP);
            if (value > _playerCurrentMP)
            {
                OnPlayerInfused?.Invoke(value -  _playerCurrentMP);
            }
            _playerCurrentMP = value;
            OnCurrentMPChanged?.Invoke(value);
        }
    }

    // Fills HP and MP to max for a new run or wave. Skips the heal/mana-gain events on purpose:
    // a refill isn't a heal, so it shouldn't show "+X" popups. The bars still update.
    public void RefillHPAndMP()
    {
        _playerCurrentHP = playerMaxHP;
        OnCurrentHPChanged?.Invoke(_playerCurrentHP);
        _playerCurrentMP = playerMaxMP;
        OnCurrentMPChanged?.Invoke(_playerCurrentMP);
    }

    [SerializeField] private float _playerLightAmount;
    
    public float PlayerLightAmount
    {
        get { return _playerLightAmount; }
        set
        {
            _playerLightAmount = Mathf.CeilToInt(value);
        }
    }
    public float playerOverallXP = 0f;
    public float playerCurrentXP = 0f;     
    public float playerLevelMultiplier = 0f;
    public float playerLevelsGained = 0f;
    
    [Header("Base Stats")]
    public float playerBasePickupRange = 0f;
    public float playerBaseMovespeed = 0f;
    public float playerBaseXP = 0f;
    

    // Turns a percentage stat into a multiplier: +X% multiplies by (1 + X/100) as before, -X% uses
    // the mirrored curve and divides by (1 + X/100). So -100% halves the value instead of reaching
    // 0, and nothing can turn negative (stopped weapons, inverted controls, backwards stabs).
    private static float GetPercentFactor(float percent)
    {
        return percent >= 0f ? 1f + percent / 100f : 1f / (1f - percent / 100f);
    }

    // Weapon cooldowns are divided by this
    public float GetAttackSpeedFactor() => GetPercentFactor(playerAttackSpeed);

    // Weapon ranges are multiplied by this
    public float GetAttackRangeFactor() => GetPercentFactor(playerAttackRange);

    public float GetCurrentPlayerMovespeed()
    {
        return playerBaseMovespeed * GetPercentFactor(playerMovespeed);
    }

    private void Update()
    {
        CommunicateMovementspeedChanged();
    }

    private float oldTotalCurrentMoveSpeed = 0;
    private void CommunicateMovementspeedChanged()
    {
        if (oldTotalCurrentMoveSpeed != playerMovespeed)
        {
            OnMovespeedChanged?.Invoke();
            oldTotalCurrentMoveSpeed = playerMovespeed;
        }
    }
    
    public void ApplyStatsToPlayer(float value, string statName)
    {
        switch (statName)
        {
            case "Max HP":
                this.playerMaxHP += value;
                break;
            case "HP Regeneration":
                this.playerHPRegeneration += value;
                break;
            case "Lifesteal":
                this.playerLifeSteal += value;
                break;
            case "Damage":
                this.playerDamage += value;
                break;
            case "Melee Damage":
                this.playerMeleeDamage += value;
                break;
            case "Ranged Damage":
                this.playerRangedDamage += value;
                break;
            case "Mystic Damage":
                this.playerMysticDamage += value;
                break;
            case "Attackspeed":
                this.playerAttackSpeed += value;
                break;
            case "Crit":
                this.PlayerCritChance += value;
                break;
            case "Range":
                this.playerAttackRange += value;
                break;
            case "Armor":
                this.PlayerArmor += value;
                break;
            case "Dodge":
                this.playerDodge += value;
                break;
            case "Movespeed":
                this.playerMovespeed += value;
                break;
            case "Luck":
                this.playerLuck += value;
                break;
            case "Cooldown":
                this.playerCooldown += value;
                break;
            case "Max MP":
                this.playerMaxMP += value;
                break;
            case "MP Regeneration":
                this.playerMPRegeneration += value;
                break;
            default:
                // Stat names come from Inspector strings - a typo would otherwise do nothing silently
                Debug.LogWarning($"Stat '{statName}' ist in ApplyStatsToPlayer nicht implementiert!");
                break;
        }
    }
}
