using System;
using UnityEngine;

public class MikeStackson : MonoBehaviour
{
    // Gain Stacks on Kill with meele weapons
    // must not have taken damage for a certain amount of seconds
    // Each Stack gives a certain amount of meeleDamage

    [SerializeField] private float meeleDamageGainedPerStack;
    [SerializeField] private float secondsWithoutDamageTakenRequired;

    public float mikeStacksonStacks;

    private Transform Player;
    private PlayerStats playerStats;
    private PlayerTakesDamage playerTakesDamage;
    
    private float lastTimestampOfDamageTaken;

    private void Start()
    {
        Player = this.transform.root;
        playerStats = Player.GetComponent<PlayerStats>();

        playerTakesDamage = Player.GetComponentInChildren<PlayerTakesDamage>();

        EnemyStats.OnEnemyDeathByWeapon += PerformAugment;
        playerTakesDamage.OnPlayerWasDamaged += SetLastTimestampOfDamageTaken;
    }

    private void OnDestroy()
    {
        EnemyStats.OnEnemyDeathByWeapon -= PerformAugment;
        playerTakesDamage.OnPlayerWasDamaged -= SetLastTimestampOfDamageTaken;
    }

    private void PerformAugment(String weaponType)
    {
        if (weaponType == "Meele")
        {
            if (Time.time - lastTimestampOfDamageTaken >= secondsWithoutDamageTakenRequired)
            {
                playerStats.playerMeeleDamage += meeleDamageGainedPerStack;
                mikeStacksonStacks++;
            }
        }
    }

    private void SetLastTimestampOfDamageTaken()
    {
        lastTimestampOfDamageTaken = Time.time;
    }
}
