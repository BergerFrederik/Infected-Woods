using UnityEngine;

public class PlayerDealsDamage : MonoBehaviour
{
    [SerializeField] private DamageCalculation damageCalculation;
    [SerializeField] private PlayerStats playerStats;
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
        float damageDealtByPlayer = damageCalculation.CalculateDamageDealtToEnemy(weaponStats, playerStats, enemyStats);
        // bonus damage
        float bonusDamage = 0f;
        damageDealtByPlayer += bonusDamage;

        enemyStats.TakeDamage(damageDealtByPlayer);
    }
}
