using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DashCooldownUI : MonoBehaviour
{
    [SerializeField] private Image cooldownOverlay;
    [SerializeField] private DashAbility dashAbility;
    
    private float _dashCooldown;
    private float _remainingCooldown;
    
    private void OnEnable()
    {
        dashAbility.OnDashEnded += StartCooldown;
    }

    private void OnDisable()
    {
        dashAbility.OnDashEnded -= StartCooldown;
    }

    private void Update()
    {
        if (_remainingCooldown > 0f)
        {
            // Count down first, so the last frame sets the overlay to empty instead of a sliver
            _remainingCooldown -= Time.deltaTime;
            cooldownOverlay.fillAmount = Mathf.Max(0f, _remainingCooldown) / _dashCooldown;
        }
        
    }

    private void StartCooldown(float cooldown)
    {
        _dashCooldown = cooldown;
        _remainingCooldown = _dashCooldown;
    }
}
