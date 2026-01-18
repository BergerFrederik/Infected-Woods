using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpiritArcher : MonoBehaviour
{
    [SerializeField] private CharacterStats characterStats;
    private PlayerDealsDamage _playerDealsDamage;
    private PlayerStats playerStats;
    private GameObject GameManagerObject;
    private GameManager gameManager;
    private bool abilityRunning = false;
    private bool abilityOnCooldown = false;
    private IEnumerator runningAbility;

    [Header("Ability")]
    public float boost_by_ability = 50f; //value is just added to playerStats and used in the weapons script
    
    [Header("Passive")]
    [SerializeField] private float _chanceToGainLight;

    private void Start()
    {       
        GameManagerObject = GameObject.FindGameObjectWithTag("Manager");
        gameManager = GameManagerObject.GetComponent<GameManager>();
        playerStats = this.transform.GetComponentInParent<PlayerStats>();
        _playerDealsDamage = this.transform.root.GetComponentInChildren<PlayerDealsDamage>();
        
        characterStats.OnExecuteAbility += CharacterAbilityExecution;
        GameManager.OnRoundOver += ResetAbilityOnRoundOver;
        _playerDealsDamage.OnPlayerHitsEnemy += GainLightOnHit;
    }
    

    private void OnDestroy()
    {
        GameManager.OnRoundOver -= ResetAbilityOnRoundOver;
        characterStats.OnExecuteAbility -= CharacterAbilityExecution;
        _playerDealsDamage.OnPlayerHitsEnemy -= GainLightOnHit;
    }

    public void CharacterAbilityExecution()
    {
        float manaCost = characterStats.ability_manaCost;
        if (!abilityRunning && playerStats.playerCurrentMP >= manaCost)
        {
            playerStats.playerCurrentMP -= manaCost;
            StartAbility();
            RunAbility();
        }
    }

    private void StartAbility()
    {        
        playerStats.playerAttackSpeed += boost_by_ability;
        gameManager.SetAbilityUIActive();              
    }

    private void RunAbility()
    {
        runningAbility = RunAbilityCoroutine();
        StartCoroutine(runningAbility);
    }

    private IEnumerator RunAbilityCoroutine()
    {
        abilityRunning = true;

        float ability_duration = characterStats.ability_duration;        
        yield return new WaitForSeconds(ability_duration);

        abilityRunning = false;
        EndAbility();
    }

    private void EndAbility()
    {
        playerStats.playerAttackSpeed -= boost_by_ability;
        abilityOnCooldown = true;
        characterStats.abilityReady = false;
        gameManager.StartAbilityCooldown();
    }

    private void ResetAbilityOnRoundOver()
    {
        if (abilityRunning) 
        {
            StopCoroutine(runningAbility);                     
            gameManager.SetAbilityUIInactive();
            playerStats.playerAttackSpeed -= boost_by_ability;
        }
        else if (abilityOnCooldown)
        {
            gameManager.StopAbilityCooldown();
        }
        characterStats.abilityReady = true;
        abilityRunning = false;
    }

    private void GainLightOnHit()
    {
        int randomNum = Random.Range(0, 100);
        if (randomNum < _chanceToGainLight)
        {
            Debug.Log("Light");
            playerStats.playerLightAmount += 1f;
        }
    }
}
