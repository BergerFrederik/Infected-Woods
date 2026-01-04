using System;
using UnityEngine;

public class PlayerGainsHP : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private float hp_regen_division_const = 10f;
    private float hpAccumulator;

    public event Action<float> OnPlayerWasHealed;

    private void OnEnable()
    {
        MeeleWeaponHitsEnemy.OnMeeleWeaponHitsEnemy += TryApplyLifesteal;
        ProjectileHitsEnemy.OnProjectileHitsEnemy += TryApplyLifesteal;
    }

    private void OnDisable()
    {
        MeeleWeaponHitsEnemy.OnMeeleWeaponHitsEnemy -= TryApplyLifesteal;
        ProjectileHitsEnemy.OnProjectileHitsEnemy -= TryApplyLifesteal;
    }

    void Update()
    {
        HealPlayerPerSecond();
    }
    
    private void HealPlayerPerSecond()
    {
        // based on Lifereg stats
        if (playerStats.playerCurrentHP < playerStats.playerMaxHP)
        {
            // "Stat / 10" = HP per sec
            float hpPerSecond = playerStats.playerHPRegeneration / hp_regen_division_const;
            hpAccumulator += hpPerSecond * Time.deltaTime;

            if (hpAccumulator >= 1f)
            {
                int wholeHPToHeal = Mathf.FloorToInt(hpAccumulator);
                playerStats.playerCurrentHP += wholeHPToHeal;
                hpAccumulator -= wholeHPToHeal;
            }
            OnPlayerWasHealed?.Invoke(playerStats.playerCurrentHP);
        }
    }

    private void TryApplyLifesteal(EnemyStats enemyStats, WeaponStats weaponStats)
    {
        bool doesLifestealProbabilityApply = false;
        float cummulatedLifestealProbability = playerStats.playerLifeSteal + weaponStats.weaponLifesteal;
        if (UnityEngine.Random.Range(0f, 100f) <= cummulatedLifestealProbability)
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
            OnPlayerWasHealed?.Invoke(playerStats.playerCurrentHP);
        }
    }
}
