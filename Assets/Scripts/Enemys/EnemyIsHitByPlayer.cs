using System;
using UnityEngine;

public class EnemyIsHitByPlayer : MonoBehaviour
{
    public String lastWeaponHit;
    
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.TryGetComponent<WeaponStats>(out WeaponStats weaponStats))
        {
            if (collider.CompareTag("Projectile"))
            {
                SetLastWeaponHit(weaponStats);
            }
            else if ((weaponStats.weaponWeaponType == WeaponStats.weaponTypeOptions.Melee))
            {
                SetLastWeaponHit(weaponStats);
            }
        }

    }
    
    private void SetLastWeaponHit(WeaponStats weaponStats)
    {
        lastWeaponHit = weaponStats.weaponWeaponType.ToString();
    }
}
