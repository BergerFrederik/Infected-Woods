using TMPro;
using UnityEngine;

public class PlayerStatsUI : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private TextMeshProUGUI playerStatsUIText;

    private void Update()
    {
        SetPlayerStatsToUI();
    }

    private void SetPlayerStatsToUI()
    {
        string display = "";

        display += $"HP: \t\t\t{playerStats.playerMaxHP}\n";
        display += $"HP Reg: \t\t{playerStats.playerHPRegeneration}\n";
        display += $"Mana: \t\t{playerStats.playerMaxMP}\n";
        display += $"Mana Reg: \t{playerStats.playerMPRegeneration}\n";
        display += $"LS: \t\t\t{playerStats.playerLifeSteal}\n";
        display += $"Damage: \t\t{playerStats.playerDamage}\n";
        display += $"Meele Dmg: \t{playerStats.playerMeeleDamage}\n";
        display += $"Ranged Dmg: \t{playerStats.playerRangedDamage}\n";
        display += $"Mystic Dmg: \t{playerStats.playerMysticDamage}\n";
        display += $"AS: \t\t\t{playerStats.playerAttackSpeed}\n";
        display += $"Crit: \t\t\t{playerStats.playerCritChance}\n";
        display += $"Armor: \t\t{playerStats.playerArmor}\n";
        display += $"Dodge: \t\t{playerStats.playerDodge}\n";
        display += $"MS: \t\t\t{playerStats.playerMovespeed}\n";
        display += $"Luck: \t\t{playerStats.playerLuck}\n";
        display += $"CD: \t\t\t{playerStats.playerCooldown}\n";

        playerStatsUIText.text = display;
    }
}
