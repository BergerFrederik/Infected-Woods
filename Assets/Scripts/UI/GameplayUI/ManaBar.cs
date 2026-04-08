using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ManaBar : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI currentMpText;

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
        currentMpText.text = $"{playerStats.playerCurrentMP} / {playerStats.playerMaxMP}";
    }

    private void SetMana(float mana)
    {
        slider.value = mana;
        currentMpText.text = $"{playerStats.playerCurrentMP} / {playerStats.playerMaxMP}";
    }
}
