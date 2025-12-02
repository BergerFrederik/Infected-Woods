using System;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [SerializeField] private EnemyIsHitByPlayer enemyIsHitByPlayer;
    public event Action OnEnemyDeath;
    public static event Action<string> OnEnemyDeathByWeapon;
    public event Action OnEnemyTakesDamage;

    public float enemyCurrentHP = 0f;
    public float enemyMaxHP = 0f; //done
    public float enemyHPPerWave = 0f; //not used  
    public float enemyDamage = 0f; //done
    public float enemyDamagePerWave = 0f; //not used
    public float enemyAttackSpeed = 0f; //not used
    public float enemyAttackRange = 0f; //not used
    public float enemyMoveSpeed = 0f; //done
    public float enemyKnockbackResistance = 0f; //done
    public float enemyLightDropped = 0f; //done
    public float enemyLightDropChance = 0f; //done
    //public float enemyLootCrateDropRate = 0f;
    //public float enemyConsumableDropRate = 0f;
    public float enemyXPGainOnKill = 0f;

    public bool isKnockedBack;


    private void Start()
    {
        enemyCurrentHP = enemyMaxHP;
    }
    public void TakeDamage(float damage)
    {
        enemyCurrentHP -= damage;
        OnEnemyTakesDamage?.Invoke();
        if (enemyCurrentHP <= 0f)
        {
            OnEnemyDeath?.Invoke();
            string lastWeaponHit = enemyIsHitByPlayer.lastWeaponHit;
            OnEnemyDeathByWeapon?.Invoke(lastWeaponHit);
        }
    }
}
