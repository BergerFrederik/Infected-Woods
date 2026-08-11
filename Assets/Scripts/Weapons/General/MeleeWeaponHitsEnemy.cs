using System;
using UnityEngine;


public class MeleeWeaponHitsEnemy : MonoBehaviour
{
    public static event Action<EnemyStats, WeaponStats> OnMeleeWeaponHitsEnemy;
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Enemy"))
        {
            if (collider.TryGetComponent<EnemyStats>(out EnemyStats enemyStats))
            {
                OnMeleeWeaponHitsEnemy?.Invoke(enemyStats, this.gameObject.GetComponent<WeaponStats>());
            }
        }
    }
}
