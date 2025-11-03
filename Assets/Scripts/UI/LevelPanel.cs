using System;
using TMPro;
using UnityEngine;

public class LevelPanel : MonoBehaviour
{
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private PlayerStats playerStats;

    public TextMeshProUGUI primaryStatsText;
    public TextMeshProUGUI secondaryStatsText;

    private void OnEnable()
    {
        SetTextToPrimayStats();
        SetTextToSecondaryStats();
    }

    private void SetTextToPrimayStats()
    {
        string display = "";

        display += $"Max. HP: \t\t{playerStats.playerMaxHP}\n"; // :
        display += $"HP Regeneration: \t{playerStats.playerHPRegeneration}\n";
        display += $"Lifesteal: \t\t{playerStats.playerLifeSteal}\n";
        display += $"Damage: \t\t{playerStats.playerDamage}\n";
        display += $"Meele Damage: \t{playerStats.playerMeeleDamage}\n";
        display += $"Ranged Damage: \t{playerStats.playerRangedDamage}\n";
        display += $"Mystic Damage: \t{playerStats.playerMysticDamage}\n";
        display += $"Attackspeed: \t{playerStats.playerAttackSpeed}\n";
        display += $"Crit Chance: \t{playerStats.playerCritChance}\n";
        display += $"Armor: \t\t{playerStats.playerArmor}\n";
        display += $"Dodge: \t\t{playerStats.playerDodge}\n";
        display += $"Movementspeed: \t{playerStats.playerMovespeed}\n";
        display += $"Luck: \t\t\t{playerStats.playerLuck}\n";
        display += $"Cooldown: \t\t{playerStats.playerCooldown}\n";

        primaryStatsText.text = display;
    }

    private void SetTextToSecondaryStats()
    {
        string display = "";

        display += $"Knockback: \t\t{playerStats.playerKnockback}\n";
        display += $"Pickup Range: \t{playerStats.playerLightAbsorption}\n";

        secondaryStatsText.text = display;
    }


    public void SelectLevelUp()
    {
        shopPanel.SetActive(true);
        this.gameObject.SetActive(false);
    }    
}
