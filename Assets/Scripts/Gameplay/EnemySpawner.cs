using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Wave Configuration")]
    [SerializeField] private List<WaveData> waves; 
    
    [SerializeField] private WaveManager waveManager;
    public static event Action<WaveData> OnWaveInitialized;
    
    private EntitySpawner _entitySpawner;

    private void Awake()
    {
        _entitySpawner = GetComponent<EntitySpawner>();
        
        // Sicherheitscheck: Falls der WaveManager im Inspector vergessen wurde
        if (waveManager == null) 
            waveManager = GetComponent<WaveManager>();
    }

    private void OnEnable()
    {
        GameManager.OnNewWaveRequested += SetupNextWave;
        GameManager.OnRoundOver += StopSpawning;
    }

    private void OnDisable()
    {
        GameManager.OnNewWaveRequested -= SetupNextWave;
        GameManager.OnRoundOver -= StopSpawning;
    }
    
    private void SetupNextWave(float currentWaveNumber)
    {
        // Index berechnen (Welle 1 -> Index 0)
        int index = Mathf.RoundToInt(currentWaveNumber) - 1;

        if (index >= 0 && index < waves.Count)
        {
            // 1. Welle im Manager setzen
            WaveData waveToStart = waves[index];
            waveManager.StartWave(waveToStart);
            OnWaveInitialized?.Invoke(waveToStart);
            Debug.Log($"Welle {currentWaveNumber} gestartet.");
        }
        else
        {
            Debug.LogWarning($"Welle {currentWaveNumber} wurde angefordert, existiert aber nicht in der Liste!");
        }
    }

    private void StopSpawning()
    {
        // Stoppt den internen Timer und die Update-Logik im WaveManager
        waveManager.EndWave(); 
        
        // Entfernt alle Gegner von der Map
        _entitySpawner.ClearEnemies(); 
    }
}