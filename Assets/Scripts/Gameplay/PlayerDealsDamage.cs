using UnityEngine;

public class PlayerDealsDamage : MonoBehaviour
{
    [SerializeField] private DamageCalculation damageCalculation;
    [SerializeField] private PlayerStats playerStats;
    private void OnEnable()
    {
        MeeleWeaponHitsEnemy.OnMeeleWeaponHitsEnemy += ApplyDamageToEnemy;
        MeeleWeaponHitsEnemy.OnMeeleWeaponHitsEnemy += TryApplyLifesteal;
        ProjectileHitsEnemy.OnProjectileHitsEnemy += ApplyDamageToEnemy;
        ProjectileHitsEnemy.OnProjectileHitsEnemy += TryApplyLifesteal;
        
    }

    private void OnDisable()
    {
        MeeleWeaponHitsEnemy.OnMeeleWeaponHitsEnemy -= ApplyDamageToEnemy;
        MeeleWeaponHitsEnemy.OnMeeleWeaponHitsEnemy -= TryApplyLifesteal;
        ProjectileHitsEnemy.OnProjectileHitsEnemy -= ApplyDamageToEnemy;
        ProjectileHitsEnemy.OnProjectileHitsEnemy -= TryApplyLifesteal;
    }

    private void ApplyDamageToEnemy(EnemyStats enemyStats, WeaponStats weaponStats)
    {
        float damageDealtByPlayer = damageCalculation.CalculateDamageDealtToEnemy(weaponStats, playerStats, enemyStats);
        // bonus damage

        enemyStats.TakeDamage(damageDealtByPlayer);
    }

    private void TryApplyLifesteal(EnemyStats enemyStats, WeaponStats weaponStats)
    {
        bool doesLifestealProbabilityApply = false;
        float cummulatedLifestealProbability = playerStats.playerLifeSteal + weaponStats.weaponLifesteal;
        if (Random.Range(0f, 100f) <= cummulatedLifestealProbability)
        {
            doesLifestealProbabilityApply = true;
        }

        bool playerIsFullHP = false;
        if (playerStats.playerCurrentHP < playerStats.playerMaxHP)
        {
            playerIsFullHP = true;
        }

        bool playerCanLifesteal = false;
        if (Time.time - playerStats.playerLastLifesteal >= 0.1f)
        {
            playerCanLifesteal = true;
        }

        if (doesLifestealProbabilityApply && playerIsFullHP && playerCanLifesteal)
        {
            playerStats.playerLastLifesteal = Time.time;
            playerStats.playerCurrentHP++;
        }
    }
}
