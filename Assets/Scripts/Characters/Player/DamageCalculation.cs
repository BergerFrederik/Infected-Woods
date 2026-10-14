using UnityEngine;

// The one place the damage formula lives. A real hit (the two Calculate methods) and the DPS
// shown on a weapon (WeaponStats) build on the same static steps below, so they cannot drift apart.
public class DamageCalculation : MonoBehaviour
{
    public (float damage, bool isCrit) CalculateCritableDamageDealtToEnemy(
        WeaponStats weaponStats,
        PlayerStats playerStats)
    {
        float weaponDamage = ComputeWeaponDamage(weaponStats, playerStats);
        bool isCrit = UnityEngine.Random.Range(0f, 100f) < GetCritChancePercent(weaponStats, playerStats);
        float totalDamage = weaponDamage;

        if (isCrit)
        {
            totalDamage = ComputeCritDamage(weaponDamage, weaponStats, playerStats);
        }
        return (totalDamage, isCrit);
    }

    public float CalculateNonCritableDamageDealtToEnemy(WeaponStats weaponStats, PlayerStats playerStats)
    {
        return ComputeWeaponDamage(weaponStats, playerStats);
    }

    // Average damage of one hit with crits weighted by their chance. Same steps and the same minimum
    // per hit as a real hit, applied after the crit like in PlayerDealsDamage.
    public static float ComputeAverageDamagePerHit(WeaponStats weaponStats, PlayerStats playerStats)
    {
        float rawDamage = ComputeWeaponDamage(weaponStats, playerStats);
        float rawCritDamage = ComputeCritDamage(rawDamage, weaponStats, playerStats);

        float normalDamage = ClampToMinimumHit(weaponStats, rawDamage);
        float critDamage = ClampToMinimumHit(weaponStats, rawCritDamage);

        float critChance = Mathf.Clamp01(GetCritChancePercent(weaponStats, playerStats) / 100f);
        return normalDamage * (1f - critChance) + critDamage * critChance;
    }

    // Damaging weapons hit for at least 1, support weapons (base damage 0) for at least 0 -
    // see WeaponStats.MinimumHitDamage
    public static float ClampToMinimumHit(WeaponStats weaponStats, float damage)
    {
        return Mathf.Max(weaponStats.MinimumHitDamage, damage);
    }

    // Crit chance in percent points (not clamped, a roll above 100 always crits)
    private static float GetCritChancePercent(WeaponStats weaponStats, PlayerStats playerStats)
    {
        return playerStats.PlayerCritChance + weaponStats.weaponCritChance;
    }

    private static float ComputeCritDamage(
        float weaponDamage,
        WeaponStats weaponStats,
        PlayerStats playerStats)
    {
        float critWeaponDamage = weaponDamage * weaponStats.weaponCritDamage;
        return critWeaponDamage + (critWeaponDamage * playerStats.PlayerCritDamage / 100f);
    }

    private static float ComputeWeaponDamage(
        WeaponStats weaponStats,
        PlayerStats playerStats)
    {
        float playerPercentDamage = playerStats.playerDamage;
        float playerMeleeDamage = playerStats.playerMeleeDamage;
        float playerRangedDamage = playerStats.playerRangedDamage;
        float playerMysticDamage = playerStats.playerMysticDamage;

        float increaseByMeleeScaling = playerMeleeDamage * (weaponStats.weaponMeleeDamageScale / 100f);
        float increaseByRangedScaling = playerRangedDamage * (weaponStats.weaponRangedDamageScale / 100f);
        float increaseByMysticScaling = playerMysticDamage * (weaponStats.weaponMysticDamageScale / 100f);

        float newWeaponBaseDamage = weaponStats.weaponBaseDamage + increaseByMeleeScaling + increaseByMysticScaling + increaseByRangedScaling;

        float increaseByPlayerDamage = (playerPercentDamage / 100f) * newWeaponBaseDamage;

        float totalDamage = newWeaponBaseDamage + increaseByPlayerDamage;

        return totalDamage;
    }
}
