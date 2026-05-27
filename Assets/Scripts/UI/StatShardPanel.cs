using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatShardPanel : MonoBehaviour
{
    [SerializeField] private Button[] statBanners;
    [SerializeField] private Image[] statIcons;
    [SerializeField] private TextMeshProUGUI[] statNameTexts;
    [SerializeField] private TextMeshProUGUI[] statValueTexts;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private string[] upgradeableStats;
    [SerializeField] private float[] upgradeableStatsValues;
    
    private Dictionary<string, float> _statGainMap;


    private void OnEnable()
    {
        GetStatMap();
        GetRandomStat();
    }

    private void OnDisable()
    {
        
    }

    private void GetStatMap()
    {
        _statGainMap = new Dictionary<string, float>();
        for (int i = 0; i < upgradeableStats.Length; i++)
        {
            string statName = upgradeableStats[i];
            float statValue = upgradeableStatsValues[i];

            if (!_statGainMap.ContainsKey(statName))
            {
                _statGainMap.Add(statName, statValue);
            }
            else
            {
                Debug.LogWarning($"StatMapper Warnung: Der Key '{statName}' ist doppelt vorhanden und wurde ignoriert.");
            }
        }
    }

    private void GetRandomStat()
    {
        foreach (string stat in upgradeableStats) ;
    }
    
    
    private void ApplyStatsToPlayer(float value, string statName)
    {
        playerStats.ApplyStatsToPlayer(value, statName);
    }



}
