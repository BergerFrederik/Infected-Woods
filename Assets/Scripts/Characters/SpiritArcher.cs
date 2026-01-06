using System.Collections;
using UnityEngine;

public class SpiritArcher : MonoBehaviour
{
    [SerializeField] private CharacterStats characterStats;
    private PlayerStats playerStats;
    private GameObject GameManagerObject;
    private GameManager gameManager;
    private bool abilityRunning = false;
    private bool abilityOnCooldown = false;
    private IEnumerator runningAbility;

    [Header("Ability")]
    public float boost_by_ability = 50f; //value is just added to playerStats and used in the weapons script
    
    [Header("Passive")]
    public float rangeModifier;
    public float attackSpeedModifier;
    

    private void Start()
    {       
        GameManagerObject = GameObject.FindGameObjectWithTag("Manager");
        gameManager = GameManagerObject.GetComponent<GameManager>();
        playerStats = this.transform.GetComponentInParent<PlayerStats>();
    }

    private void OnEnable()
    {
        characterStats.OnExecuteAbility += CharacterAbilityExecution;
        GameManager.OnRoundOver += ResetAbilityOnRoundOver;
    }

    private void OnDisable()
    {
        GameManager.OnRoundOver -= ResetAbilityOnRoundOver;
        characterStats.OnExecuteAbility -= CharacterAbilityExecution;
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
}
