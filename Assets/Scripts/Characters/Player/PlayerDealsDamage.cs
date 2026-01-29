using System;
using System.Collections;
using UnityEngine;

public class PlayerDealsDamage : MonoBehaviour
{
    [SerializeField] private DamageCalculation damageCalculation;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private GameManager gameManager;
    
    private CharacterStats _characterStats;
    
    public event Action OnPlayerHitsEnemy;
    public event Action<WeaponStats> OnPlayerHitsEnemyWithWeapon;
    public event Action<Transform, float, bool> OnInstantiatePopUpDamageUI;
    private void OnEnable()
    {
        gameManager.OnCharacterSet += SetCharacterStats;
        MeeleWeaponHitsEnemy.OnMeeleWeaponHitsEnemy += ApplyDamageToEnemy;
        ProjectileHitsEnemy.OnProjectileHitsEnemy += ApplyDamageToEnemy;
    }

    private void OnDisable()
    {
        gameManager.OnCharacterSet -= SetCharacterStats;
        MeeleWeaponHitsEnemy.OnMeeleWeaponHitsEnemy -= ApplyDamageToEnemy;
        ProjectileHitsEnemy.OnProjectileHitsEnemy -= ApplyDamageToEnemy;
        _characterStats.OnAbilityDealsDamage -= DealDamage;
    }

    private void SetCharacterStats()
    {
        GameObject playerObject = this.transform.root.gameObject;
        GameObject character = playerObject.transform.Find("Character").gameObject;
        _characterStats = character.GetComponentInChildren<CharacterStats>();
        _characterStats.OnAbilityDealsDamage += DealDamage;
    }

    private void ApplyDamageToEnemy(EnemyStats enemyStats, WeaponStats weaponStats)
    {
        OnPlayerHitsEnemy?.Invoke();
        OnPlayerHitsEnemyWithWeapon?.Invoke(weaponStats);
        
        var result = damageCalculation.CalculateDamageDealtToEnemy(weaponStats, playerStats);
        float damageDealtByPlayer = result.damage;
        bool didCrit = result.isCrit;
        
        // bonus damage
        float bonusDamage = 0f;
        damageDealtByPlayer += bonusDamage;
        
        DealDamage(enemyStats, damageDealtByPlayer, didCrit);
    }

    private void DealDamage(EnemyStats enemyStats, float damageDealtByPlayer, bool didCrit)
    {
        enemyStats.TakeDamage(damageDealtByPlayer);
        
        Transform enemyTransform = enemyStats.transform;
        OnInstantiatePopUpDamageUI?.Invoke(enemyTransform, damageDealtByPlayer, didCrit);
    }
}
