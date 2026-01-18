using System;
using System.Collections;
using UnityEngine;

public class PlayerDealsDamage : MonoBehaviour
{
    [SerializeField] private DamageCalculation damageCalculation;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private GameObject popUpDmgUI;
    
    public event Action OnPlayerHitsEnemy;
    public event Action<WeaponStats> OnPlayerHitsEnemyWithWeapon;
    public event Action<Transform, float> OnInstantiatePopUpDamageUI;
    private void OnEnable()
    {
        MeeleWeaponHitsEnemy.OnMeeleWeaponHitsEnemy += ApplyDamageToEnemy;
        ProjectileHitsEnemy.OnProjectileHitsEnemy += ApplyDamageToEnemy;       
    }

    private void OnDisable()
    {
        MeeleWeaponHitsEnemy.OnMeeleWeaponHitsEnemy -= ApplyDamageToEnemy;
        ProjectileHitsEnemy.OnProjectileHitsEnemy -= ApplyDamageToEnemy;
    }

    private void ApplyDamageToEnemy(EnemyStats enemyStats, WeaponStats weaponStats)
    {
        OnPlayerHitsEnemy?.Invoke();
        OnPlayerHitsEnemyWithWeapon?.Invoke(weaponStats);
        float damageDealtByPlayer = damageCalculation.CalculateDamageDealtToEnemy(weaponStats, playerStats, enemyStats);
        // bonus damage
        float bonusDamage = 0f;
        damageDealtByPlayer += bonusDamage;
        enemyStats.TakeDamage(damageDealtByPlayer);
        
        Transform enemyTransform = enemyStats.transform;
        OnInstantiatePopUpDamageUI?.Invoke(enemyTransform, damageDealtByPlayer);
    }

}
