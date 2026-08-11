using System;
using UnityEngine;


public class MeleeWeaponHitsEnemy : MonoBehaviour
{
    public event Action<EnemyStats, bool> OnMeleeWeaponHitsEnemy;
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Enemy"))
        {
            if (collider.TryGetComponent<EnemyStats>(out EnemyStats enemyStats))
            {
                WeaponStats weaponStats = this.gameObject.GetComponent<WeaponStats>();
                PlayerDealsDamage playerDealsDamage = transform.root.GetComponentInChildren<PlayerDealsDamage>();
                PlayerGainsHP playerGainsHP = transform.root.GetComponentInChildren<PlayerGainsHP>();

                bool didCrit = playerDealsDamage?.ApplyDamageToEnemy(enemyStats, weaponStats) ?? false;
                playerGainsHP?.TryApplyLifesteal(enemyStats, weaponStats);

                OnMeleeWeaponHitsEnemy?.Invoke(enemyStats, didCrit);
            }
        }
    }
}
