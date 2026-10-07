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
        // Round the whole time up before splitting, so 59.5s shows 01:00 (not 00:60) and the
        // timer only reads 00:00 at the very end
        int totalSeconds = remainingTime < 0.05f ? 0 : Mathf.CeilToInt(remainingTime);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
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

