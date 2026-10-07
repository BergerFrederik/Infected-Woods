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
    public event Action<EnemyStats, WeaponStats> OnPlayerCritsEnemy;

    public bool ApplyCritableDamageToEnemy(EnemyStats enemyStats, WeaponStats weaponStats)
    {
        OnPlayerHitsEnemy?.Invoke();
        OnPlayerHitsEnemyWithWeapon?.Invoke(weaponStats);

        var result = damageCalculation.CalculateCritableDamageDealtToEnemy(weaponStats, playerStats);
        float damageDealtByPlayer = result.damage;
        bool didCrit = result.isCrit;

        // bonus damage
        float bonusDamage = 0f;
        damageDealtByPlayer += bonusDamage;

        DealDamage(enemyStats, weaponStats, damageDealtByPlayer, didCrit);

        return didCrit;
    }

    public void ApplyNonCritableDamageToEnemy(EnemyStats enemyStats, WeaponStats weaponStats)
    {
        OnPlayerHitsEnemy?.Invoke();
        OnPlayerHitsEnemyWithWeapon?.Invoke(weaponStats);

        float damageDealtByPlayer = damageCalculation.CalculateNonCritableDamageDealtToEnemy(weaponStats, playerStats);
        
        // bonus damage
        float bonusDamage = 0f;
        damageDealtByPlayer += bonusDamage;

        DealDamage(enemyStats, weaponStats, damageDealtByPlayer, false);
    }

    private void DealDamage(EnemyStats enemyStats, WeaponStats weaponStats, float damageDealtByPlayer, bool didCrit)
    {
        // Damaging weapons hit for at least 1, support weapons (base damage 0) for at least 0 -
        // see WeaponStats.MinimumHitDamage. Negative stats (damage, crit damage, flat damage) could
        // otherwise push a hit below 0, which would heal the enemy. All enemy hits pass here.
        damageDealtByPlayer = Mathf.Max(weaponStats.MinimumHitDamage, damageDealtByPlayer);

        enemyStats.TakeDamage(damageDealtByPlayer);
        
        Transform enemyTransform = enemyStats.transform;
        instantiatePopUp.Instantiate(damageDealtByPlayer, didCrit, enemyTransform);

        // Fires for every crit that dealt damage, whatever its source (weapon, projectile, ability).
        // Instance event: only this player's subscribers are notified.
        if (didCrit)
        {
            OnPlayerCritsEnemy?.Invoke(enemyStats, weaponStats);
        }
    }
}
