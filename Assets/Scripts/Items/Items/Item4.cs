using UnityEngine;

public class Item4 : MonoBehaviour
{
    // +5% Cooldown Reduction; +5% Attackspeed; -1 Armor;
    [SerializeField] private float cooldownReduction;
    [SerializeField] private float attackSpeedGain;
    [SerializeField] private float armorLoss;
    private PlayerStats playerStats;
    
    
    private void Start()
    {
        playerStats = transform.root.GetComponent<PlayerStats>();
        ApplyItem();
    }

    private void ApplyItem()
    {
        playerStats.playerCooldown += cooldownReduction;
        playerStats.playerAttackSpeed += attackSpeedGain;
        playerStats.playerArmor -= armorLoss;
    }
}
