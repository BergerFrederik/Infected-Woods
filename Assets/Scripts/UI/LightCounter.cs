using TMPro;
using UnityEngine;

public class LightCounter : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private TextMeshProUGUI LightCounterText;

    private void Update()
    {
        LightCounterText.text = playerStats.playerLightAmount.ToString();
    }
}
