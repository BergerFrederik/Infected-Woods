using UnityEngine;

public class DamageCalculation : MonoBehaviour
{
    public float CalculateDamageDealtToEnemy(
        WeaponStats weaponStats, 
        PlayerStats playerStats,
        EnemyStats enemyStats)
    {
        float weaponDamge = ComputeWeaponDamage(weaponStats, playerStats);
        float totalDamage = ComputeTotalDamage(weaponDamge, weaponStats, playerStats);
        return totalDamage;
    }

    private float ComputeWeaponDamage(
        WeaponStats weaponStats,
        PlayerStats playerStats)
    {
        float playerPercentDamage = playerStats.playerDamage;
        float playerMeeleDamage = playerStats.playerMeeleDamage;
        float playerRangedDamage = playerStats.playerRangedDamage;
        float playerMysticDamage = playerStats.playerMysticDamage;

        float increaseByMeeleScaling = playerMeeleDamage * (weaponStats.weaponMeeleDamageScale / 100);
        float increaseByRangedScaling = playerRangedDamage * (weaponStats.weaponMysticDamageScale / 100);
        float increaseByMysticScaling = playerMysticDamage * (weaponStats.weaponRangedDamageScale / 100);

        float newWeaponBaseDamage = weaponStats.weaponBaseDamage + increaseByMeeleScaling + increaseByMysticScaling + increaseByRangedScaling;

        float increaseByPlayerDamage = (playerPercentDamage / 100) * newWeaponBaseDamage;

        float totalDamage = newWeaponBaseDamage + increaseByPlayerDamage;

        return totalDamage;
    }

    private float ComputeTotalDamage(
        float weaponDamage, 
        WeaponStats weaponStats,
        PlayerStats playerStats)
    {
        float totalDamage = weaponDamage;
        if (Random.Range(0f, 100f) <= playerStats.playerCritChance)
        {
            totalDamage = weaponDamage + weaponDamage * weaponStats.weaponCritDamage;            
        }
        return totalDamage;
    }
}
