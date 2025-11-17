using System;
using UnityEngine;

public class Bunnyhopping : MonoBehaviour
{
    private GameObject Player;
    private PlayerStats playerStats;
    [SerializeField] private float movespeed_bonus = 100f;
    [SerializeField] private float bonus_activation_penalty = 3f;

    private bool isBonusActive = false;
    private float damageStartTime;
    

    private void OnEnable()
    {
        Player = GameObject.Find("Player");
        playerStats = Player.GetComponent<PlayerStats>();
        PlayerTakesDamage.OnPlayerTakesDamage += ApplyDamagePenalty;
        Timer.OnRoundOver += ApplyDamagePenalty;
        damageStartTime = Time.time;
    }

    private void OnDisable()
    {
        PlayerTakesDamage.OnPlayerTakesDamage -= ApplyDamagePenalty;
        Timer.OnRoundOver -= ApplyDamagePenalty;
    }

    private void Update()
    {
        if (!isBonusActive && Time.time - damageStartTime >= bonus_activation_penalty)
        {
            SetBonusMovespeed();
        }      
    }

    private void ApplyDamagePenalty()
    {
        damageStartTime = Time.time;
        if (isBonusActive)
        {
            playerStats.playerMovespeed -= movespeed_bonus;
            isBonusActive = false;
        }
    }

    private void SetBonusMovespeed()
    {        
        playerStats.playerMovespeed += movespeed_bonus;
        isBonusActive = true;        
    }
}
