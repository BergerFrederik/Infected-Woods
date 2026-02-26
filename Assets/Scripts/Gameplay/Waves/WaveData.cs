using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewWave", menuName = "WaveSystem/Wave")]
public class WaveData : ScriptableObject 
{
    [Header("Wave Configuration")]
    public float waveDuration = 20f;  // Gesamtdauer der Welle
    public int maxEnemies = 100;      // Maximale Gegner gleichzeitig auf Map

    [Header("Groups in this Wave")]
    public List<WaveGroupData> groupsData; // Liste aller Gruppen für diese Welle
}