using TMPro;
using UnityEngine;

public class PlayerStatsUI : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private TextMeshProUGUI statsText;
    [SerializeField] private TextMeshProUGUI valuesText;
    
    [SerializeField] private float updateInterval = 0.5f;
    private float nextUpdateTime;

    private void Update()
    {
        if (Time.time >= nextUpdateTime)
        {
            SetPlayerStatsToUI();
            nextUpdateTime = Time.time + updateInterval;
        }
    }

    private void SetPlayerStatsToUI()
    {
        StaticHelpers.SetPrimaryStatsToTextUI(statsText, valuesText, playerStats);
    }
}
