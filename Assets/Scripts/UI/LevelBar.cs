using UnityEngine;
using UnityEngine.UI;

public class LevelBar : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    public Slider slider;

    public void SetExpNeeded(float maxExp, float currentExp)
    {
        slider.maxValue = maxExp;
        slider.value = currentExp;
    }

    public void SetExp(float exp)
    {
        slider.value = exp;
    }
}

