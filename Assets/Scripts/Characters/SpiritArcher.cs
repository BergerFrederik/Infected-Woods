using System.Collections;
using UnityEngine;

public class SpiritArcher : MonoBehaviour
{
    [SerializeField] private CharacterStats characterStats;
    private PlayerDealsDamage _playerDealsDamage;
    private PlayerStats playerStats;
    private GameObject GameManagerObject;
    private bool abilityRunning = false;
    private IEnumerator runningAbility;
    private AbilityUI _abilityUI;
    private RandomRollEvent _randomRollEvent;

    [Header("Ability")]
    public float boost_by_ability = 50f; //value is just added to playerStats and used in the weapons script
    
    [Header("Passive")]
    [SerializeField] private float _chanceToGainLight;

    private void Start()
    {       
        playerStats = this.transform.root.GetComponent<PlayerStats>();
        _playerDealsDamage = this.transform.root.GetComponentInChildren<PlayerDealsDamage>();
        _randomRollEvent = this.transform.root.GetComponentInChildren<RandomRollEvent>();
        _abilityUI = FindAnyObjectByType<AbilityUI>();
        
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
        if (characterStats.abilityReady && !abilityRunning && playerStats.playerCurrentMP >= manaCost)
        {
            playerStats.playerCurrentMP -= manaCost;
            StartAbility();
            RunAbility();
        }
    }

    private void StartAbility()
    {        
        playerStats.playerAttackSpeed += boost_by_ability;
        _abilityUI.StartActiveAbilityUI();           
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
        _abilityUI.EndActiveAbilityUI();
        characterStats.cooldownStarted = true;
        playerStats.playerAttackSpeed -= boost_by_ability;
        characterStats.abilityReady = false;
    }

    private void ResetAbilityOnRoundOver()
    {
        if (abilityRunning) 
        {
            StopCoroutine(runningAbility);                     
            _abilityUI.EndActiveAbilityUI();
            playerStats.playerAttackSpeed -= boost_by_ability;
        }
        else if (!characterStats.abilityReady)
        {
            _abilityUI.EndActiveAbilityUI();
        }
        characterStats.abilityReady = true;
        abilityRunning = false;
    }

    private void GainLightOnHit()
    {
        float randomNum = _randomRollEvent.GetRandomFloatRoll(0f, 100f);
        if (randomNum > 1 - _chanceToGainLight) //Muss 1- sein, damit luck einen Einfluss hat. Luck erhöht den Roll
        {
            playerStats.playerLightAmount += 1f;
        }
    }
}
