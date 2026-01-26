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
    private bool _abilityOnCooldown;
    private IEnumerator _runningAbility;
    private Coroutine _manaDrainCoroutine;
    private GameObject[] _playerWeaponSlots;

    [Header("Ability")]
    [SerializeField] private float ability;
    
    [Header("Passive")]
    [SerializeField] private float passive;

    private void Start()
    {       
        _gameManagerObject = GameObject.FindGameObjectWithTag("Manager");
        _gameManager = _gameManagerObject.GetComponent<GameManager>();
        _playerStats = this.transform.GetComponentInParent<PlayerStats>();
        _playerDealsDamage = this.transform.root.GetComponentInChildren<PlayerDealsDamage>();
        
        characterStats.OnExecuteAbility += CharacterAbilityExecution;
        characterStats.OnStopAbility += EndAbility;
        GameManager.OnRoundOver += ResetAbilityOnRoundOver;
    }
    

    private void OnDestroy()
    {
        GameManager.OnRoundOver -= ResetAbilityOnRoundOver;
        characterStats.OnExecuteAbility -= CharacterAbilityExecution;
        characterStats.OnExecuteAbility -= EndAbility;
    }

    private void CharacterAbilityExecution()
    {
        if (!_abilityRunning && !_abilityOnCooldown && _playerStats.playerCurrentMP >= characterStats.ability_manaCost)
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
        List<GameObject> playerWapons = GetPlayerWeapons();
        float manaPerTick = characterStats.ability_manaCost / 10f;
        float tickInterval = 0.1f;
        
        while (_abilityRunning)
        {
            if (_playerStats.playerCurrentMP >= manaPerTick)
            {
                _playerStats.playerCurrentMP -= manaPerTick;
                
                Whirlwind();
                
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

        _abilityOnCooldown = true;
        characterStats.abilityReady = false;
        _gameManager.StartAbilityCooldown();
        
        _gameManager.SetAbilityUIInactive();
    }

    private void ResetAbilityOnRoundOver()
    {
        if (_abilityRunning) 
        {
            EndAbility();
        }
        else if (_abilityOnCooldown)
        {
            _gameManager.StopAbilityCooldown();
        }
        characterStats.abilityReady = true;
        _abilityRunning = false;
        _abilityOnCooldown = false;
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
    
    private void Whirlwind()
    {
        Debug.Log("Barbar wirbelt und verbraucht Mana");
    }
}
