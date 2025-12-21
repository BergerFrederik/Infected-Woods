using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    
    public event Action<EnemyStats> OnPlayerCollidesWithEnemy;
    public event Action<Collider2D> OnDropCollected;


    private void OnTriggerStay2D(Collider2D collider)
    {     
        if (collider.TryGetComponent<EnemyStats>(out EnemyStats enemyStats))
        {
            OnPlayerCollidesWithEnemy?.Invoke(enemyStats);
        }            
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Drop"))
        {
            OnDropCollected?.Invoke(collider);
        }

        if (collider.TryGetComponent<EnemyProjectile>(out EnemyProjectile enemyProjectile))
        {
            EnemyStats enemyStats = enemyProjectile.GetEnemyStats();
            OnPlayerCollidesWithEnemy?.Invoke(enemyStats);
        }
    }  
}
