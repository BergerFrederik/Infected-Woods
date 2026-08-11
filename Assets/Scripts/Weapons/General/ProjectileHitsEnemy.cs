using System;
using UnityEngine;

public class ProjectileHitsEnemy : MonoBehaviour
{
    [SerializeField] private Projectile projectile;
    [SerializeField] private WeaponStats weaponStats;

    public event Action OnWeaponProjectileHitsEnemyTrigger;

    private Transform ownerRoot;

    public void SetOwner(Transform ownerRoot)
    {
        this.ownerRoot = ownerRoot;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Enemy"))
        {
            if (collider.TryGetComponent<EnemyStats>(out EnemyStats enemyStats))
            {
                OnWeaponProjectileHitsEnemyTrigger?.Invoke();

                if (ownerRoot != null)
                {
                    PlayerDealsDamage playerDealsDamage = ownerRoot.GetComponentInChildren<PlayerDealsDamage>();
                    PlayerGainsHP playerGainsHP = ownerRoot.GetComponentInChildren<PlayerGainsHP>();

                    playerDealsDamage?.ApplyDamageToEnemy(enemyStats, weaponStats);
                    playerGainsHP?.TryApplyLifesteal(enemyStats, weaponStats);
                }

                Destroy(this.gameObject);
            }
        }
    }
}
