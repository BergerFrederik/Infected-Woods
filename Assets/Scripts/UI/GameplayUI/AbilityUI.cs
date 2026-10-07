using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityUI : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Image cooldownOverlay;
    [SerializeField] private Image abilityActiveOverlay;

    [Header("Stacks")]
    [SerializeField] private TextMeshProUGUI stackCounterText;
    [SerializeField] private TextMeshProUGUI stackCountdownText;

    private bool _stackUIEnabled;
    private int _shownStackCount = -1;
    private int _shownCountdownSeconds = -1;

    // Set by CharacterStats every frame from its own remaining cooldown, so changes to it
    // (e.g. Enchanted Arrows) show up right away and the overlay is empty when it hits 0
    public void SetAbilityCooldownUI(float remainingCooldown, float cooldown)
    {
        cooldownOverlay.fillAmount = cooldown > 0f ? Mathf.Clamp01(remainingCooldown / cooldown) : 0f;
    }

    public void StartActiveAbilityUI()
    {
        abilityActiveOverlay.enabled = true;
    }

    public void EndActiveAbilityUI()
    {
        abilityActiveOverlay.enabled = false;
    }

    public void StopAbilityCooldownUI()
    {
        cooldownOverlay.fillAmount = 0f;
    }

    // The stack texts are disabled in the scene, an ability that can stack switches them on
    public void EnableStackUI()
    {
        if (stackCounterText == null || stackCountdownText == null)
        {
            Debug.LogWarning("AbilityUI: assign the stack counter and stack countdown texts to show stacks.", this);
            return;
        }

        stackCounterText.gameObject.SetActive(true);
        stackCountdownText.gameObject.SetActive(true);
        _stackUIEnabled = true;
    }

    // Meant to be called every frame, the text is only rewritten when the shown number changes
    public void SetStackCount(float stacks)
    {
        if (!_stackUIEnabled) return;

        int roundedStacks = Mathf.RoundToInt(stacks);
        if (roundedStacks == _shownStackCount) return;

        _shownStackCount = roundedStacks;
        stackCounterText.SetText("{0:0}", roundedStacks);
    }

    // Seconds until the stacks run out, empty while there is nothing to count down
    public void SetStackCountdown(float secondsRemaining)
    {
        if (!_stackUIEnabled) return;

        // Rounded up, so the text never reads 0 while the stacks are still alive
        int seconds = secondsRemaining > 0f ? Mathf.CeilToInt(secondsRemaining) : 0;
        if (seconds == _shownCountdownSeconds) return;

        _shownCountdownSeconds = seconds;
        if (seconds == 0)
        {
            stackCountdownText.SetText(string.Empty);
        }
        else
        {
            stackCountdownText.SetText("{0:0}", seconds);
        }
    }
}
