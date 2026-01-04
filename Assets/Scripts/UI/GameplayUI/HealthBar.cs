using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private PlayerGainsHP playerGainsHP;
    [SerializeField] private PlayerTakesDamage playerTakesDamage;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Slider slider;
    
    private void OnEnable()
    {
        playerGainsHP.OnPlayerWasHealed += SetHealth;
        playerTakesDamage.OnPlayerTakesDamage += SetHealth;
        playerStats.OnMaxHPChanged += SetMaxHealth;
        playerStats.OnCurrentHPChanged += SetHealth;
    }

    private void OnDisable()
    {
        playerGainsHP.OnPlayerWasHealed -= SetHealth;
        playerTakesDamage.OnPlayerTakesDamage -= SetHealth;
        playerStats.OnMaxHPChanged -= SetMaxHealth;
        playerStats.OnCurrentHPChanged -= SetHealth;
    }

    private void SetMaxHealth(float health)
    {
        slider.maxValue = health;
    }

    private void SetHealth(float health)
    {
        slider.value = health;
    }
}
