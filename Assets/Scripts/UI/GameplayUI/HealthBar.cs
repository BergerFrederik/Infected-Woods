using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI currentHpText;
    
    
    private void OnEnable()
    {
        playerStats.OnMaxHPChanged += SetMaxHealth;
        playerStats.OnCurrentHPChanged += SetHealth;
    }

    private void OnDisable()
    {
        playerStats.OnMaxHPChanged -= SetMaxHealth;
        playerStats.OnCurrentHPChanged -= SetHealth;
    }

    private void SetMaxHealth(float health)
    {
        slider.maxValue = health;
        currentHpText.text = $"{playerStats.playerCurrentHP} / {playerStats.playerMaxHP}";
    }

    private void SetHealth(float health)
    {
        slider.value = health;
        currentHpText.text = $"{playerStats.playerCurrentHP} / {playerStats.playerMaxHP}";
    }
}
