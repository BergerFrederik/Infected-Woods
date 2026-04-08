using UnityEngine;
using UnityEngine.UI;

public class AbilityUI : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Image cooldownOverlay;
    [SerializeField] private Image abilityActiveOverlay;
    
    private float _abilityCooldown;
    private float _remainingCooldown;
    
    private void Update()
    {
        if (_remainingCooldown > 0f)
        {
            cooldownOverlay.fillAmount = _remainingCooldown / _abilityCooldown;
            _remainingCooldown -= Time.deltaTime;
        }
    }

    public void StartAbilityCooldownUI(float cooldown)
    {
        _abilityCooldown = cooldown;
        _remainingCooldown = _abilityCooldown;
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
        abilityActiveOverlay.fillAmount = 0f;
    }
}
