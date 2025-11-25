using System;
using UnityEngine;

public class PlayerTakesDamage : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Player player;
    [SerializeField] private float calculate_armor = 20f;    
    [SerializeField] private float iframe_formula_const_1 = 0.4f;
    [SerializeField] private float iframe_formula_const_2 = 0.15f;

    public event Action<float> OnPlayerTakesDamage;
    public event Action OnPlayerWasDamaged;

    private float iFrameStartTime;
    private float currentIFrames;


    private void OnEnable()
    {
        player.OnPlayerCollidesWithEnemy += DealDamageToPlayer;
    }

    private void OnDisable()
    {
        player.OnPlayerCollidesWithEnemy -= DealDamageToPlayer;
    }

    private void DealDamageToPlayer(EnemyStats enemyStats)
    {
        if (Time.time - iFrameStartTime >= currentIFrames)
        {
            float damageDealtToPlayer = CalculateDamageDealtToPlayer(enemyStats);
            if (damageDealtToPlayer > 0)
            {
                playerStats.playerCurrentHP -= damageDealtToPlayer;
                OnPlayerTakesDamage?.Invoke(damageDealtToPlayer);
                OnPlayerWasDamaged?.Invoke();
                currentIFrames = SetIFrames(damageDealtToPlayer);
                iFrameStartTime = Time.time;
            }            
        }
    }

    
    private float SetIFrames(float damageDealtToPlayer)
    {
        float playerMaxHP = playerStats.playerMaxHP;
        float iframes = iframe_formula_const_1 * ((damageDealtToPlayer / playerMaxHP) / iframe_formula_const_2);
        iframes = Mathf.Clamp(iframes, 0.2f, 0.4f);
        return iframes;
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
