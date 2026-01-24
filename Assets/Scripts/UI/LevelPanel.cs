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
    [SerializeField] private TextMeshProUGUI[] StatUpgradeTitles;
    [SerializeField] private TextMeshProUGUI[] StatUpgradeContents;

    [Range(0f, 100f)][SerializeField] private float chance_for_root;
    [Range(0f, 100f)][SerializeField] private float chance_for_shroom;
    [Range(0f, 100f)][SerializeField] private float chance_for_bud;

    private const int rarity_code_blossom = 4;
    private const int rarity_code_bud = 3;
    private const int rarity_code_shroom = 2;
    private const int rarity_code_root = 1;
    private Dictionary<string, float> StatGainMap;

    private int[] randomRaritys;
    private string[] randomStats;
    
    [SerializeField] private TextMeshProUGUI primaryStatsText;
    [SerializeField] private TextMeshProUGUI primaryStatsValues;
    [SerializeField] private TextMeshProUGUI secondaryStatsText;
    [SerializeField] private TextMeshProUGUI secondaryStatsValues;

    private void OnEnable()
    {
        RerollButton.onClick.AddListener(RerollStats);
        StartFunction();
        SetTextToPrimayStats(primaryStatsText);
        SetTextToSecondaryStats(secondaryStatsText);
        SetLogic();
        SetUI();
    }

    private void OnDisable()
    {
        RerollButton.onClick.RemoveListener(RerollStats);
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
        SetUIToButtons();
        SetTextToLevelsGainedUI();
    }
    public void SetTextToPrimayStats(TextMeshProUGUI primaryStatsText)
    {
        StaticHelpers.SetPrimaryStatsToTextUI(primaryStatsText, primaryStatsValues, playerStats);
    }

    private void SetTextToSecondaryStats(TextMeshProUGUI secondaryStatsText)
    {
        StaticHelpers.SetSecondaryStatsToTextUI(secondaryStatsText, secondaryStatsValues, playerStats);
    }

    private void DetermineRarities()
    {
        int numRarities = LVLUpBoarders.Length;
        randomRaritys = new int[numRarities];
        for (int i = 0; i < numRarities; i++)
        {
            int rarity = Random.Range(0, 100);
            rarity += (int)playerStats.playerLevel;
            if (rarity < chance_for_root)
            {
                randomRaritys[i] = rarity_code_root;
            }
            else if (rarity >= chance_for_root && rarity < chance_for_root + chance_for_shroom)
            {
                randomRaritys[i] = rarity_code_shroom;
            }
            else if (rarity >= chance_for_root + chance_for_shroom && rarity < chance_for_root + chance_for_shroom + chance_for_bud)
            {
                randomRaritys[i] = rarity_code_bud;
            }
            else
            {
                randomRaritys[i] = rarity_code_blossom;
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

    private void SetUIToButtons()
    {
        for (int i = 0; i <= LVLUpButtons.Length - 1; i++)
        {
            Sprite nextSprite = LVLUpBoarders[randomRaritys[i] - 1];
            string nextStatUpgradeTitle = randomStats[i];
            
            LVLUpButtons[i].GetComponent<Image>().sprite = nextSprite;
            StatUpgradeTitles[i].text = nextStatUpgradeTitle;
            float statUpgradeValue = StatGainMap[nextStatUpgradeTitle] * randomRaritys[i];
            StatUpgradeContents[i].text = statUpgradeValue.ToString();
            
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
        // +1 because it should show 1 when you select the last level up
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
            case "HP Regeneration":
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
            case "Max MP":
                playerStats.playerMaxMP += value;
                break;
            case "MP Regeneration":
                playerStats.playerMPRegeneration += value;
                break;
        }
    }
}
