using System;
using UnityEngine;

public class Item1 : MonoBehaviour
{
    [SerializeField] private float attackSpeedGain;
    [SerializeField] private float movementSpeedLoss;
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
        playerStats.playerAttackSpeed += attackSpeedGain;
        playerStats.playerMovespeed -= movementSpeedLoss;
    }
}
