using TMPro;
using UnityEngine;

public class PlayerStatsUI : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private TextMeshProUGUI playerStatsUIText;
    [SerializeField] private LevelPanel levelPanel;

    private void Update()
    {
        levelPanel.SetTextToPrimayStats(playerStatsUIText);
    }
}
