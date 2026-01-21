using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    
    public event Action<EnemyStats> OnPlayerCollidesWithEnemy;
    public event Action<Collider2D> OnDropCollected;

    private List<EnemyStats> enemiesInReach = new List<EnemyStats>();
    private float nextDamageTime;

    private void Update()
    {
        for (int i = enemiesInReach.Count - 1; i >= 0; i--)
        {
            if (enemiesInReach[i] == null)
            {
                enemiesInReach.RemoveAt(i);
                continue;
            }
            OnPlayerCollidesWithEnemy?.Invoke(enemiesInReach[i]);
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
            EnemyStats projectileSource = enemyProjectile.GetEnemyStats();
            OnPlayerCollidesWithEnemy?.Invoke(projectileSource);
        }
        
        if (collider.TryGetComponent<EnemyStats>(out EnemyStats enemyStats))
        {
            if (!enemiesInReach.Contains(enemyStats))
            {
                enemiesInReach.Add(enemyStats);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.TryGetComponent<EnemyStats>(out EnemyStats enemyStats))
        {
            enemiesInReach.Remove(enemyStats);
        }
    }
}