using System;
using UnityEngine;

public class EnemyIsHitByPlayer : MonoBehaviour
{
    [SerializeField] private Transform popUpDamage;

    public String lastWeaponHit;
    
   
    
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Projectile"))
        {
            if (collider.TryGetComponent<WeaponStats>(out WeaponStats weaponStats))
            {
                SetLastWeaponHit(weaponStats);
            }
        } 
        else if (collider.CompareTag("MeeleWeapon"))
        {
            if (collider.TryGetComponent<WeaponStats>(out WeaponStats weaponStats))
            {
                SetLastWeaponHit(weaponStats);
            }
        }
    }
    
    private void SetLastWeaponHit(WeaponStats weaponStats)
    {
        lastWeaponHit = weaponStats.weaponWeaponType;
    }
}
