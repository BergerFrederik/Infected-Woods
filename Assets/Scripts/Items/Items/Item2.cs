using UnityEngine;

public class Item2 : MonoBehaviour
{
    // + 2 hp - 2 hp reg
    [SerializeField] private float hpGain;
    [SerializeField] private float hpRegLoss;
    private GameObject Player;
    private PlayerStats playerStats;
    
    
    private void Start()
    {
        Player = this.transform.root.gameObject;
        playerStats = Player.GetComponent<PlayerStats>();

        ApplyItem();
    }

    private void ApplyItem()
    {
        playerStats.playerMaxHP += hpGain;
        playerStats.playerHPRegeneration -= hpRegLoss;
    }
}
