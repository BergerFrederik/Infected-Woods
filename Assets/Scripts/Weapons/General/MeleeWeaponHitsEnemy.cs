using System;
using UnityEngine;


public class MeleeWeaponHitsEnemy : MonoBehaviour
{
    public event Action<EnemyStats, bool> OnMeleeWeaponHitsEnemy;

    private Transform cachedRoot;

    private void Awake()
    {
        cachedRoot = transform.root;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Enemy"))
        {
            if (collider.TryGetComponent<EnemyStats>(out EnemyStats enemyStats))
            {
                WeaponStats weaponStats = this.gameObject.GetComponent<WeaponStats>();
                PlayerDealsDamage playerDealsDamage = cachedRoot.GetComponentInChildren<PlayerDealsDamage>();
                PlayerGainsHP playerGainsHP = cachedRoot.GetComponentInChildren<PlayerGainsHP>();

                bool didCrit = playerDealsDamage?.ApplyCritableDamageToEnemy(enemyStats, weaponStats) ?? false;
                playerGainsHP?.TryApplyLifesteal(enemyStats, weaponStats);

                OnMeleeWeaponHitsEnemy?.Invoke(enemyStats, didCrit);
            }
        }
    }
}
