using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private PlayerGainsHP playerGainsHP;
    public Slider slider;

    private void OnEnable()
    {
        playerGainsHP.OnPlayerWasHealed += SetHealth;
        // player loses HP
        // Max Hp Changed
        // game started, set max hp
    }

    private void OnDisable()
    {
        playerGainsHP.OnPlayerWasHealed -= SetHealth;
    }

    private void SetMaxHealth(float health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    private void SetHealth(float health)
    {
        slider.value = health;
    }
}
