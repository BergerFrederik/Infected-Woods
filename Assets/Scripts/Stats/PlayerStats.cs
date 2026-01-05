using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public event Action OnMovespeedChanged;
    public event Action<float> OnCurrentMPChanged;
    public event Action<float> OnMaxMPChanged;
    public event Action<float> OnCurrentHPChanged;
    public event Action<float> OnMaxHPChanged;

    [Header("Primary Stats")]
    [SerializeField] private float _playerMaxHP;
    public float playerMaxHP
    {
        get { return _playerMaxHP; }
        set { _playerMaxHP = value;
            OnMaxHPChanged?.Invoke(value);
        }
    }

    [SerializeField] private float _playerMaxMP;
    public float playerMaxMP
    {
        get => _playerMaxMP;
        set { _playerMaxMP = value;            
            OnMaxMPChanged?.Invoke(value);
        }
    }

    public float playerHPRegeneration = 0f;
    public float playerMPRegeneration = 0f;
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
    public float playerHealPower = 0f;
    public float playerShieldPower = 0f;



    [Header("Helper Stats")]
    public float playerLastLifesteal = 0f;
    public bool playerAbilityOnCooldown = false;
    
    [SerializeField] private float _playerCurrentHP = 0f;
    public float playerCurrentHP
    {
        get { return _playerCurrentHP; }
        set { 
            _playerCurrentHP = value;
            OnCurrentHPChanged.Invoke(value);
        }
    }


    [SerializeField] private float _playerCurrentMP;
    public float playerCurrentMP
    {
        get { return _playerCurrentMP; }
        set
        {
            _playerCurrentMP = value;
            OnCurrentMPChanged.Invoke(value);
        }
    }
    public float playerLightAmount = 0f;
    public float playerOverallXP = 0f;
    public float playerCurrentXP = 0f;     
    public float playerLevelMultiplier = 0f;
    public float playerAbilityDuration = 0f;
    public float playerLevelsGained = 0f;
    
    [Header("Base Stats")]
    public float playerBasePickupRange = 0f;
    public float playerBaseMovespeed = 0f;
    public float playerBaseXP = 0f;


    public float GetCurrentPlayerMovespeed()
    {
        float playerBaseMS = playerBaseMovespeed;
        float playerMSIncrease = playerMovespeed / 100;
        float currentPlayerMoveSpeed = playerBaseMS + playerBaseMS * playerMSIncrease;
        return currentPlayerMoveSpeed;
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
}
