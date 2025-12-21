using UnityEngine;

public class Item3 : MonoBehaviour
{
    // +5% Movementspeed; -2.5% Attackspeed
    [SerializeField] private float attackSpeedLoss;
    [SerializeField] private float movementSpeedain;
    private PlayerStats playerStats;
    
    
    private void Start()
    {
        playerStats = transform.root.GetComponent<PlayerStats>();
        ApplyItem();
    }

    private void ApplyItem()
    {
        playerStats.playerAttackSpeed -= attackSpeedLoss;
        playerStats.playerMovespeed += movementSpeedain;
    }
}
