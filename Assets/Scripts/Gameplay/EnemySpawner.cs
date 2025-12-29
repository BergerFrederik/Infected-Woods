using System;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Wave")]
    [SerializeField] private GameObject[] Waves;
    
    public static event Action OnWaveInitialized;
    
    private void OnEnable()
    {
        GameManager.OnNewWaveRequested += InstantiateCurrentWave;
        GameManager.OnRoundOver += StopSpawning;
    }

    private void OnDisable()
    {
        GameManager.OnNewWaveRequested -= InstantiateCurrentWave;
        GameManager.OnRoundOver -= StopSpawning;
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
