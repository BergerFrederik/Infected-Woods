using System;
using UnityEngine;

public class Bunnyhopping : MonoBehaviour
{
    private GameObject Player;
    private PlayerStats playerStats;
    private PlayerTakesDamage playerTakesDamage;
    [SerializeField] private float movespeed_bonus = 100f;
    [SerializeField] private float bonus_activation_penalty = 3f;

    private bool isBonusActive = false;
    private float damageStartTime;
    
    private void Start()
    {
        Player = this.transform.root.gameObject;
        playerStats = Player.GetComponent<PlayerStats>();
        playerTakesDamage = Player.GetComponentInChildren<PlayerTakesDamage>();
        playerTakesDamage.OnPlayerWasDamaged += ApplyDamagePenalty;
        GameManager.OnRoundOver += ApplyDamagePenalty;
        damageStartTime = Time.time;
    }
    
    private void OnDestroy()
    {
        playerTakesDamage.OnPlayerWasDamaged -= ApplyDamagePenalty;
        GameManager.OnRoundOver -= ApplyDamagePenalty;
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
