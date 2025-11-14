using System;
using UnityEngine;


public class MeeleWeaponHitsEnemy : MonoBehaviour
{
    public static event Action<EnemyStats, WeaponStats> OnMeeleWeaponHitsEnemy;
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Enemy"))
        {
            if (collider.TryGetComponent<EnemyStats>(out EnemyStats enemyStats))
            {
                OnMeeleWeaponHitsEnemy?.Invoke(enemyStats, this.gameObject.GetComponent<WeaponStats>());
            }
        }
    }
}
