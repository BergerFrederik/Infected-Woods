using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Wave Configuration")]
    [SerializeField] private List<WaveData> waves; 
    
    [SerializeField] private WaveManager waveManager;
    public static event Action OnWaveInitialized;
    
    private EntitySpawner _entitySpawner;

    private void Awake()
    {
        _entitySpawner = GetComponent<EntitySpawner>();
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
        int index = (int)currentWaveNumber - 1;

        if (index >= 0 && index < waves.Count)
        {
            waveManager.currentWave = waves[index];
            OnWaveInitialized?.Invoke();
        }
    }

    private void StopSpawning()
    {
        waveManager.StopWave();
        _entitySpawner.ClearEnemies(); // Nutzt die neue Clear-Funktion im EntitySpawner
    }
}