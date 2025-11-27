using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpiritArcher : MonoBehaviour
{
    [Header("Stats")]
    public float characterMaxHP = 0f;
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
    public float boost_by_ability = 50f; //value is just added to playerStats and used in the weapons script
    public float ability_duration = 10f; //seconds
    public float ability_cooldown = 5f;
    

    private PlayerInput gameInput;
    private PlayerStats playerStats;
    private GameObject GameManagerObject;
    private GameManager gameManager;
    private float startTime;
    private float remainingCooldown;
    private bool abilityReady = true;
    private bool abilityRunning = false;
    private bool abilityOnCooldown = false;
    private IEnumerator runningAbility;


    private void Start()
    {
        playerStats = this.transform.GetComponentInParent<PlayerStats>();

        playerStats.playerMaxHP = characterMaxHP;
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
        playerStats.playerLightAbsorption = characterLightAbsorption;

        playerStats.playerAbilityCooldown = ability_cooldown;
        playerStats.playerAbilityDuration = ability_duration;
        
        playerStats.playerCurrentHP = characterMaxHP;
    }

    private void Awake()
    {       
        //animator = GetComponentInChildren<Animator>();
        gameInput = new PlayerInput();
        GameManagerObject = GameObject.FindGameObjectWithTag("Manager");
        gameManager = GameManagerObject.GetComponent<GameManager>();
    }


    private void Update()
    {
        if (!abilityReady)
        {
            float passedCooldown = Time.time - startTime;
            float reducedCooldown = ability_cooldown * (1f - playerStats.playerCooldown / 100f);
            float clampedCooldown = Mathf.Clamp(reducedCooldown, 0.01f, ability_cooldown);
            float remainingCooldown = clampedCooldown - passedCooldown;
            if (remainingCooldown <= 0)
            {
                abilityReady = true;
            }
        }
    }

    private void OnEnable()
    {
        gameInput.Enable();
        GameManager.OnRoundOver += ResetAbilityOnRoundOver;
        gameInput.Player.UseAbility.performed += OnUseAbilityPerformed;
    }

    private void OnDisable()
    {
        gameInput.Player.UseAbility.performed -= OnUseAbilityPerformed;
        gameInput.Disable();
        GameManager.OnRoundOver -= ResetAbilityOnRoundOver;
    }

    private void OnUseAbilityPerformed(InputAction.CallbackContext context)
    {
        if (abilityReady && !abilityRunning)
        {
            CharacterAbilityExecution();
        }
    }

    private void CharacterAbilityExecution()
    {       
        playerStats.playerAttackSpeed += boost_by_ability;
        gameManager.SetAbilityUIActive();        
        runningAbility = RunAbility();
        StartCoroutine(runningAbility);
    }

    private IEnumerator RunAbility()
    {
        abilityRunning = true;
        yield return new WaitForSeconds(ability_duration);
        abilityRunning = false;
        startTime = Time.time;
        abilityReady = false;
        gameManager.StartAbilityCooldown();
        abilityOnCooldown = true;
        playerStats.playerAttackSpeed -= boost_by_ability;
    }

    private void ResetAbilityOnRoundOver()
    {
        if (abilityRunning) 
        {
            StopCoroutine(runningAbility);
            abilityReady = true;
            abilityRunning = false;
            startTime = 0f;           
            gameManager.SetAbilityUIInactive();
            playerStats.playerAttackSpeed -= boost_by_ability;
        }
        else if (abilityOnCooldown)
        {
            abilityReady = true;
            abilityRunning = false;
            gameManager.StopAbilityCooldown();
        }
    }
}
