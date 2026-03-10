using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewGroup", menuName = "WaveSystem/Group")]
public class WaveGroupData : ScriptableObject 
{
    [Header("Spawn Settings")]
    public float spawnChance;
    public float spawnTiming;        // Wann startet die Gruppe (Sekunden)
    public int repeating;           // Wie oft wiederholen
    public float repeatingInterval;  // Pause zwischen den Wiederholungen
    public float reduceRepeatingInterval;
    public float minRepeatingInterval;
    public float area;
    public float spawnDistAwayFromEdges;
    public bool spawnEdgeOfMap;

    [Header("Enemy Units")]
    public List<WaveDataUnit> waveUnitsData; // Liste der Gegner in dieser Gruppe

    [Header("Special Flags")]
    public bool isBoss = false;
    public bool isHorde = false;
    public bool isLoot = false;
    public bool isNeutral = false;
    
    [Header("Difficulty")]
    public float minDifficulty;
    public float minWave;
    public float maxWave;
}
