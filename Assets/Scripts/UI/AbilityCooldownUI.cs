using UnityEngine;
using UnityEngine.UI;

public class AbilityCooldownUI : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private GameObject Character;
    private CharacterStats characterStats;
    public Image Overlay;

    private void Awake()
    {
        characterStats = Character.GetComponentInChildren<CharacterStats>();
    }

    private void OnEnable()
    {
        Overlay.fillAmount = 1f;
    }
    private void Update()
    {
        if (characterStats.actualMaxCooldown > 0.001f)
        {
            Overlay.fillAmount = characterStats.remainingCooldown / characterStats.actualMaxCooldown;
        }
        
        if (characterStats.abilityReady)
        {
            Overlay.fillAmount = 0f;
            this.gameObject.SetActive(false);
        }
    }
}
