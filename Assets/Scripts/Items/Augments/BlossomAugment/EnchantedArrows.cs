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

        playerDealsDamage.OnPlayerHitsEnemy += ReduceCooldownOnHit;
    }

    private void OnDestroy()
    {
        playerDealsDamage.OnPlayerHitsEnemy -= ReduceCooldownOnHit;
    }

    private void ReduceCooldownOnHit()
    {
        if (characterStats.remainingCooldown > 0.5f)
        {
            characterStats.remainingCooldown -= cooldownSecondsReducedOnHit;
        }
    }
}
