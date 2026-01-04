using TMPro;
using UnityEngine;


public class Timer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    
    private bool  isTimerRed;
    private void OnEnable()
    {
        GameManager.OnTimerChanged += SetTimerText;
        GameManager.OnRoundOver += ResetTimerText;
    }

    private void OnDisable()
    {
        GameManager.OnTimerChanged -= SetTimerText;
        GameManager.OnRoundOver -= ResetTimerText;
    }

    private void PaintTimerRed()
    {
        timerText.color = Color.red;
        isTimerRed = true;
    }

    private void PaintTimerWhite()
    {
        timerText.color = Color.white;
        isTimerRed = false;
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

    private void ResetTimerText()
    {
        SetTimerText(0f);
        PaintTimerWhite();
    }
}

