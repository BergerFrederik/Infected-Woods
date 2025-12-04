using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class LevelPanel : MonoBehaviour
{
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private PlayerStats playerStats;

    [SerializeField] private Sprite[] LVLUpBoarders;
    [SerializeField] private Button[] LVLUpButtons;
    [SerializeField] private Button RerollButton;
    [SerializeField] private string[] UpgradeableStats;
    [SerializeField] private float[] StatUpgradeValues;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private TextMeshProUGUI levelsGainedText;
    [SerializeField] private GameObject[] StatUpgradeTitles;
    [SerializeField] private GameObject[] StatUpgradeContents;

    [Range(0f, 100f)][SerializeField] private float chance_for_blossom;
    [Range(0f, 100f)][SerializeField] private float chance_for_bud;
    [Range(0f, 100f)][SerializeField] private float chance_for_shroom;

    private const int rarity_code_blossom = 4;
    private const int rarity_code_bud = 3;
    private const int rarity_code_shroom = 2;
    private const int rarity_code_root = 1;
    private Dictionary<string, float> StatGainMap;

    private int[] randomRaritys;
    private string[] randomStats;
    
    public TextMeshProUGUI primaryStatsText;
    public TextMeshProUGUI secondaryStatsText;

    private void OnEnable()
    {
        RerollButton.onClick.AddListener(RerollStats);
        StartFunction();
        SetTextToPrimayStats();
        SetTextToSecondaryStats();
        SetLogic();
        SetUI();
    }

    private void StartFunction()
    {
        StatGainMap = new Dictionary<string, float>();
        for (int i = 0; i < UpgradeableStats.Length; i++)
        {
            string statName = UpgradeableStats[i];
            float statValue = StatUpgradeValues[i];

            if (!StatGainMap.ContainsKey(statName))
            {
                StatGainMap.Add(statName, statValue);
            }
            else
            {
                Debug.LogWarning($"StatMapper Warnung: Der Key '{statName}' ist doppelt vorhanden und wurde ignoriert.");
            }
        }
    }

    private void SetLogic()
    {
        DetermineRarities();
        DetermineStats();
    }

    private void SetUI()
    {
        SetSpritesToButtons();
        SetTextToLevelsGainedUI();
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

    private void DetermineRarities()
    {
        int numRarities = LVLUpBoarders.Length;
        randomRaritys = new int[numRarities];
        for (int i = 0; i < numRarities; i++)
        {
            int rarity = Random.Range(0, 100);
            if (rarity <= chance_for_blossom)
            {
                randomRaritys[i] = rarity_code_blossom;
            }
            else if (rarity > chance_for_blossom && rarity <= chance_for_bud + chance_for_blossom)
            {
                randomRaritys[i] = rarity_code_bud;
            }
            else if (rarity > chance_for_blossom + chance_for_bud && rarity <= chance_for_shroom + chance_for_bud + chance_for_blossom)
            {
                randomRaritys[i] = rarity_code_shroom;
            }
            else
            {
                randomRaritys[i] = rarity_code_root;
            }
        }
    }

    private void DetermineStats()
    {
        List<string> availableStats = new List<string>(UpgradeableStats); 
    
        int numStatsToPick = UpgradeableStats.Length;
        randomStats = new string[numStatsToPick];
    
        for (int i = 0; i < numStatsToPick; i++)
        {
            int randomIndex = Random.Range(0, availableStats.Count); 
        
            string selectedStat = availableStats[randomIndex];
        
            randomStats[i] = selectedStat;
        
            availableStats.RemoveAt(randomIndex); 
        }
    }

    private void SetSpritesToButtons()
    {
        for (int i = 0; i <= LVLUpButtons.Length - 1; i++)
        {
            Sprite nextSprite = LVLUpBoarders[randomRaritys[i] - 1];
            string nextStatUpgradeTitle = randomStats[i];
            LVLUpButtons[i].GetComponent<Image>().sprite = nextSprite;
            StatUpgradeTitles[i].GetComponent<TextMeshProUGUI>().text = nextStatUpgradeTitle;
            LVLUpButtons[i].onClick.RemoveAllListeners();
            int index = i;
            LVLUpButtons[i].onClick.AddListener(() => SelectLevelUp(index));           
        }
    }

    private void RerollStats()
    {
        SetLogic();
        SetUI();
    }

    private void SetTextToLevelsGainedUI()
    {
        float levelsGained = playerStats.playerLevelsGained + 1;
        string text = "[" + levelsGained.ToString() + "]";
        levelsGainedText.text = text;
    }
    public void SelectLevelUp(int buttonIndex)
    {
        float chosenStat = StatGainMap[randomStats[buttonIndex]];
        float multiplier = randomRaritys[buttonIndex];
        float statToApply = chosenStat * multiplier;
        ApplyStatsToPlayer(statToApply, randomStats[buttonIndex]);
        this.gameObject.SetActive(false);
        gameManager.CycleShops();
    }
    private void ApplyStatsToPlayer(float value, string statName)
    {
        switch (statName)
        {
            case "Max HP":
                playerStats.playerMaxHP += value;
                break;
            case "Regeneration":
                playerStats.playerHPRegeneration += value;
                break;
            case "Lifesteal":
                playerStats.playerLifeSteal += value;
                break;
            case "Damage":
                playerStats.playerDamage += value;
                break;
            case "Meele Damage":
                playerStats.playerMeeleDamage += value;
                break;
            case "Ranged Damage":
                playerStats.playerRangedDamage += value;
                break;
            case "Mystic Damage":
                playerStats.playerMysticDamage += value;
                break;
            case "Attackspeed":
                playerStats.playerAttackSpeed += value;
                break;
            case "Crit":
                playerStats.playerCritChance += value;
                break;
            case "Range":
                playerStats.playerAttackRange += value;
                break;
            case "Armor":
                playerStats.playerArmor += value;
                break;
            case "Dodge":
                playerStats.playerDodge += value;
                break;
            case "Movespeed":
                playerStats.playerMovespeed += value;
                break;
            case "Luck":
                playerStats.playerLuck += value;
                break;
            case "Cooldown":
                playerStats.playerCooldown += value;
                break;
        }
    }
}
