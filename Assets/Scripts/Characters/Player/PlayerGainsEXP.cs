using System;
using UnityEngine;

public class PlayerGainsEXP : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;

    public event Action<float> OnPlayerGainsXP;
    public event Action<float> OnPlayerLeveledUp;

    public void GiveXPByEnemyToPlayer(EnemyStats enemyStats)
    {
        float xpGained = enemyStats.enemyXPGainOnKill;
        playerStats.playerOverallXP += xpGained;
        playerStats.playerCurrentXP += xpGained;
        UpdatePlayerLevelAndEXP();
        OnPlayerGainsXP?.Invoke(playerStats.playerCurrentXP);
    }

    private void UpdatePlayerLevelAndEXP()
    {
        float requiredEXP = GetRequiredXPForNextLevel();
        if (playerStats.playerCurrentXP >= requiredEXP)
        {
            playerStats.playerLevel++;
            float newRequiredEXP = GetRequiredXPForNextLevel();
            OnPlayerLeveledUp?.Invoke(newRequiredEXP);
            playerStats.playerLevelsGained++;
            playerStats.playerCurrentXP -= requiredEXP;                        
            if (playerStats.playerCurrentXP > newRequiredEXP)
            {
                UpdatePlayerLevelAndEXP();
            }
        }        
    }

    private float GetRequiredXPForNextLevel()
    {
        float requiredXP = playerStats.playerBaseXP * Mathf.Pow(playerStats.playerLevelMultiplier, playerStats.playerLevel - 1);
        return requiredXP;
    }
}
