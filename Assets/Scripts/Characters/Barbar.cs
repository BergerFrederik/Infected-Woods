using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barbar : MonoBehaviour
{
    [SerializeField] private CharacterStats characterStats;
    
    private PlayerDealsDamage _playerDealsDamage;
    private PlayerStats _playerStats;
    private GameObject _gameManagerObject;
    private GameManager _gameManager;
    private bool _abilityRunning;
    private IEnumerator _runningAbility;
    private Coroutine _manaDrainCoroutine;
    private Coroutine _dealDamageCoroutine;
    private List<GameObject> _playerWeaponSlots;
    private List<GameObject> _playerWeapons;

    [Header("Ability")]
    [SerializeField] private float ability;
    [SerializeField] private Collider2D wwCollider;
    [SerializeField] private WeaponStats abilityWeaponStats;
    [SerializeField] private GameObject wwIndicator;
    [SerializeField] private float attackSpeedBoost = 200;
    
    [Header("Passive")]
    [SerializeField] private float passive;

    private void Start()
    {       
        _gameManagerObject = GameObject.FindGameObjectWithTag("Manager");
        _gameManager = _gameManagerObject.GetComponent<GameManager>();
        _playerStats = this.transform.GetComponentInParent<PlayerStats>();
        _playerDealsDamage = this.transform.root.GetComponentInChildren<PlayerDealsDamage>();
        _playerWeaponSlots = GetPlayerWeaponSlots();
        
        characterStats.OnExecuteAbility += CharacterAbilityExecution;
        characterStats.OnStopAbility += EndAbility;
        GameManager.OnRoundOver += ResetAbilityOnRoundOver;
        
        wwCollider.enabled = false;
        wwIndicator.SetActive(false);
    }
    

    private void OnDestroy()
    {
        GameManager.OnRoundOver -= ResetAbilityOnRoundOver;
        characterStats.OnExecuteAbility -= CharacterAbilityExecution;
        characterStats.OnExecuteAbility -= EndAbility;
    }

    private void CharacterAbilityExecution()
    {
        if (!_abilityRunning && characterStats.abilityReady && _playerStats.playerCurrentMP >= characterStats.ability_manaCost)
        {
            StartAbility();
        }
    }

    private void StartAbility()
    {        
        _abilityRunning = true;
        _gameManager.SetAbilityUIActive();
        
        _playerWeapons = GetPlayerWeapons();
        SetAbilityWeaponStats(_playerWeapons);
        foreach (GameObject weapon in _playerWeapons)
        {
            weapon.SetActive(false);
        }
        
        wwCollider.enabled = true;
        wwIndicator.SetActive(true);
        
        _manaDrainCoroutine = StartCoroutine(AbilityTickRoutine());
        _dealDamageCoroutine = StartCoroutine(AbilityDamageRoutine());
    }

    private IEnumerator AbilityTickRoutine()
    {
        float manaPerTick = characterStats.ability_manaCost / 10f;
        float tickInterval = 0.1f;
        
        while (_abilityRunning)
        {
            if (_playerStats.playerCurrentMP >= manaPerTick)
            {
                _playerStats.playerCurrentMP -= manaPerTick;
                yield return new WaitForSeconds(tickInterval);
            }
            else
            {
                EndAbility();
                yield break;
            }
        }
    }

    private IEnumerator AbilityDamageRoutine()
    {
        while (_abilityRunning)
        {
            float speedFactor = 1f + (_playerStats.playerAttackSpeed / 100f);
            float attackSpeedCooldown = abilityWeaponStats.weaponAttackSpeedCooldown / speedFactor;
            attackSpeedCooldown = Mathf.Max(attackSpeedCooldown, 0.05f);
            Debug.Log(attackSpeedCooldown);
            DealWhirlwindDamage();
            yield return new WaitForSeconds(attackSpeedCooldown);
        }
    }
    
    private void DealWhirlwindDamage()
    {
        float radius = ((CircleCollider2D)wwCollider).radius * transform.lossyScale.x;
        
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (Collider2D enemyCollider in hitEnemies)
        {
            if (enemyCollider.CompareTag("Enemy"))
            {
                if (enemyCollider.TryGetComponent<EnemyStats>(out EnemyStats enemyStats))
                {
                    _playerDealsDamage.ApplyDamageToEnemy(enemyStats, abilityWeaponStats);
                }
            }
        }
    }

    private void EndAbility()
    {
        if (!_abilityRunning) return; // Verhindert doppeltes Ausführen

        _abilityRunning = false;
        
        if (_manaDrainCoroutine != null) StopCoroutine(_manaDrainCoroutine);
        if (_dealDamageCoroutine != null) StopCoroutine(_dealDamageCoroutine);
        
        foreach (GameObject weapon in _playerWeapons)
        {
            weapon.SetActive(true);
        }

        wwCollider.enabled = false;
        wwIndicator.SetActive(false);
        characterStats.abilityReady = false;
        characterStats.cooldownStarted = true;
        
        
        _gameManager.StartAbilityCooldown();
        _gameManager.SetAbilityUIInactive();
    }

    private void ResetAbilityOnRoundOver()
    {
        if (_abilityRunning) 
        {
            EndAbility();
        }
        else if (!characterStats.abilityReady)
        {
            _gameManager.StopAbilityCooldown();
        }
        characterStats.abilityReady = true;
        _abilityRunning = false;
    }

    private List<GameObject> GetPlayerWeaponSlots()
    {
        GameObject playerObject = this.transform.root.gameObject;
        GameObject playerWeaponManager = playerObject.transform.Find("PlayerWeaponManager").gameObject;
        List<GameObject> playerWeaponSlots = new List<GameObject>();
        for (int i = 0; i < playerWeaponManager.transform.childCount; i++)
        {
            GameObject newWeaponSlot = playerWeaponManager.transform.GetChild(i).gameObject;
            playerWeaponSlots.Add(newWeaponSlot);
        }

        return playerWeaponSlots;
    }

    private List<GameObject> GetPlayerWeapons()
    {
        List<GameObject> playerWeapons = new List<GameObject>();
        foreach (GameObject weaponSlot in _playerWeaponSlots)
        {
            if (weaponSlot.transform.childCount != 0)
            {
                WeaponStats weaponStats = weaponSlot.GetComponentInChildren<WeaponStats>();
                if (weaponStats.weaponWeaponType == WeaponStats.weaponTypeOptions.Melee)
                {
                    GameObject weaponPrefab = weaponSlot.transform.GetChild(0).gameObject;
                    playerWeapons.Add(weaponPrefab);
                }
            }
        }

        return playerWeapons;
    }

    private void SetAbilityWeaponStats(List<GameObject> playerWeapons)
    {
        float combinedBaseDamage = 0f;
        float combinedMeeleScaling = 0f;
        float combinedRangedScaling = 0f;
        float combinedMysticScaling = 0f;
        float averageAttackSpeed = 0f;
        float combinedCritChance = 0f;
        float averageCritDamage = 0f;

        foreach (var weapon in playerWeapons)
        {
            WeaponStats weaponStats = weapon.GetComponent<WeaponStats>();
            combinedBaseDamage += weaponStats.weaponBaseDamage;
            combinedMeeleScaling += weaponStats.weaponMeeleDamageScale;
            combinedRangedScaling += weaponStats.weaponRangedDamageScale;
            combinedMysticScaling += weaponStats.weaponMysticDamageScale;
            averageAttackSpeed += weaponStats.weaponAttackSpeedCooldown;
            combinedCritChance += weaponStats.weaponCritChance;
            averageCritDamage += weaponStats.weaponCritDamage;
        }
        
        int numWeapons = playerWeapons.Count;
        float averageAttackSpeedPerWeapon = averageAttackSpeed/numWeapons;
        float finalAttackSpeed = averageAttackSpeedPerWeapon / (1 + attackSpeedBoost);
        abilityWeaponStats.weaponBaseDamage = combinedBaseDamage;
        abilityWeaponStats.weaponAttackSpeedCooldown = finalAttackSpeed;
        abilityWeaponStats.weaponMeeleDamageScale = combinedMeeleScaling;
        abilityWeaponStats.weaponRangedDamageScale = combinedRangedScaling;
        abilityWeaponStats.weaponMysticDamageScale = combinedMysticScaling;
        abilityWeaponStats.weaponCritChance = combinedCritChance;
        abilityWeaponStats.weaponCritDamage = averageCritDamage/numWeapons;
    }
}
