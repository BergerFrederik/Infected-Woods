using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Valmidir: MonoBehaviour
{
    [SerializeField] private CharacterStats characterStats;
    [SerializeField] private WeaponStats weaponStats;
    private PlayerDealsDamage _playerDealsDamage;
    private PlayerStats _playerStats;
    private GameObject _gameManagerObject;
    private bool _abilityRunning;
    private IEnumerator _runningAbilityCoroutine;
    private Coroutine _activeStacksCoroutine;
    private AbilityUI _abilityUI;
    private RandomRollEvent _randomRollEvent;

    [Header("Ability")]
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float maxStacks;
    [SerializeField] private float costPerStack = 50; //prozentuale Kostenerhöhung
    [SerializeField] private float stackDuration;
    [SerializeField] private GameObject projectilePrefab;
    
    
    [Header("Passive")]
    [SerializeField] private float dmgPerStack;
    [SerializeField] private float lifestealPerStack;
    [SerializeField] private float manaPerKill;

    private float _currentStacks;
    private bool _isActiveStacksRunning;

    private void Start()
    {       
        _playerStats = this.transform.root.GetComponent<PlayerStats>();
        _playerDealsDamage = this.transform.root.GetComponentInChildren<PlayerDealsDamage>();
        _randomRollEvent = this.transform.root.GetComponentInChildren<RandomRollEvent>();
        _abilityUI = FindAnyObjectByType<AbilityUI>();
        
        characterStats.OnExecuteAbility += CharacterAbilityExecution;
        GameManager.OnRoundOver += ResetAbilityOnRoundOver;
        _playerDealsDamage.OnPlayerHitsEnemy += GainLifeOnHit;
    }
    

    private void OnDestroy()
    {
        GameManager.OnRoundOver -= ResetAbilityOnRoundOver;
        characterStats.OnExecuteAbility -= CharacterAbilityExecution;
        _playerDealsDamage.OnPlayerHitsEnemy -= GainLifeOnHit;
    }

    public void CharacterAbilityExecution()
    {
        float manaCost = characterStats.ability_manaCost;
        float amplyfiedManaCost = Mathf.Round(manaCost + manaCost * ((costPerStack * _currentStacks) / 100f));
        if (FindEnemysInRadius().Count == 0) return;
        if (characterStats.abilityReady && !_abilityRunning && _playerStats.playerCurrentMP >= amplyfiedManaCost)
        {
            _playerStats.playerCurrentMP -= amplyfiedManaCost;
            StartAbility();
            RunAbility();
        }
    }

    private void StartAbility()
    {        
        _abilityUI.StartActiveAbilityUI();           
    }

    private void RunAbility()
    {
        _runningAbilityCoroutine = RunAbilityCoroutine();
        StartCoroutine(_runningAbilityCoroutine);
    }

    private IEnumerator RunAbilityCoroutine()
    {
        yield return null;
        _abilityRunning = true;
        List<Transform> enemysInRadius = FindEnemysInRadius();
        
        foreach (Transform enemy in enemysInRadius)
        {
            GameObject newProjectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Projectile projectile = newProjectile.GetComponent<Projectile>();
            projectile.sourceWeaponStats = weaponStats;
            projectile.target = enemy;
            projectile.abilityProjectileSpeed = projectileSpeed;
            newProjectile.GetComponent<ProjectileHitsEnemy>().SetOwner(transform.root);
        }
        
        _abilityRunning = false;
        EndAbility();
    }
    
    private List<Transform> FindEnemysInRadius()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, weaponStats.weaponRange);
        List<Transform> enemysInRadius = new();

        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                enemysInRadius.Add(collider.transform);
            }
        }
        return enemysInRadius;
    }

    private void EndAbility()
    {
        if (_currentStacks < 5f)
        {
            _currentStacks++;
            if (_isActiveStacksRunning) StopCoroutine(_activeStacksCoroutine);
            _activeStacksCoroutine = StartCoroutine(ActiveStacksCoroutine());
        }
        
        _abilityUI.EndActiveAbilityUI();
        characterStats.cooldownStarted = true;
        characterStats.abilityReady = false;
    }

    private IEnumerator ActiveStacksCoroutine()
    {
        _isActiveStacksRunning = true;
        yield return new WaitForSeconds(stackDuration);
        _currentStacks = 0;
        _isActiveStacksRunning = false;
    }

    private void ResetAbilityOnRoundOver()
    {
        StopAllCoroutines();
        if (_abilityRunning) 
        {
            _abilityUI.EndActiveAbilityUI();
        }
        else if (!characterStats.abilityReady)
        {
            _abilityUI.EndActiveAbilityUI();
        }
        characterStats.abilityReady = true;
        _abilityRunning = false;
    }

    private void GainLifeOnHit()
    {
        float randomNum = _randomRollEvent.GetRandomFloatRoll(0f, 100f);
        if (randomNum > 1f - weaponStats.weaponLifesteal) //Muss 1- sein, damit luck einen Einfluss hat. Luck erhöht den Roll
        {
            _playerStats.playerCurrentHP += 1f;
        }
    }
}
