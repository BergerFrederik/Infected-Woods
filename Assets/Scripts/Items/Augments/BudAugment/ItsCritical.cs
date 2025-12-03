using System;
using UnityEngine;

public class ItsCritical : MonoBehaviour
{
    [SerializeField] private float critChanceGained = 50;

    private Transform Player;
    private PlayerStats playerStats;

    private void Start()
    {
        Player = this.transform.root;
        playerStats =  Player.GetComponent<PlayerStats>();
        playerStats.playerCritChance += critChanceGained;
    }
}
