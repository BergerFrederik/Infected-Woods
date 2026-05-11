using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class ItemInformation : MonoBehaviour
{
    [Header("Display Data")]
    public string itemName;
    public Sprite itemIcon;
    public float itemPrice;
    public string itemID;

    [Header("Stats Configuration")]
    public List<ItemStat> stats = new List<ItemStat>();

    [Header("Passive Ability")]
    [TextArea(3, 5)]
    public string passiveDescription;
    

    private void OnEnable()
    {
        if (IsPlayerRoot())
        {
            ApplyStats(1f);
        }
    }

    private void OnDisable()
    {
        if (IsPlayerRoot())
        {
            ApplyStats(-1f);
        }
    }

    public bool IsPlayerRoot()
    {
        bool isPlayerRoot = false;
        if (this.transform.root == null) isPlayerRoot = false;
        
        string rootName = this.transform.root.name;
        if (rootName == "Player") isPlayerRoot = true;

        return isPlayerRoot;
    }
    
    public string GetStatsAsText()
    {
        if (stats == null || stats.Count == 0) return "";

        string formattedStats = "";

        foreach (var stat in stats)
        {
            string readableName = Regex.Replace(stat.statType.ToString(), "([a-z])([A-Z])", "$1 $2");
            
            formattedStats += $"{readableName}: +{stat.value}\n";
        }

        return formattedStats;
    }

    public void ApplyStats(float multiplier)
    {
        if (stats == null || stats.Count == 0) return;

        PlayerStats player = FindAnyObjectByType<PlayerStats>();
        if (player == null) return;

        foreach (var stat in stats)
        {
            float amount = stat.value * multiplier;

            switch (stat.statType)
            {
                // Primary Stats (Properties mit Events)
                case StatType.MaxHP:
                    player.playerMaxHP += amount;
                    break;
                case StatType.MaxMP:
                    player.playerMaxMP += amount;
                    break;
                case StatType.CritChance:
                    player.PlayerCritChance += amount;
                    // Falls du ein Event für CritChance hast (im Code Action<float>):
                    // player.OnCritChanceChanged?.Invoke(player.playerCritChance);
                    break;
                case StatType.CritDamage:
                    player.PlayerCritDamage += amount;
                    break;

                // Kampf Stats (Einfache Floats)
                case StatType.HPRegeneration:
                    player.playerHPRegeneration += amount;
                    break;
                case StatType.MPRegeneration:
                    player.playerMPRegeneration += amount;
                    break;
                case StatType.LifeSteal:
                    player.playerLifeSteal += amount;
                    break;
                case StatType.Damage:
                    player.playerDamage += amount;
                    break;
                case StatType.MeleeDamage:
                    player.playerMeleeDamage += amount;
                    break;
                case StatType.RangedDamage:
                    player.playerRangedDamage += amount;
                    break;
                case StatType.MysticDamage:
                    player.playerMysticDamage += amount;
                    break;
                case StatType.AttackSpeed:
                    player.playerAttackSpeed += amount;
                    break;
                case StatType.AttackRange:
                    player.playerAttackRange += amount;
                    break;
                case StatType.Armor:
                    player.playerArmor += amount;
                    break;
                case StatType.Dodge:
                    player.playerDodge += amount;
                    break;
                case StatType.Movespeed:
                    player.playerMovespeed += amount; // Das löst CommunicateMovementspeedChanged aus
                    break;
                case StatType.Luck:
                    player.playerLuck += amount;
                    break;
                case StatType.Cooldown:
                    player.playerCooldown += amount;
                    break;

                // Secondary Stats
                case StatType.Knockback:
                    player.playerKnockback += amount;
                    break;
                case StatType.LightPickupRange:
                    player.playerLightPickupRange += amount;
                    break;
                case StatType.DashCooldownReduction:
                    player.playerDashCooldownReduction += amount;
                    break;
                case StatType.AbilityCooldown:
                    player.playerAbilityCooldown += amount;
                    break;
                case StatType.HealPower:
                    player.playerHealPower += amount;
                    break;
                case StatType.ShieldPower:
                    player.playerShieldPower += amount;
                    break;
                case StatType.WeaponSlots:
                    player.PlayerWeaponSlots += amount;
                    break;

                default:
                    Debug.LogWarning($"Stat {stat.statType} ist noch nicht im switch implementiert!");
                    break;
            }
        }
    }
    
}
