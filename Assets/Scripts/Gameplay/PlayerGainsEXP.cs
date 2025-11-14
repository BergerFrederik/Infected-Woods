using UnityEngine;

public class PlayerGainsEXP : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;

    public void GiveXPByEnemyToPlayer(EnemyStats enemyStats)
    {
        float xpGained = enemyStats.enemyXPGainOnKill;
        playerStats.playerOverallXP += xpGained;
        playerStats.playerCurrentXP += xpGained;
    }
}
