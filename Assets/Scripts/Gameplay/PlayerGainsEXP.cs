using UnityEngine;

public class PlayerGainsEXP : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;

    private float oldPlayerEXP = 0;

    public void GiveXPByEnemyToPlayer(EnemyStats enemyStats)
    {
        float xpGained = enemyStats.enemyXPGainOnKill;
        playerStats.playerOverallXP += xpGained;
        playerStats.playerCurrentXP += xpGained;
    }
}
