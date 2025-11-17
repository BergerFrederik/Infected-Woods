using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Timer timer;
    [SerializeField] private GameObject[] Waves;

    private float currentWaveNumber = 1f;
    void OnEnable()
    {
        InstantiateCurrentWave();
    }
    
    private void InstantiateCurrentWave()
    {
        currentWaveNumber = 1f;
        GameObject newWave = Instantiate(Waves[(int)currentWaveNumber - 1], transform.position, Quaternion.identity);
        newWave.transform.SetParent(transform);
        timer.gameObject.SetActive(true);
    }
}
