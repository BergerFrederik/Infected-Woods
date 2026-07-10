using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class StatShardPanel : MonoBehaviour
{
    [SerializeField] private StatConfigurator statConfigurator;
    [SerializeField] private float budStatMultiplier = 1.5f;
    [SerializeField] private GameObject infoStatShardPrefab;
    [SerializeField] private Transform playerStatShardContainer;

    [Header("Blossom Stats")]
    [SerializeField] private GameObject[] blossomShards;
    private List<GameObject> _rndChosenBlossomShards;

    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private RandomRollEvent randomRollEvent;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI[] statUpgradeTitles;
    [SerializeField] private TextMeshProUGUI[] statUpgradeContents;
    [SerializeField] private Image[] statShardIcons; // TODO: für spätere Implementierung
    [SerializeField] private Button[] statShardButtons;
    [SerializeField] private Image[] statShardBackgroundImages;

    [Header("Odds")]
    [SerializeField] private float budChanceIncrease = 1f;
    [SerializeField] private float blossomChanceIncrease = 0.5f;
    [SerializeField] private float budIncreaseAt;
    [SerializeField] private float blossomIncreaseAt;
    [SerializeField] private float budIncreaseCap;
    private const float BaseChanceForRoot = 100f;

    private enum Rarities
    {
        Root,
        Bud,
        Blossom
    }

    private Rarities _randomRarity;
    private Dictionary<string, float> _randomStatMap;

    private void OnEnable()
    {
        StartFunction();
        DetermineRarities();
        SetUIToButtons();
    }

    private void OnDisable()
    {
        foreach (Button button in statShardButtons)
        {
            button.onClick.RemoveAllListeners();
        }
    }

    private void StartFunction()
    {
        _randomStatMap = new Dictionary<string, float>();
        _rndChosenBlossomShards = new List<GameObject>();
    }

    private void DetermineRarities()
    {
        float lvl = playerStats.playerLevel;

        float rawBlossom = ComputeChances(lvl, blossomChanceIncrease, blossomIncreaseAt, Mathf.Infinity, 0f);
        float rawBud     = ComputeChances(lvl, budChanceIncrease, budIncreaseAt, budIncreaseCap, 0f);

        float currentBud  = Mathf.Max(0, rawBud - rawBlossom);
        float currentRoot = Mathf.Max(0, BaseChanceForRoot - currentBud - rawBlossom);

        Debug.Log($"root: {currentRoot}");
        Debug.Log($"bud: {currentBud}");
        Debug.Log($"bl: {rawBlossom}");

        float thresholdBud = currentRoot + currentBud;

        float roll = randomRollEvent.GetRandomFloatRoll(0f, 100f);
        Debug.Log($"roll: {roll}");

        if (roll <= currentRoot)
        {
            _randomRarity = Rarities.Root;
            GetRootStats();
        }
        else if (roll <= thresholdBud)
        {
            _randomRarity = Rarities.Bud;
            GetBudStats();
        }
        else
        {
            _randomRarity = Rarities.Blossom;
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
        var availableStats = new List<StatConfigurator.Stat>(statConfigurator.allStats);

        for (int i = 0; i < 3; i++)
        {
            int randomIndex = Random.Range(0, availableStats.Count);
            _randomStatMap.Add(availableStats[randomIndex].GetStatName(), Mathf.Round(availableStats[randomIndex].GetRandomValue() * 10f) / 10f);
            availableStats.RemoveAt(randomIndex);
        }
    }

    private void GetBudStats()
    {
        var availableStats = new List<StatConfigurator.Stat>(statConfigurator.allStats);

        for (int i = 0; i < 3; i++)
        {
            int randomIndex = Random.Range(0, availableStats.Count);
            _randomStatMap.Add(availableStats[randomIndex].GetStatName(), availableStats[randomIndex].GetMaxValue());
            availableStats.RemoveAt(randomIndex);
        }

        List<string> statNames = new List<string>(_randomStatMap.Keys);

        foreach (string name in statNames)
        {
            float multiplied = _randomStatMap[name] * budStatMultiplier;
            _randomStatMap[name] = Mathf.CeilToInt(multiplied / 5f) * 5;
        }
    }

    private void GetBlossomStats()
    {
        List<GameObject> blossoms = new List<GameObject>(blossomShards);

        for (int i = 0; i < statShardButtons.Length; i++)
        {
            int rndIndex = Random.Range(0, blossoms.Count);
            _rndChosenBlossomShards.Add(blossoms[rndIndex]);
            blossoms.RemoveAt(rndIndex);
        }
    }

    private void SetUIToButtons()
    {
        bool isBlossom = _randomRarity == Rarities.Blossom;

        for (int i = 0; i < statShardButtons.Length; i++)
        {
            if (isBlossom)
                SetBlossomUI(i);
            else
                SetNonBlossomUI(i);
        }
    }

    private void SetBlossomUI(int i)
    {
        BlossomStatShard info = _rndChosenBlossomShards[i].GetComponent<BlossomStatShard>();
        SetButtonUI(i, info.StatName, info.ShardDescription);
    }

    private void SetNonBlossomUI(int i)
    {
        string title = _randomStatMap.Keys.ElementAt(i);
        string content = _randomStatMap[title].ToString();
        SetButtonUI(i, title, content);
    }

    private void SetButtonUI(int i, string title, string content)
    {
        statUpgradeTitles[i].text = title;
        statUpgradeContents[i].text = content;
        statShardBackgroundImages[i].sprite = statShardBackgroundImages[(int)_randomRarity].sprite;

        statShardButtons[i].onClick.RemoveAllListeners();
        int index = i;
        statShardButtons[i].onClick.AddListener(() => SelectStatShard(index));
    }

    public void SelectStatShard(int buttonIndex)
    {
        if (_randomRarity != Rarities.Blossom)
        {
            string statMapKey = _randomStatMap.Keys.ElementAt(buttonIndex);
            playerStats.ApplyStatsToPlayer(_randomStatMap[statMapKey], statMapKey);

            GameObject statShard = Instantiate(infoStatShardPrefab, playerStatShardContainer);
            InfoStatShard info = statShard.GetComponent<InfoStatShard>();
            info.StatName = statMapKey;
            info.StatValue = _randomStatMap[statMapKey];
        }
        else
        {
            Instantiate(blossomShards[buttonIndex], playerStatShardContainer);
        }

        gameObject.SetActive(false);
    }
}