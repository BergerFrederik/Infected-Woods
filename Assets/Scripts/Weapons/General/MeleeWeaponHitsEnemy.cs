using System;
using UnityEngine;


public class MeleeWeaponHitsEnemy : MonoBehaviour
{
    public event Action<EnemyStats, bool> OnMeleeWeaponHitsEnemy;

    private Transform cachedRoot;
    public Transform Owner => cachedRoot;

    private void Awake()
    {
        UpdateOwner();
    }

    private void OnTransformParentChanged()
    {
        UpdateOwner();
    }

    // Nur übernehmen, wenn die Waffe unter einem Spieler hängt (nicht im Shop-Slot oder im Bumerang-Flug)
    private void UpdateOwner()
    {
        Transform root = transform.root;
        if (root.GetComponentInChildren<PlayerDealsDamage>() != null)
        {
            cachedRoot = root;
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (cachedRoot == null) return;

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
