using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.TerrainTools;

public class Timer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    
    public static event Action OnRoundOver;
    
    private WaveStats waveStats;
    private GameObject CurrentWave;

    private float remainingTime;
    private bool  isTimerRed;
    private void OnEnable()
    {
        GameObject enemySpawner = GameObject.FindGameObjectWithTag("Spawner");
        Transform enemySpawnerTransform = enemySpawner.transform;
        CurrentWave = enemySpawnerTransform.GetChild(0).gameObject;
        waveStats = CurrentWave.GetComponent<WaveStats>();
        remainingTime = waveStats.waveDuration;
        PaintTimerWhite();
        SetTimerText();
    }


    private void Update()
    {
        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
        }
        if (remainingTime <= 10f && !isTimerRed)
        {
            PaintTimerRed();
        }
        if (remainingTime <= 0)
        {
            remainingTime = 0;
            OnRoundOver?.Invoke();
            this.gameObject.SetActive(false);
        }
        SetTimerText();
    }

    private void PaintTimerRed()
    {
        timerText.color = Color.red;
        isTimerRed = true;
    }

    private void PaintTimerWhite()
    {
        timerText.color = Color.white;
    }

    private void SetTimerText()
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds + 1);
        if (remainingTime < 0.05)
        {
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
}

