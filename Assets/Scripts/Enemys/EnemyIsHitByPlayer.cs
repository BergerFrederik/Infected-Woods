using System;
using UnityEngine;

public class EnemyIsHitByPlayer : MonoBehaviour
{
    [SerializeField] private ParticleSystem particlesOnHit;
    public String lastWeaponHit;
    
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.TryGetComponent<WeaponStats>(out WeaponStats weaponStats))
        {
            PlayParticleEffect();
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

    private void PlayParticleEffect()
    {
        ParticleSystem onHitEffect = Instantiate(particlesOnHit, transform.position, particlesOnHit.transform.rotation);
        onHitEffect.Play();
    }
    
    private void SetLastWeaponHit(WeaponStats weaponStats)
    {
        lastWeaponHit = weaponStats.weaponWeaponType.ToString();
    }
}
