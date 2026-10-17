using System;
using UnityEngine;

public class EnemyIsHitByPlayer : MonoBehaviour
{
    [SerializeField] private ParticleSystem particlesOnHit;
    public String lastWeaponHit;
    
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.TryGetComponent<WeaponStats>(out _))
        {
            // A homing projectile aimed at another enemy just passes through - no hit effect
            if (collider.TryGetComponent(out Projectile projectile) && !projectile.CanHit(transform)) return;

            PlayParticleEffect();
        }
    }

    private void PlayParticleEffect()
    {
        ParticleSystem onHitEffect = Instantiate(particlesOnHit, transform.position, particlesOnHit.transform.rotation);
        onHitEffect.Play();
    }
    
    // Set when the player's damage lands (PlayerDealsDamage), not on collider contact: the contact can come after
    // the damage, and effects and abilities don't touch the enemy with a weapon collider at all.
    public void SetLastWeaponHit(WeaponStats weaponStats)
    {
        lastWeaponHit = weaponStats.weaponWeaponType.ToString();
    }
}
