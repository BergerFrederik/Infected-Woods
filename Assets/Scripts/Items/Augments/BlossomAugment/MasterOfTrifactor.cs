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

    private StackCounter _stacks;

    private void Start()
    {
        Player = this.transform.root;
        playerStats = Player.GetComponentInChildren<PlayerStats>();
        _stacks = StackCounter.Uncapped(playerStats);
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
        _stacks.TryAdd(out int wholeStacksGained);
        if (wholeStacksGained == 0) return;

        float meleeGain = meleeDamageIncreaseOnHit * wholeStacksGained;
        float rangedGain = rangedDamageIncreaseOnHit * wholeStacksGained;
        float mysticGain = mysticDamageIncreaseOnHit * wholeStacksGained;

        if (weaponStats.weaponWeaponType == WeaponStats.weaponTypeOptions.Melee)
        {
            playerStats.playerRangedDamage += rangedGain;
            playerStats.playerMysticDamage += mysticGain;
            rangedDamageGainedOnRound += rangedGain;
            mysticDamageGainedOnRound += mysticGain;
        }
        else if (weaponStats.weaponWeaponType == WeaponStats.weaponTypeOptions.Ranged)
        {
            playerStats.playerMeleeDamage += meleeGain;
            playerStats.playerMysticDamage += mysticGain;
            meleeDamageGainedOnRound += meleeGain;
            mysticDamageGainedOnRound += mysticGain;
        }
        else // Mystic
        {
            playerStats.playerMeleeDamage += meleeGain;
            playerStats.playerRangedDamage += rangedGain;
            rangedDamageGainedOnRound += rangedGain;
            meleeDamageGainedOnRound += meleeGain;
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
        _stacks.Reset();
    }
}
