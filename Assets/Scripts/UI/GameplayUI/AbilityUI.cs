using UnityEngine;
using UnityEngine.UI;

public class AbilityUI : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Image cooldownOverlay;
    [SerializeField] private Image abilityActiveOverlay;

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
}
