using UnityEngine;

public class MasterOfTrifactor : MonoBehaviour
{
    // When hitting an enemy with a Melee Attack gain Ranged and Mystic Damage
    // When hitting an enemy with a Ranged Attack gain Melee and Mystic Damage
    // When hitting an enemy with a Mystic Attack gain Melee and Ranged Damage
    // Stacks infinetely 
    // Removed on Round end

    [SerializeField] private float meleeDamageIncreaseOnHit;
    [SerializeField] private float rangedDamageIncreaseOnHit;
    [SerializeField] private float mysticDamageIncreaseOnHit;

    private Transform Player;
    private PlayerStats playerStats;
    private PlayerDealsDamage playerDealsDamage;

    private float rangedDamageGainedOnRound;
    private float meleeDamageGainedOnRound;
    private float mysticDamageGainedOnRound;

    private void Start()
    {
        Player = this.transform.root;
        playerStats = Player.GetComponentInChildren<PlayerStats>();
        playerDealsDamage = Player.GetComponentInChildren<PlayerDealsDamage>();

        playerDealsDamage.OnPlayerHitsEnemyWithWeapon += IncreaseStatsOnHit;
        GameManager.OnRoundOver += ResetGainedStatsOnRoundEnd;
    }

    private void OnDestroy()
    {
        playerDealsDamage.OnPlayerHitsEnemyWithWeapon -= IncreaseStatsOnHit;
        GameManager.OnRoundOver -= ResetGainedStatsOnRoundEnd;
    }

    private void IncreaseStatsOnHit(WeaponStats weaponStats)
    {
        if (weaponStats.weaponWeaponType == WeaponStats.weaponTypeOptions.Melee)
        {
            playerStats.playerRangedDamage += rangedDamageIncreaseOnHit;
            playerStats.playerMysticDamage += mysticDamageIncreaseOnHit;
            rangedDamageGainedOnRound += rangedDamageIncreaseOnHit;
            mysticDamageGainedOnRound += mysticDamageIncreaseOnHit;
        }
        else if (weaponStats.weaponWeaponType == WeaponStats.weaponTypeOptions.Ranged)
        {
            playerStats.playerMeleeDamage += meleeDamageIncreaseOnHit;
            playerStats.playerMysticDamage += mysticDamageIncreaseOnHit;
            meleeDamageGainedOnRound += meleeDamageIncreaseOnHit;
            mysticDamageGainedOnRound += mysticDamageIncreaseOnHit;
        }
        else // Mystic
        {
            playerStats.playerMeleeDamage += meleeDamageIncreaseOnHit;
            playerStats.playerRangedDamage += rangedDamageIncreaseOnHit;
            rangedDamageGainedOnRound += rangedDamageIncreaseOnHit;
            meleeDamageGainedOnRound += meleeDamageIncreaseOnHit;
        }
    }

    private void ResetGainedStatsOnRoundEnd()
    {
        playerStats.playerMeleeDamage -= meleeDamageGainedOnRound;
        playerStats.playerRangedDamage -= rangedDamageGainedOnRound;
        playerStats.playerMysticDamage -= mysticDamageGainedOnRound;
        meleeDamageGainedOnRound = 0f;
        rangedDamageGainedOnRound = 0f;
        mysticDamageGainedOnRound = 0f;
    }
}
