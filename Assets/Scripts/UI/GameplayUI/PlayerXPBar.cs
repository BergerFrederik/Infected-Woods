using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))] 
public class PlayerXPBar : MonoBehaviour
{
    [SerializeField] private PlayerGainsEXP playerGainsExp;
    private Slider _slider;

    private Slider newSlider => _slider ??= GetComponent<Slider>();
    private void SetEXPNeeded(float maxExp)
    {
        newSlider.maxValue = maxExp;
    }

    private void SetEXP(float currentEXP)
    {
        newSlider.value = currentEXP;
    }

    private void OnEnable()
    {
        playerGainsExp.OnPlayerGainsXP += SetEXP;
        playerGainsExp.OnPlayerLeveledUp += SetEXPNeeded;

        // OnPlayerLeveledUp only fires after a level-up - before the first one the bar would
        // keep the slider's Inspector max instead of the XP actually needed
        SetEXPNeeded(playerGainsExp.GetRequiredXPForNextLevel());
    }

    private void OnDisable() 
    {
        playerGainsExp.OnPlayerGainsXP -= SetEXP;
        playerGainsExp.OnPlayerLeveledUp -= SetEXPNeeded;
    }
}

