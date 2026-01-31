using UnityEngine;

public class MasterOfTrifactor : MonoBehaviour
{
    // When hitting an enemy with a Meele Attack gain Ranged and Mystic Damage
    // When hitting an enemy with a Ranged Attack gain Meele and Mystic Damage
    // When hitting an enemy with a Mystic Attack gain Meele and Ranged Damage
    // Stacks infinetely 
    // Removed on Round end

    [SerializeField] private float meeleDamageIncreaseOnHit;
    [SerializeField] private float rangedDamageIncreaseOnHit;
    [SerializeField] private float mysticDamageIncreaseOnHit;

    private Transform Player;
    private PlayerStats playerStats;
    private PlayerDealsDamage playerDealsDamage;

    private float rangedDamageGainedOnRound;
    private float meeleDamageGainedOnRound;
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
            playerStats.playerMeeleDamage += meeleDamageIncreaseOnHit;
            playerStats.playerMysticDamage += mysticDamageIncreaseOnHit;
            meeleDamageGainedOnRound += meeleDamageIncreaseOnHit;
            mysticDamageGainedOnRound += mysticDamageIncreaseOnHit;
        }
        else // Mystic
        {
            playerStats.playerMeeleDamage += meeleDamageIncreaseOnHit;
            playerStats.playerRangedDamage += rangedDamageIncreaseOnHit;
            rangedDamageGainedOnRound += rangedDamageIncreaseOnHit;
            meeleDamageGainedOnRound += meeleDamageIncreaseOnHit;
        }
    }

    private void ResetGainedStatsOnRoundEnd()
    {
        playerStats.playerMeeleDamage -= meeleDamageGainedOnRound;
        playerStats.playerRangedDamage -= rangedDamageGainedOnRound;
        playerStats.playerMysticDamage -= mysticDamageGainedOnRound;
        meeleDamageGainedOnRound = 0f;
        rangedDamageGainedOnRound = 0f;
        mysticDamageGainedOnRound = 0f;
    }
}
