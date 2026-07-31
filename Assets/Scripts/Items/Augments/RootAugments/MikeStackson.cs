using System;
using UnityEngine;


public class MikeStackson : MonoBehaviour
{
    // Gain Stacks on Kill with melee weapons
    // must not have taken damage for a certain amount of seconds
    // Each Stack gives a certain amount of meleeDamage

    [SerializeField] private float meleeDamageGainedPerStack;
    [SerializeField] private float secondsWithoutDamageTakenRequired;
    [SerializeField] private float chanceToGainStacks;

    public float mikeStacksonStacks;

    private Transform Player;
    private PlayerStats playerStats;
    private RandomRollEvent _randomRollEvent;
    private PlayerTakesDamage playerTakesDamage;
    
    private float lastTimestampOfDamageTaken;

    private void Start()
    {
        Player = this.transform.root;
        playerStats = Player.GetComponent<PlayerStats>();
        _randomRollEvent = Player.GetComponentInChildren<RandomRollEvent>();
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
        if (weaponType != "Melee")
        {
            return;
        }

        if (Time.time - lastTimestampOfDamageTaken >= secondsWithoutDamageTakenRequired)
        {
            if (_randomRollEvent.GetRandomFloatRoll(0f, 100f) > 100f - chanceToGainStacks) 
                //Muss 100- sein, damit luck einen Einfluss hat.
                //Luck erhöht den Roll.
                //Bei 30% muss also höher als 70 gerollt werden
            {
                playerStats.playerMeleeDamage += meleeDamageGainedPerStack;
                mikeStacksonStacks++;
            }
        }
    }

    private void SetLastTimestampOfDamageTaken()
    {
        lastTimestampOfDamageTaken = Time.time;
    }
}
