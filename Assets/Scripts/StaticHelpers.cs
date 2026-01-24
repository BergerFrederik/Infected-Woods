using TMPro;
using UnityEngine;

public class StaticHelpers : MonoBehaviour
{
    public static void SetPrimaryStatsToTextUI(TextMeshProUGUI textField, TextMeshProUGUI valuesField, PlayerStats playerStats)
    {
        string stats = "";
        string values = "";

        stats += $"HP:\n";
        stats += $"HP Reg:\n";
        stats += $"Mana:\n";
        stats += $"Mana Reg:\n";
        stats += $"LS:\n";
        stats += $"Damage:\n";
        stats += $"Meele Dmg:\n";
        stats += $"Ranged Dmg:\n";
        stats += $"Mystic Dmg:\n";
        stats += $"AS:\n";
        stats += $"Crit:\n";
        stats += $"Armor:\n";
        stats += $"Dodge:\n";
        stats += $"MS:\n";
        stats += $"Luck:\n";
        stats += $"CD:\n";
        
        values += $"{playerStats.playerMaxHP}\n";
        values += $"{playerStats.playerHPRegeneration}\n";
        values += $"{playerStats.playerMaxMP}\n";
        values += $"{playerStats.playerMPRegeneration}\n";
        values += $"{playerStats.playerLifeSteal}\n";
        values += $"{playerStats.playerDamage}\n";
        values += $"{playerStats.playerMeeleDamage}\n";
        values += $"{playerStats.playerRangedDamage}\n";
        values += $"{playerStats.playerMysticDamage}\n";
        values += $"{playerStats.playerAttackSpeed}\n";
        values += $"{playerStats.playerCritChance}\n";
        values += $"{playerStats.playerArmor}\n";
        values += $"{playerStats.playerDodge}\n";
        values += $"{playerStats.playerMovespeed}\n";
        values += $"{playerStats.playerLuck}\n";
        values += $"{playerStats.playerCooldown}\n";

        textField.text = stats;
        valuesField.text = values;
    }

    public static void SetSecondaryStatsToTextUI(TextMeshProUGUI textField, TextMeshProUGUI valuesField, PlayerStats playerStats)
    {
        string stats = "";
        string values = "";

        stats += $"Knockback:\n";
        stats += $"Pickup Range:\n";
        stats += $"Dash CD:\n";
        stats += $"Ability CD:\n";
        stats += $"Heal Power:\n";
        stats += $"Shield Power:\n";
        
        values += $"{playerStats.playerKnockback}\n";
        values += $"{playerStats.playerLightPickupRange}\n";
        values += $"{playerStats.playerDashCooldownReduction}\n";
        values += $"{playerStats.playerAbilityCooldown}\n";
        values += $"{playerStats.playerHealPower}\n";
        values += $"{playerStats.playerShieldPower}\n";
        
        textField.text = stats;
        valuesField.text = values;
    }
}
