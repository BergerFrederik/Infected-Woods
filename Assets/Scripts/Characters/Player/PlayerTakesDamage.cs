using System;
using UnityEngine;

public class PlayerTakesDamage : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private float calculate_armor = 20f;

    public static event Action OnPlayerTakesDamage;


    public float DealDamageToPlayer(EnemyStats enemyStats)
    {
        float damageDealtToPlayer = CalculateDamageDealtToPlayer(enemyStats);
        playerStats.playerCurrentHP -= damageDealtToPlayer;
        OnPlayerTakesDamage?.Invoke();
        return damageDealtToPlayer;
    }
    private float CalculateDamageDealtToPlayer(EnemyStats enemyStats)
    {
        float damageByEnemy = enemyStats.enemyDamage;

        // Balancing for Armor. Armor should be less effective, the more armor you have. Changing calculate_armor_const balances the armor.
        float damageReductionByArmor = (playerStats.playerArmor / (playerStats.playerArmor + calculate_armor));

        float totalDamageDealt = Mathf.Round(damageByEnemy * (1 - damageReductionByArmor));

        // You should always get at least 1 damage
        if (totalDamageDealt < 1f)
        {
            totalDamageDealt = 1f;
        }

        // Dodge

        if (UnityEngine.Random.Range(0f, 100f) <= playerStats.playerDodge)
        {
            totalDamageDealt = 0f;
        }

        return totalDamageDealt;
    }
}
