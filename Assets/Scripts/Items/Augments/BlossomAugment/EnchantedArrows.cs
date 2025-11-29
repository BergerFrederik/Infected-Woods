using UnityEngine;

public class EnchantedArrows : MonoBehaviour
{
    [SerializeField] private float cooldownSecondsReducedOnHit; 
    private Transform PlayerTransform;
    private CharacterStats characterStats;
    private PlayerDealsDamage playerDealsDamage;

    private void Awake()
    {
        PlayerTransform = this.transform.root;
        characterStats = PlayerTransform.GetComponentInChildren<CharacterStats>();
        playerDealsDamage = PlayerTransform.GetComponentInChildren<PlayerDealsDamage>();
    }
    private void OnEnable()
    {
       playerDealsDamage.OnPlayerHitsEnemy += ReduceCooldownOnHit;
    }

    private void OnDisable()
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
