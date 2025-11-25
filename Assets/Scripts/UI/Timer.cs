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
    
    private bool  isTimerRed;
    private void OnEnable()
    {
        GameManager.OnTimerChanged += SetTimerText;
    }

    private void OnDisable()
    {
        GameManager.OnTimerChanged -= SetTimerText;
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

    private void SetTimerText(float remainingTime)
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds + 1);
        if (remainingTime < 0.05)
        {
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        if (remainingTime <= 10f && !isTimerRed)
        {
            PaintTimerRed();
        }
    }
}

