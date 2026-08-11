using System;
using System.Collections;
using UnityEngine;

public class PlayerDealsDamage : MonoBehaviour
{
    [SerializeField] private DamageCalculation damageCalculation;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private InstantiatePopUp instantiatePopUp;
    
    public event Action OnPlayerHitsEnemy;
    public event Action<WeaponStats> OnPlayerHitsEnemyWithWeapon;

    public void ApplyDamageToEnemy(EnemyStats enemyStats, WeaponStats weaponStats)
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
        instantiatePopUp.Instantiate(damageDealtByPlayer, didCrit, enemyTransform);
    }
}
