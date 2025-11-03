using UnityEngine;
using UnityEngine.UI;

public class AbilityCooldownUI : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    public Image Overlay;

    private float cooldown;


    private void OnEnable()
    {
        Overlay.fillAmount = 1;
        float abilityCooldown = playerStats.playerAbilityCooldown;
        float playerCooldownReduction = playerStats.playerCooldown;
        cooldown = abilityCooldown - abilityCooldown * (playerCooldownReduction / 100);
    }
    private void Update()
    {
        Overlay.fillAmount -= (1/cooldown) * Time.deltaTime;
        if (Overlay.fillAmount == 0)
        {        
            this.gameObject.SetActive(false);
        }
    }
}
