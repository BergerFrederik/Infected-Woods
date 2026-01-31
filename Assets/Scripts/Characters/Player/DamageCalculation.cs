using UnityEngine;

public class DamageCalculation : MonoBehaviour
{
    public (float damage, bool isCrit) CalculateDamageDealtToEnemy(
        WeaponStats weaponStats, 
        PlayerStats playerStats)
    {
        float weaponDamage = ComputeWeaponDamage(weaponStats, playerStats);
        bool isCrit = UnityEngine.Random.Range(1f, 100f) < playerStats.playerCritChance + weaponStats.weaponCritChance;
        float totalDamage = weaponDamage;

        if (isCrit)
        {
            totalDamage = weaponDamage + (weaponDamage * weaponStats.weaponCritDamage);
        }
        return (totalDamage, isCrit);
    }

    private float ComputeWeaponDamage(
        WeaponStats weaponStats,
        PlayerStats playerStats)
    {
        float playerPercentDamage = playerStats.playerDamage;
        float playerMeeleDamage = playerStats.playerMeeleDamage;
        float playerRangedDamage = playerStats.playerRangedDamage;
        float playerMysticDamage = playerStats.playerMysticDamage;

        float increaseByMeeleScaling = playerMeeleDamage * (weaponStats.weaponMeeleDamageScale / 100f);
        float increaseByRangedScaling = playerRangedDamage * (weaponStats.weaponMysticDamageScale / 100f);
        float increaseByMysticScaling = playerMysticDamage * (weaponStats.weaponRangedDamageScale / 100f);

        float newWeaponBaseDamage = weaponStats.weaponBaseDamage + increaseByMeeleScaling + increaseByMysticScaling + increaseByRangedScaling;

        float increaseByPlayerDamage = (playerPercentDamage / 100f) * newWeaponBaseDamage;

        float totalDamage = newWeaponBaseDamage + increaseByPlayerDamage;

        return totalDamage;
    }
}
