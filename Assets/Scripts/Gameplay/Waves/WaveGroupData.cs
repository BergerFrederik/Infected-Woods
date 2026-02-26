using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewGroup", menuName = "WaveSystem/Group")]
public class WaveGroupData : ScriptableObject 
{
    [Header("Spawn Settings")]
    public float spawnTiming = 1f;        // Wann startet die Gruppe (Sekunden)
    public int repeating = 999;           // Wie oft wiederholen
    public float repeatingInterval = 3f;  // Pause zwischen den Wiederholungen
    public float spawnRadius = 3;

    [Header("Enemy Units")]
    public List<WaveUnitData> waveUnitsData; // Liste der Gegner in dieser Gruppe

    [Header("Special Flags")]
    public bool isBoss = false;
    public bool isHorde = false;
}

// Eine kleine Hilfsklasse, um Gegner und Anzahl zu bündeln
[System.Serializable]
public class WaveUnitData 
{
    public GameObject enemyPrefab;
    public int minNumber;
    public int maxNumber;
}