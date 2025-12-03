using UnityEngine;

public class AidingLight : MonoBehaviour
{
    // + 20% Heal and Shield Power
    [SerializeField] private float healPowerGain;
    [SerializeField] private float shieldPowerGain;

    private Transform Player;
    private PlayerStats playerStats;
    private void Start()
    {
        Player = this.transform.root;
        playerStats = Player.GetComponent<PlayerStats>();
        GiveStatsToPlayer();
    }

    private void GiveStatsToPlayer()
    {
        playerStats.playerHealPower += healPowerGain;
        playerStats.playerShieldPower += shieldPowerGain;
    }
}
