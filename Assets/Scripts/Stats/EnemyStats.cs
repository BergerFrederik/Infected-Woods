using System;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [SerializeField] private EnemyIsHitByPlayer enemyIsHitByPlayer;
    private GameManager gameManager;
    public event Action OnEnemyDeath;
    public static event Action<string> OnEnemyDeathByWeapon;
    public event Action OnEnemyTakesDamage;

    public float enemyCurrentHP = 0f; //done
    public float enemyMaxHP = 0f; //done
    public float enemyHPPerWave = 0f; //done 
    public float enemyDamage = 0f; //done
    public float enemyDamagePerWave = 0f; //done
    public float enemyMoveSpeed = 0f; //done
    public float enemyKnockbackResistance = 0f; //done
    public float enemyLightDropped = 0f; //done
    //public float enemyLootCrateDropRate = 0f;
    //public float enemyConsumableDropRate = 0f;
    public float enemyXPGainOnKill = 0f; //done

    public bool isKnockedBack;


    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        enemyMaxHP += enemyHPPerWave * (gameManager.currentWaveNumber - 1f);
        enemyDamage += enemyDamagePerWave * (gameManager.currentWaveNumber - 1f);
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
