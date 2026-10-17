using UnityEngine;

public class EnchantedArrows : MonoBehaviour
{
    [SerializeField] private float cooldownSecondsReducedOnHit; 
    private Transform PlayerTransform;
    private CharacterStats characterStats;
    private PlayerDealsDamage playerDealsDamage;

    private void Start()
    {
        PlayerTransform = this.transform.root;
        characterStats = PlayerTransform.GetComponentInChildren<CharacterStats>();
        playerDealsDamage = PlayerTransform.GetComponentInChildren<PlayerDealsDamage>();

        playerDealsDamage.OnPlayerHitsEnemyWithWeapon += ReduceCooldownOnHit;
    }

    private void OnDestroy()
    {
        playerDealsDamage.OnPlayerHitsEnemyWithWeapon -= ReduceCooldownOnHit;
    }

    private void ReduceCooldownOnHit(WeaponStats weaponStats)
    {
        // Only weapon hits count - not abilities (they would shorten their own cooldown) and not effect ticks
        if (!weaponStats.IsWeaponHit) return;

        if (characterStats.remainingCooldown > 0.5f)
        {
            characterStats.remainingCooldown -= cooldownSecondsReducedOnHit;
        }
    }
}
