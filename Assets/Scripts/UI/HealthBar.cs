using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private PlayerGainsHP playerGainsHP;
    [SerializeField] private PlayerTakesDamage playerTakesDamage;
    [SerializeField] private GameManager gameManager;
    public Slider slider;

    private void OnEnable()
    {
        playerGainsHP.OnPlayerWasHealed += SetHealth;
        playerTakesDamage.OnPlayerTakesDamage += SetHealth;

        // Max Hp Changed
    }

    private void OnDisable()
    {
        playerGainsHP.OnPlayerWasHealed -= SetHealth;
        playerTakesDamage.OnPlayerTakesDamage -= SetHealth;

        // Max Hp Changed
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
