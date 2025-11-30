using UnityEngine;

public class AidingLight : MonoBehaviour
{
    // + 20% Heal and Shield Power
    [SerializeField] private float healPowerGain;
    [SerializeField] private float shieldPowerGain;

    private Transform Player;
    private PlayerStats playerStats;
    private void Awake()
    {
        Player = this.transform.root;
        playerStats = this.GetComponent<PlayerStats>();
    }

    private void Start()
    {
        playerStats.playerHealPower += healPowerGain;
        playerStats.playerShieldPower += shieldPowerGain;
    }
}
