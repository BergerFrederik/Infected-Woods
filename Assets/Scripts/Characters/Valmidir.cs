using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Valmidir: MonoBehaviour
{
    [SerializeField] private CharacterStats characterStats;
    [SerializeField] private WeaponStats weaponStats;
    private PlayerDealsDamage _playerDealsDamage;
    private PlayerStats _playerStats;
    private RandomRollEvent _randomRollEvent;
    private GameObject _gameManagerObject;
    private bool _abilityRunning;
    private IEnumerator _runningAbilityCoroutine;
    private Coroutine _activeStacksCoroutine;
    private AbilityUI _abilityUI;
    private PlayerGainsHP _playerGainsHp;

    [Header("Ability")]
    [SerializeField] private float maxStacks;
    [SerializeField] private float costPerStack = 50; //prozentuale Kostenerhöhung
    [SerializeField] private float stackDuration;
    [SerializeField] private GameObject projectilePrefab;
    
    [Header("Passive")] [SerializeField] private float dmgPerStack = 50; //prozentuale Schadenserhöhung

    private StackCounter _stacks;
    private bool _isActiveStacksRunning;
    private float _stacksExpireAt;
    private float _originalBaseDamage;
    private float _originalMeleeDamageScale;
    private float _originalRangedDamageScale;
    private float _originalMysticDamageScale;

    private void Start()
    {       
        _playerStats = this.transform.root.GetComponent<PlayerStats>();
        _playerDealsDamage = this.transform.root.GetComponentInChildren<PlayerDealsDamage>();
        _playerGainsHp = this.transform.root.GetComponentInChildren<PlayerGainsHP>();
        _randomRollEvent = this.transform.root.GetComponentInChildren<RandomRollEvent>();
        _abilityUI = FindAnyObjectByType<AbilityUI>();
        _stacks = new StackCounter(_playerStats, maxStacks);
        _abilityUI.EnableStackUI();

        characterStats.OnExecuteAbility += CharacterAbilityExecution;
        GameManager.OnRoundOver += ResetAbilityOnRoundOver;
        _playerDealsDamage.OnPlayerHitsEnemy += GainLifeOnHit;

        GetOriginalDamageValues();
    }

    private void Update()
    {
        UpdateStackUI();
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
        float amplyfiedManaCost = Mathf.Round(manaCost + manaCost * ((costPerStack * _stacks.Current) / 100f));
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
        AmplifyDamageByStacks();

        foreach (Transform enemy in enemysInRadius)
        {
            GameObject newProjectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Projectile projectile = newProjectile.GetComponent<Projectile>();
            projectile.sourceWeaponStats = weaponStats;
            projectile.target = enemy;
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
        if (_stacks.TryAdd())
        {
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
        _stacksExpireAt = Time.time + stackDuration;
        yield return new WaitForSeconds(stackDuration);
        _stacks.Reset();
        _isActiveStacksRunning = false;
        RestoreOriginalDamage();
    }

    // Reads the stacks themselves every frame, so the HUD also shows a reset that came from somewhere else
    private void UpdateStackUI()
    {
        float secondsRemaining = _stacks.Current > 0f ? _stacksExpireAt - Time.time : 0f;
        _abilityUI.SetStackCount(_stacks.Current);
        _abilityUI.SetStackCountdown(secondsRemaining);
    }

    private void AmplifyDamageByStacks()
    {
        float damageMultiplier = 1f + (dmgPerStack * _stacks.Current) / 100f; //Base und Scaling, damit der Bonus auch mit Spielerstats prozentual bleibt
        weaponStats.weaponBaseDamage = _originalBaseDamage * damageMultiplier;
        weaponStats.weaponMeleeDamageScale = _originalMeleeDamageScale * damageMultiplier;
        weaponStats.weaponRangedDamageScale = _originalRangedDamageScale * damageMultiplier;
        weaponStats.weaponMysticDamageScale = _originalMysticDamageScale * damageMultiplier;
    }

    private void RestoreOriginalDamage()
    {
        weaponStats.weaponBaseDamage = _originalBaseDamage;
        weaponStats.weaponMeleeDamageScale = _originalMeleeDamageScale;
        weaponStats.weaponRangedDamageScale = _originalRangedDamageScale;
        weaponStats.weaponMysticDamageScale = _originalMysticDamageScale;
    }

    private void ResetAbilityOnRoundOver()
    {
        StopAllCoroutines();
        _stacks.Reset();
        _isActiveStacksRunning = false;
        RestoreOriginalDamage();
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
        float rndRoll = _randomRollEvent.GetRandomFloatRoll(1f, 100f);
        if (rndRoll >= 100f - weaponStats.weaponLifesteal)
        {
            // One heal of 1 + stacks HP - separate 1 HP heals would be blocked by the lifesteal cooldown
            _playerGainsHp.ApplyLifestealHeal(1f + _stacks.Current);
        }
    }

    private void GetOriginalDamageValues()
    {
        _originalBaseDamage = weaponStats.weaponBaseDamage;
        _originalMeleeDamageScale = weaponStats.weaponMeleeDamageScale;
        _originalRangedDamageScale = weaponStats.weaponRangedDamageScale;
        _originalMysticDamageScale = weaponStats.weaponMysticDamageScale;
    }
}
