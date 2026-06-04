using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatShardPanel : MonoBehaviour
{
    [Header("Root Stats")]
    [SerializeField] private string[] rootStats;
    [SerializeField] private float[] rootMinValues;
    [SerializeField] private float[] rootMaxValues;
    
    [Header("Bud Stats")]
    [SerializeField] private string[] budStats;
    [SerializeField] private float[] budValues;
    
    [Header("Blossom Stats")]
    [SerializeField] private GameObject[] blossomShards;
    
    
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private RandomRollEvent randomRollEvent;

    [SerializeField] private Sprite[] statShardBoarders;
    [SerializeField] private Button[] statShardButtons;
    [SerializeField] private Button RerollButton;
    [SerializeField] private string[] upgradeableStats;
    [SerializeField] private float[] statUpgradeValues;
    [SerializeField] private GameManager gameManager;
    
    [Header("StatUpgrades")]
    [SerializeField] private TextMeshProUGUI levelsGainedText;
    [SerializeField] private TextMeshProUGUI[] statUpgradeTitles;
    [SerializeField] private TextMeshProUGUI[] statUpgradeContents;
    
    [Header("MoneyAmount")]
    [SerializeField] private TextMeshProUGUI moneyAmountText;

    [Header("Odds")]
    [SerializeField] private float budChanceIncrease = 1f;
    [SerializeField] private float blossomChanceIncrease = 0.5f;
    [SerializeField] private float budIncreaseAt;
    [SerializeField] private float blossomIncreaseAt;
    [SerializeField] private float budIncreaseCap;
    private float _baseChanceForRoot = 100f;
    
    private const int rarity_code_blossom = 3;
    private const int rarity_code_bud = 2;
    private const int rarity_code_root = 1;
    private Dictionary<string, float> _rootStatGainMap;
    private Dictionary<string, float> _budStatGainMap;

    private int _randomRarity;
    private string[] _randomStats;
    
    [Header("Stats")]
    [SerializeField] private TextMeshProUGUI primaryStatsText;
    [SerializeField] private TextMeshProUGUI primaryStatsValues;
    [SerializeField] private TextMeshProUGUI secondaryStatsText;
    [SerializeField] private TextMeshProUGUI secondaryStatsValues;
    
    [Header("Reroll")]
    [SerializeField] private RerollMechanic rerollMechanic;
    [SerializeField] private TextMeshProUGUI rerollCostTextLeft;
    [SerializeField] private TextMeshProUGUI rerollCostTextRight;

    private int _numRolls;
    private string[] _chosenRandomStats;

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
        
    }

    private void StartFunction()
    {
        _budStatGainMap = new Dictionary<string, float>();
        for (int i = 0; i < budStats.Length; i++)
        {
            string statName = budStats[i];
            float statValue = budValues[i];
            
            _budStatGainMap.TryAdd(statName, statValue);
        }
        _numRolls = statShardBoarders.Length;
        
    }

    private void SetLogic()
    {
        DetermineRarities();
    }

    private void SetUI()
    {
        SetUIToMoneyAmount();
        SetUIToButtons();
        SetTextToLevelsGainedUI();
        SetRerollCostText();
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
        float lvl = playerStats.playerLevel;
        
        float rawBlossom = ComputeChances(lvl, blossomChanceIncrease, blossomIncreaseAt, Mathf.Infinity, 0f);
        float rawBud     = ComputeChances(lvl, budChanceIncrease, budIncreaseAt, budIncreaseCap, 0f);
        
        float currentBlossom = rawBlossom;
        float currentBud     = Mathf.Max(0, rawBud - currentBlossom);
        float currentRoot = Mathf.Max(0, _baseChanceForRoot - currentBud - currentBlossom);
        
        Debug.Log($"root: {currentRoot}");
        Debug.Log($"bud: {currentBud}");
        Debug.Log($"bl: {currentBlossom}");
        
        float thresholdBud = currentRoot + currentBud;
        
        float roll = randomRollEvent.GetRandomFloatRoll(0f, 100f);
        Debug.Log($"roll: {roll}");
        if (roll <= currentRoot)
        {
            _randomRarity = rarity_code_root;
            GetRootStats();
        }
        else if (roll <= thresholdBud)
        {
            _randomRarity = rarity_code_bud;
            GetBudStats();
        }
        else
        {
            _randomRarity = rarity_code_blossom;
            GetBlossomStats();
        }
    }

    private float ComputeChances(float playerLvl, float increase, float minLvl, float maxLvl, float baseChance)
    {
        if (playerLvl < minLvl)
        {
            return baseChance;
        }
        
        float cappedLvl = Mathf.Min(playerLvl, maxLvl);
        float levelDiff = cappedLvl - (minLvl - 1);
        
        return increase * levelDiff + baseChance;
    }

    private void GetRootStats()
    {
        
    }

    private void GetBudStats()
    {
        Dictionary<string, float> statMapTemp = _budStatGainMap;
        for (int i = 0; i < _numRolls; i++)
        {
            
        }
        
    }

    private void GetBlossomStats()
    {
        
    }
    

    private void SetUIToMoneyAmount()
    {
        moneyAmountText.text = playerStats.PlayerLightAmount.ToString();
    }

    private void SetUIToButtons()
    {
        for (int i = 0; i <= statShardButtons.Length - 1; i++)
        {
            Sprite nextSprite = statShardBoarders[_randomRarity[i] - 1];
            string nextStatUpgradeTitle = _randomStats[i];
            
            statShardButtons[i].GetComponent<Image>().sprite = nextSprite;
            statUpgradeTitles[i].text = nextStatUpgradeTitle;
            float statUpgradeValue = _rootStatGainMap[nextStatUpgradeTitle] * _randomRarity[i];
            statUpgradeContents[i].text = statUpgradeValue.ToString();
            
            statShardButtons[i].onClick.RemoveAllListeners();
            int index = i;
            statShardButtons[i].onClick.AddListener(() => SelectLevelUp(index));           
        }
    }

    private void SetRerollCostText()
    {
        int rerollCost = rerollMechanic.GetRerollPrice();
        
        rerollCostTextLeft.text = $"Reroll - {rerollCost}";
        rerollCostTextRight.text = $"Reroll - {rerollCost}";
        
        if (playerStats.PlayerLightAmount < rerollCost)
        {
            rerollCostTextLeft.color = Color.red;
            rerollCostTextRight.color = Color.red;
        }
    }

    private void RerollStats()
    {
        if (playerStats.PlayerLightAmount >= rerollMechanic.GetRerollPrice())
        {
            playerStats.PlayerLightAmount -= rerollMechanic.GetRerollPrice();
            rerollMechanic.NumRerolls++;
            SetLogic();
            SetUI();
        }
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
        float chosenStat = _rootStatGainMap[_randomStats[buttonIndex]];
        float multiplier = _randomRarity[buttonIndex];
        float statToApply = chosenStat * multiplier;
        ApplyStatsToPlayer(statToApply, _randomStats[buttonIndex]);
        this.gameObject.SetActive(false);
        gameManager.CycleShops();
    }
    
    
    private void ApplyStatsToPlayer(float value, string statName)
    {
        playerStats.ApplyStatsToPlayer(value, statName);
    }



}
