using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] Waves;
    [SerializeField] private GameManager gameManager;
    
    public static event Action OnWaveInitialized;
    private void OnEnable()
    {
        GameManager.OnNewWaveRequested += InstantiateCurrentWave;
        gameManager.OnRoundOver += StopSpawning;
    }

    private void OnDisable()
    {
        GameManager.OnNewWaveRequested -= InstantiateCurrentWave;
        gameManager.OnRoundOver += StopSpawning;
    }
    
    private void InstantiateCurrentWave(float currentWaveNumber)
    {
        GameObject newWave = Instantiate(Waves[(int)currentWaveNumber - 1], transform.position, Quaternion.identity);
        newWave.transform.SetParent(transform);
        OnWaveInitialized?.Invoke();
    }

    private void StopSpawning()
    {
        Destroy(this.transform.GetChild(0).gameObject);
    }
}
