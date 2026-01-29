using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    private List<GameObject> _playerWeaponSlots;
    private List<GameObject> _playerWeapons;
    private Dictionary<string, float> _combinedDamageMap;

    [Header("Ability")]
    [SerializeField] private float ability;
    [SerializeField] private Collider2D wwCollider;
    
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
        _manaDrainCoroutine = StartCoroutine(AbilityTickRoutine());            
    }

    private IEnumerator AbilityTickRoutine()
    {
        _playerWeapons = GetPlayerWeapons();
        _combinedDamageMap = GetCombinedWeaponStats(_playerWeapons);
        
        float manaPerTick = characterStats.ability_manaCost / 10f;
        float tickInterval = 0.1f;

        foreach (GameObject weaponSlot in _playerWeaponSlots)
        {
            weaponSlot.SetActive(false);
        }
        
        wwCollider.enabled = true;
    
        while (_abilityRunning)
        {
            if (_playerStats.playerCurrentMP >= manaPerTick)
            {
                _playerStats.playerCurrentMP -= manaPerTick;
                
                Whirlwind(_combinedDamageMap);
                
                yield return new WaitForSeconds(tickInterval);
            }
            else
            {
                EndAbility();
                yield break;
            }
        }
    }

    private void EndAbility()
    {
        if (!_abilityRunning) return; // Verhindert doppeltes Ausführen

        _abilityRunning = false;
        
        if (_manaDrainCoroutine != null)
        {
            StopCoroutine(_manaDrainCoroutine);
        }
        
        foreach (GameObject weaponSlot in _playerWeaponSlots)
        {
            weaponSlot.SetActive(true);
        }

        wwCollider.enabled = false;
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
                GameObject weaponPrefab = weaponSlot.transform.GetChild(0).gameObject;
                playerWeapons.Add(weaponPrefab);
            }
        }

        return playerWeapons;
    }

    private Dictionary<string, float> GetCombinedWeaponStats(List<GameObject> playerWeapons)
    {
        float combinedBaseDamage = 0f;
        float combinedMeeleScaling = 0f;
        float combinedRangedScaling = 0f;
        float combinedMysticScaling = 0f;
        float averageAttackSpeed = 0f;
        float combinedCritChance = 0f;
        float combinedCritDamage = 0f;
        
        Dictionary<string, float> combinedDamageMap = new Dictionary<string, float>();
        foreach (var weapon in playerWeapons)
        {
            WeaponStats weaponStats = weapon.GetComponent<WeaponStats>();
            combinedBaseDamage += weaponStats.weaponBaseDamage;

            averageAttackSpeed += weaponStats.weaponAttackSpeedCooldown;
            combinedCritChance += weaponStats.weaponCritChance;
            combinedCritDamage += weaponStats.weaponCritDamage;
            
        }
        combinedDamageMap.Add("BaseDamage", combinedBaseDamage);
        combinedDamageMap.Add("MeleeScaling", combinedMeeleScaling);
        combinedDamageMap.Add("RangedScaling", combinedRangedScaling);
        combinedDamageMap.Add("MysticScaling", combinedMysticScaling);
        combinedDamageMap.Add("AttackSpeed", averageAttackSpeed);
        combinedDamageMap.Add("CritChance", combinedCritChance);
        combinedDamageMap.Add("CritDamage", combinedCritDamage);
        
        return combinedDamageMap;
    }
    
    private void Whirlwind(Dictionary<string, float> combinedDamageMap)
    {
        
        
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Enemy"))
        {
            if (collider.TryGetComponent<EnemyStats>(out EnemyStats enemyStats))
            {
                var overallDamage = ComputeOverallDamage(_combinedDamageMap);
                Debug.Log("Triggert");
                float damage = overallDamage.damage;
                bool didCrit = overallDamage.didCrit;
                characterStats.InvokeAbilityDamage(enemyStats, damage, didCrit);
            }
        }
    }

    private (float damage, bool didCrit) ComputeOverallDamage(Dictionary<string, float> combinedDamageMap)
    {
        float damage = combinedDamageMap["BaseDamage"];
        bool isCrit = Random.Range(0, 100) < _playerStats.playerCritChance;
        float overallDamage = damage;
        if (isCrit)
        {
            overallDamage *= 2f;
        }
        return (overallDamage, isCrit);
    }
}
