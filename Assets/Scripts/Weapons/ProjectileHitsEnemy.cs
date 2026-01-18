using System;
using UnityEngine;

public class ProjectileHitsEnemy : MonoBehaviour
{
    [SerializeField] private Projectile projectile;
    private WeaponStats weaponStats;

    public event Action OnWeaponProjectileHitsEnemyTrigger;

    private void Start()
    {
        weaponStats = projectile.sourceWeaponStats;
    }
    public static event Action<EnemyStats, WeaponStats> OnProjectileHitsEnemy;
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Enemy"))
        {
            if (collider.TryGetComponent<EnemyStats>(out EnemyStats enemyStats))
            {
                OnWeaponProjectileHitsEnemyTrigger?.Invoke();
                OnProjectileHitsEnemy?.Invoke(enemyStats, weaponStats);
                Destroy(this.gameObject);
            }
        }
    }
}
