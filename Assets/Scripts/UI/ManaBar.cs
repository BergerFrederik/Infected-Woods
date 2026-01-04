using UnityEngine;
using UnityEngine.UI;

public class ManaBar : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Slider slider;

    private void OnEnable()
    {
        playerStats.OnMaxMPChanged += SetMaxMana;
        playerStats.OnCurrentMPChanged += SetMana;
    }

    private void OnDisable()
    {
        playerStats.OnMaxMPChanged -= SetMaxMana;
        playerStats.OnCurrentMPChanged -= SetMana;
    }

    private void SetMaxMana(float mana)
    {
        slider.maxValue = mana;
    }

    private void SetMana(float mana)
    {
        slider.value = mana;
    }
}
