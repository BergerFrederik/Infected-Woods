using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerHitsEnemy : MonoBehaviour
{
    [SerializeField] private EnemyStats enemyStats;    
    [SerializeField] private GameObject lightDropPrefab;
    [SerializeField] private Transform popUpDamage;
    [SerializeField] private DamageCalculation damageCalculation;

    private PlayerStats playerStats;


    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Weapon") || collider.CompareTag("Projectile"))
        {
            WeaponStats finalWeaponStats = null;

            if (collider.TryGetComponent<WeaponStats>(out WeaponStats weaponStats))
            {
                finalWeaponStats = weaponStats;
            }
            else if (collider.TryGetComponent<Projectile>(out Projectile projectileScript))
            {
                finalWeaponStats = projectileScript.sourceWeaponStats;
                Destroy(collider.gameObject);
            }

            FindPlayer();
            ApplyDamage(finalWeaponStats);
            InstantiatePopUpDamage();
            TryApplyLifeSteal(finalWeaponStats);

            if (enemyStats.enemyMaxHP <= 0f)
            {
                InstantiateDropsOnDeath();
                GiveXPToPlayer();
            }
        }
                       
    }

    private void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerStats = player.GetComponent<PlayerStats>();
    }

    private void ApplyDamage(WeaponStats weaponStats)
    {
        float damageDealt = damageCalculation.CalculateDamageDealtToEnemy(weaponStats, playerStats);
        enemyStats.enemyMaxHP -= damageDealt;
    }

    private void InstantiatePopUpDamage()
    {
        //Instantiate(popUpDamage, transform.position, Quaternion.identity);
    }

    private void TryApplyLifeSteal(WeaponStats weaponStats)
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

    private void InstantiateDropsOnDeath()
    {    
        if (Random.Range(0f, 100f) <= enemyStats.enemyLightDropChance)
        {
            GameObject newLightDrop = Instantiate(lightDropPrefab, transform.position, Quaternion.identity);
            if (newLightDrop.TryGetComponent<LightDrop>(out LightDrop lightDrop))
            {
                lightDrop.lightDropValue = enemyStats.enemyLightDropped;
            }
        }
        Destroy(gameObject);        
    }
    private void GiveXPToPlayer()
    {
        float xpGained = enemyStats.enemyXPGainOnKill;
        playerStats.playerOverallXP += xpGained;
        playerStats.playerCurrentXP += xpGained;
    }
}
