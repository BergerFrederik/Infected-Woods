using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ShopPanel : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private Button startNextWaveButton;
    [SerializeField] private TextMeshProUGUI moneyAmountText;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Button toggleButton;
    [SerializeField] private StatPanel statPanel;
    [SerializeField] private Transform playerTransform;
    
    
    [Header("Items")]
    [SerializeField] private Button[] itemButtons;
    [SerializeField] private Image[] itemImages;
    [SerializeField] private List<GameObject> itemPrefabs;
    [SerializeField] private GameObject playerItems;
    [SerializeField] private Button itemRerollButton;
    [SerializeField] private GameObject itemVisualizerPrefab;
    [SerializeField] private Transform itemScrollViewContent;
    [SerializeField] private TextMeshProUGUI[] itemTitles;
    [SerializeField] private TextMeshProUGUI[] itemButtonText;
    [SerializeField] private TextMeshProUGUI[] itemDescriptionText;
    [SerializeField] private GameObject[] itemObjects;
    
    [Header("Weapons")]
    [SerializeField] private Button[] weaponButtons;
    [SerializeField] private Image[] weaponImages;
    [SerializeField] private TextMeshProUGUI[] weaponTitles;
    [SerializeField] private TextMeshProUGUI[] weaponSubtitles;
    [SerializeField] private TextMeshProUGUI[] weaponButtonText;
    [SerializeField] private TextMeshProUGUI[] weaponDescriptionText;
    [SerializeField] private List<GameObject> weaponPrefabs;
    [SerializeField] private GameObject playerWeaponManager;
    [SerializeField] private Button weaponRerollButton;
    [SerializeField] private Image[] inventoryImages;
    [SerializeField] private GameObject[] weaponObjects;
    [SerializeField] private TextMeshProUGUI[] weaponOddsTexts;
    [SerializeField] private TextMeshProUGUI weaponShopLvLText;
    [SerializeField] private Button weaponShopLvLUpButton;
    [SerializeField] private float shroomChanceIncrease = 3f;
    [SerializeField] private float budChanceIncrease = 1f;
    [SerializeField] private float blossomChanceIncrease = 0.5f;
    [SerializeField] private float shroomIncreaseAt;
    [SerializeField] private float budIncreaseAt;
    [SerializeField] private float blossomIncreaseAt;
    [SerializeField] private float shroomIncreaseCap;
    [SerializeField] private float budIncreaseCap;
    [SerializeField] private float weaponShopMaxLvL;
    [SerializeField] private float weaponShopLvlUpCostIncrease;
    [SerializeField] private float weaponShopBaseLvLUpCost;
    
    private float _baseChanceForRoot = 100f;
    private const int rarity_code_blossom = 4;
    private const int rarity_code_bud = 3;
    private const int rarity_code_shroom = 2;
    private const int rarity_code_root = 1;
    Dictionary<int, List<GameObject>> weaponsByRarity = new Dictionary<int, List<GameObject>>();
    private int _weaponShopLvl;
    private float _currentWeaponShopLvlUpCost;

    [Header("Inventory")] 
    [SerializeField] private GameObject characterObject;
    [SerializeField] private Image characterInventoryImage;
    [SerializeField] private GameObject[] weaponShopBenchSlots;
    [SerializeField] private Color[] weaponLvLColors;
    [SerializeField] private Sprite[] weaponBoarderSprites;
    [SerializeField] private Transform[] inventoryWeaponSlots;
    
    
    
    private GameObject[] arrayOfChosenRandomItems;
    private GameObject[] arrayOfChosenRandomWeapons;
    private List<GameObject> playerWeaponAnkers = new List<GameObject>();

    public static event Action OnShopCycleEnd;
    public event Action OnWeaponBought;
    
    private void OnEnable()
    {
        startNextWaveButton.onClick.AddListener(StartNextWave);
        itemRerollButton.onClick.AddListener(RerollItems);
        weaponRerollButton.onClick.AddListener(RerollWeapons);
        toggleButton.onClick.AddListener(ToggleStatsheet);
        weaponShopLvLUpButton.onClick.AddListener(IncreaseWeaponShopLvL);
        
        SetSpritesToInventoryOnActivate();
        SetSpritesToItemshop();
        SetSpritesToWeaponShop();
        SetMoneyToUI();
    }

    private void OnDisable()
    {
        startNextWaveButton.onClick?.RemoveListener(StartNextWave);
        itemRerollButton.onClick.RemoveListener(RerollItems);
        weaponRerollButton.onClick.RemoveListener(RerollWeapons);
        toggleButton.onClick.RemoveListener(ToggleStatsheet);
        weaponShopLvLUpButton.onClick.RemoveListener(IncreaseWeaponShopLvL);
        ResetItemText();
    }

    private void SetSpritesToInventoryOnActivate()
    {
        GetWeaponAnkers();
        
        for (int i = 0; i < playerWeaponAnkers.Count; i++)
        {
            if (i < playerWeaponAnkers.Count && playerWeaponAnkers[i].transform.childCount != 0)
            {
                Transform weaponObj = playerWeaponAnkers[i].transform.GetChild(0);
                WeaponStats currentWeaponStats = weaponObj.GetComponent<WeaponStats>();
                float weaponLevel = currentWeaponStats.weaponLevel;
                float weaponTier = currentWeaponStats.weaponTier;

                Transform backgroundImage = inventoryWeaponSlots[i].Find("BackgroundImage");
                Transform boarderImage = inventoryWeaponSlots[i].Find("BoarderImage");
                Transform weaponImage = inventoryWeaponSlots[i].Find("WeaponImage");

                backgroundImage.GetComponent<Image>().color = weaponLvLColors[(int)weaponLevel];
                boarderImage.GetComponent<Image>().sprite = weaponBoarderSprites[(int)weaponTier - 1];
                weaponImage.GetComponent<Image>().sprite = weaponObj.GetComponentInChildren<SpriteRenderer>().sprite;

                weaponObj.SetParent(inventoryWeaponSlots[i].Find("WeaponPrefab"));
            }
            else
            {
                inventoryImages[i].color = Color.white;
            }
        }
        
        if (characterObject.transform.childCount > 0)
        {
            GameObject chosenCharacterObject = characterObject.transform.GetChild(0).gameObject;
            Transform visuals = chosenCharacterObject.transform.Find("CharacterVisuals");
            if (visuals != null)
                characterInventoryImage.sprite = visuals.GetComponentInChildren<SpriteRenderer>().sprite;
        }
    }

    private void SetSpritesToInventory()
    {
        for (int i = 0; i < inventoryWeaponSlots.Length; i++)
        {
            Transform backgroundImage = inventoryWeaponSlots[i].Find("BackgroundImage");
            Transform boarderImage = inventoryWeaponSlots[i].Find("BoarderImage");
            Transform weaponImage = inventoryWeaponSlots[i].Find("WeaponImage");
            Transform weaponPrefabSlot = inventoryWeaponSlots[i].Find("WeaponPrefab");
            
            if (weaponPrefabSlot.childCount != 0)
            {
                Transform weaponObj = weaponPrefabSlot.GetChild(0);
                WeaponStats currentWeaponStats = weaponObj.GetComponent<WeaponStats>();
                float weaponLevel = currentWeaponStats.weaponLevel;
                float weaponTier = currentWeaponStats.weaponTier;
                
                backgroundImage.GetComponent<Image>().color = weaponLvLColors[(int)weaponLevel];
                boarderImage.GetComponent<Image>().sprite = weaponBoarderSprites[(int)weaponTier - 1];
                weaponImage.GetComponent<Image>().sprite = weaponObj.GetComponentInChildren<SpriteRenderer>().sprite;
            }
            else
            {
                backgroundImage.GetComponent<Image>().color = Color.white;
                boarderImage.GetComponent<Image>().sprite = null;
                weaponImage.GetComponent<Image>().sprite = null;
            }
        }
    }

    private void SetSpritesToItemshop()
    {
        int num_items_in_shop = 4;
        arrayOfChosenRandomItems = new GameObject[num_items_in_shop];
        for (int i = 0; i < itemButtons.Length; i++)
        {
            // set active if not active
            if (!itemObjects[i].activeSelf)
            {
                itemObjects[i].SetActive(true);
            }
            // get random item
            int randomIndex = Random.Range(0, itemPrefabs.Count);
            GameObject randomItem = itemPrefabs[randomIndex];
            arrayOfChosenRandomItems[i] = randomItem;
            ItemInformation itemInformation = randomItem.GetComponent<ItemInformation>();

            // get Sprite from item
            Sprite sprite = randomItem.GetComponent<SpriteRenderer>().sprite;

            // set sprite to button
            itemImages[i].sprite = sprite;
            
            // set Text to Title
            itemTitles[i].text = itemInformation.itemID;

            // set Text to description
            foreach (string itemDescription in itemInformation.itemText)
            {
                itemDescriptionText[i].text += itemDescription + System.Environment.NewLine;
            }

            // set Text to Buttons
            itemButtonText[i].text = itemInformation.itemPrice.ToString();
            
            // Add Listeners to Button
            itemButtons[i].onClick.RemoveAllListeners();
            int index = i;
            itemButtons[i].onClick.AddListener(() => BuyItem(index, itemInformation));
        }
    }

    private void BuyItem(int index, ItemInformation itemInformation)
    {
        float itemPrice = itemInformation.itemPrice;
        float playerLightAmount = playerStats.playerLightAmount;
        if (playerLightAmount - itemPrice >= 0)
        {
            itemButtons[index].onClick.RemoveAllListeners();
            GameObject chosenItem = arrayOfChosenRandomItems[index];
            GameObject boughtItem = Instantiate(chosenItem);
            boughtItem.transform.SetParent(playerItems.transform, false);

            Sprite itemSprite = chosenItem.GetComponent<SpriteRenderer>().sprite;
            AddItemToVisualizer(itemSprite);
            HandlePurchase(itemPrice);
            itemObjects[index].SetActive(false);
        }
    }

    private void AddItemToVisualizer(Sprite itemSprite)
    {
        GameObject itemVisual = Instantiate(itemVisualizerPrefab);
        itemVisual.transform.SetParent(itemScrollViewContent, false);
        Image visualImage = itemVisual.GetComponent<Image>();
        if (visualImage != null)
        {
            visualImage.sprite = itemSprite;
        }
    }


    private void SetSpritesToWeaponShop()
    {
        SetWeaponDict();
        SetRarityText();
        
        const int num_weapons_in_shop = 4;
        
        arrayOfChosenRandomWeapons = new GameObject[num_weapons_in_shop];
        for (int i = 0; i < weaponImages.Length; i++)
        {
            // set active if not active
            if (!weaponObjects[i].activeSelf)
            {
                weaponObjects[i].SetActive(true);
            }
            
            // get rarity
            int rarity = CalculateRarity();
            
            // get random Weapon
            if (weaponsByRarity.ContainsKey(rarity) && weaponsByRarity[rarity].Count > 0)
            {
                List<GameObject> matchingWeapons = weaponsByRarity[rarity];
                
                int randomIndex = Random.Range(0, matchingWeapons.Count);
                GameObject chosenWeapon = matchingWeapons[randomIndex];
                
                arrayOfChosenRandomWeapons[i] = chosenWeapon;
            }
            else
            {
                Debug.LogError($"Keine Waffen für Rarity {rarity} gefunden!");
            }
            
            GameObject randomWeapon = arrayOfChosenRandomWeapons[i];
            
            WeaponStats weaponStats = randomWeapon.GetComponent<WeaponStats>();

            // get Sprite from Weapon
            Sprite sprite = randomWeapon.GetComponentInChildren<SpriteRenderer>().sprite;

            // set Sprite to Image
            weaponImages[i].sprite = sprite;
            
            // set Title- and Subtitle-text
            weaponTitles[i].text = weaponStats.weaponName;
            weaponSubtitles[i].text = weaponStats.weaponSubtitle;
            
            // set information text

            
            // set cost to button
            weaponButtonText[i].text = $"Buy - {weaponStats.weaponPrice}";

            // Add Listeners to Button
            weaponButtons[i].onClick.RemoveAllListeners();
            int index = i;
            weaponButtons[i].onClick.AddListener(() => BuyWeapon(index, weaponStats));
        }        
    }

    private void SetWeaponDict()
    {
        weaponsByRarity.Clear();
        foreach (var prefab in weaponPrefabs)
        {
            int tier = (int)prefab.GetComponent<WeaponStats>().weaponTier;
            if (!weaponsByRarity.ContainsKey(tier))
            {
                weaponsByRarity[tier] = new List<GameObject>();
            }
            weaponsByRarity[tier].Add(prefab);
        }
    }

    private void SetRarityText()
    {
        var odds = GetCurrentOdds(_weaponShopLvl);

        weaponOddsTexts[0].text = $"{odds.root}%";
        weaponOddsTexts[1].text = $"{odds.shroom}%";
        weaponOddsTexts[2].text = $"{odds.bud}%";
        weaponOddsTexts[3].text = $"{odds.blossom}%";

        if (_weaponShopLvl == 0)
        {
            _currentWeaponShopLvlUpCost = weaponShopBaseLvLUpCost;
        }
        
        weaponShopLvLUpButton.GetComponentInChildren<TextMeshProUGUI>().text = $"LVL Up - {_currentWeaponShopLvlUpCost}";
        weaponShopLvLText.text = _weaponShopLvl.ToString();
    }

    private int CalculateRarity()
    {
        var odds = GetCurrentOdds(playerStats.playerLevel);
        RandomRollEvent randomRollEvent = playerTransform.GetComponentInChildren<RandomRollEvent>();
    
        float roll = randomRollEvent.GetRandomFloatRoll(0f, 100f);

        
        if (roll <= odds.root) 
            return rarity_code_root;
    
        if (roll <= odds.root + odds.shroom) 
            return rarity_code_shroom;
    
        if (roll <= odds.root + odds.shroom + odds.bud) 
            return rarity_code_bud;

        return rarity_code_blossom;
    }

    private (float root, float shroom, float bud, float blossom) GetCurrentOdds(float lvl)
    {
        float rawBlossom = ComputeChances(lvl, blossomChanceIncrease, blossomIncreaseAt, Mathf.Infinity, 0f);
        float rawBud     = ComputeChances(lvl, budChanceIncrease, budIncreaseAt, budIncreaseCap, 0f);
        float rawShroom  = ComputeChances(lvl, shroomChanceIncrease, shroomIncreaseAt, shroomIncreaseCap, 0f);
    
        float blossom = rawBlossom;
        float bud     = Mathf.Max(0, rawBud - blossom);
        float shroom  = Mathf.Max(0, rawShroom - bud - blossom);
        float root    = Mathf.Max(0, _baseChanceForRoot - shroom - bud - blossom);

        return (root, shroom, bud, blossom);
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

    private void BuyWeapon(int index, WeaponStats weaponStats)
    {
        float weaponPrice = weaponStats.weaponPrice;
        float playerLightAmount = playerStats.playerLightAmount;
        
        if (playerLightAmount - weaponPrice >= 0)
        {
            GetWeaponAnkers();
            weaponButtons[index].onClick.RemoveAllListeners();
            int inventoryWeaponSlotIndex = GetNextEmptyWeaponSlotIndex();
            int benchIndex = GetNextEmptyBenchSlotIndex();

            Transform targetParent = null;
            
            if (inventoryWeaponSlotIndex != -1)
            {
                targetParent = inventoryWeaponSlots[inventoryWeaponSlotIndex].Find("WeaponPrefab");
            }  
            else if (benchIndex != -1)
            {
                targetParent = weaponShopBenchSlots[benchIndex].transform.Find("WeaponPrefab");
            }
            else
            {
                Debug.Log("Inventar und Bench voll");
                return;
            }
            
            GameObject chosenWeapon = arrayOfChosenRandomWeapons[index];
            GameObject boughtWeapon = Instantiate(chosenWeapon, targetParent, false);
            
            HandlePurchase(weaponPrice);
            weaponObjects[index].SetActive(false);
            
            RefreshAllUI();
            OnWeaponBought?.Invoke();
        }
    }

    private int GetNextEmptyWeaponSlotIndex()
    {
        int index = - 1;
        for (int i = 0; i < inventoryWeaponSlots.Length; i++)
        {
            if (inventoryWeaponSlots[i].Find("WeaponPrefab").childCount == 0)
            {
                index = i;
                break;
            }
        }
        return index;
    }
    
    private int GetNextEmptyBenchSlotIndex()
    {
        for (int i = 0; i < weaponShopBenchSlots.Length; i++)
        {
            Transform weaponPrefabSlot = weaponShopBenchSlots[i].transform.Find("WeaponPrefab");
            
            if (weaponPrefabSlot != null && weaponPrefabSlot.childCount == 0)
            {
                return i;
            }
        }
        return -1; 
    }

    private void HandlePurchase(float purchasePrice)
    {
        playerStats.playerLightAmount -= purchasePrice;
        SetMoneyToUI();
    }

    private void GetWeaponAnkers()
    {
        playerWeaponAnkers.Clear();
        for (int i = 0; i < playerWeaponManager.transform.childCount; i++)
        {
            playerWeaponAnkers.Add(playerWeaponManager.transform.GetChild(i).gameObject);
        }
    }

    private void SetMoneyToUI()
    {
        moneyAmountText.text = playerStats.playerLightAmount.ToString(); 
    }

    private void RerollItems()
    {
        ResetItemText();
        SetSpritesToItemshop();
    }

    private void ResetItemText()
    {
        for (int i = 0; i < itemDescriptionText.Length; i++)
        {
            if (itemDescriptionText[i] == null)
            {
                break;
            }
            itemDescriptionText[i].text = "";
        }
    }

    private void IncreaseWeaponShopLvL()
    {
        if (_weaponShopLvl == (int)weaponShopMaxLvL)
        {
            Debug.Log("Max LVL");
            return;
        }
        
        if (playerStats.playerLightAmount < _currentWeaponShopLvlUpCost)
        {
            Debug.Log("Insufficient funds");
            return;
        }

        HandlePurchase(_currentWeaponShopLvlUpCost);
        _weaponShopLvl++;
        
        weaponShopLvLText.text = _weaponShopLvl.ToString();
        
        if (_weaponShopLvl < (int)weaponShopMaxLvL)
        {
            _currentWeaponShopLvlUpCost = Mathf.RoundToInt(_currentWeaponShopLvlUpCost + _currentWeaponShopLvlUpCost * weaponShopLvlUpCostIncrease);
            weaponShopLvLUpButton.GetComponentInChildren<TextMeshProUGUI>().text = $"LVL Up - {_currentWeaponShopLvlUpCost}";
            SetRarityText();
        }
        else
        {
            weaponShopLvLUpButton.GetComponentInChildren<TextMeshProUGUI>().text = "Max LVL";
        }
    }

    private void RerollWeapons()
    {
        SetSpritesToWeaponShop();
    }

    private void ToggleStatsheet()
    {
        statPanel.gameObject.SetActive(true);
    }
    
    public void MoveWeapon(bool fromBench, int fromIndex, bool toBench, int toIndex)
{
    GameObject weaponToMove = null;
    Transform sourceParent = null;
    Transform targetParent = null;

    // 1. Quelle bestimmen
    if (fromBench) {
        sourceParent = weaponShopBenchSlots[fromIndex].transform.Find("WeaponPrefab");
    } else {
        sourceParent = inventoryWeaponSlots[fromIndex].Find("WeaponPrefab");
    }

    if (sourceParent == null || sourceParent.childCount == 0) return;
    weaponToMove = sourceParent.GetChild(0).gameObject;

    // 2. Ziel bestimmen
    if (toBench) {
        targetParent = weaponShopBenchSlots[toIndex].transform.Find("WeaponPrefab");
    } else {
        targetParent = inventoryWeaponSlots[toIndex].Find("WeaponPrefab");
    }

    if (targetParent == null) return;

    // 3. Belegungs-Check (Merge oder Swap)
    if (targetParent.childCount > 0)
    {
        GameObject targetWeapon = targetParent.GetChild(0).gameObject;
        WeaponStats statsToMove = weaponToMove.GetComponent<WeaponStats>();
        WeaponStats statsTarget = targetWeapon.GetComponent<WeaponStats>();

        // MERGE LOGIK
        if (statsToMove.weaponName == statsTarget.weaponName && statsToMove.weaponLevel == statsTarget.weaponLevel)
        {
            int maxLevel = weaponLvLColors.Length - 1; 

            if (statsTarget.weaponLevel < maxLevel)
            {
                statsTarget.weaponLevel++;
                
                weaponToMove.transform.SetParent(null);
                weaponToMove.SetActive(false);
                Destroy(weaponToMove);
                
                RefreshAllUI();
                OnWeaponBought?.Invoke();
                return;
            }
        }
        
        // SWAP LOGIK (Wenn kein Merge möglich war oder Max Level erreicht ist)
        // Wir schieben die Ziel-Waffe temporär in den alten Slot der gezogenen Waffe
        targetWeapon.transform.SetParent(sourceParent, false);
        targetWeapon.transform.localPosition = Vector3.zero;
        
        // Dann schieben wir die gezogene Waffe in den Ziel-Slot
        weaponToMove.transform.SetParent(targetParent, false);
        weaponToMove.transform.localPosition = Vector3.zero;
    }
    else
    {
        // Standard Verschieben (Ziel war leer)
        weaponToMove.transform.SetParent(targetParent, false);
        weaponToMove.transform.localPosition = Vector3.zero;
    }

    RefreshAllUI();
    OnWeaponBought?.Invoke();
}
    
    public void RefreshAllUI()
    {
        SetSpritesToInventory();
        UpdateBenchUI();
    }

    private void UpdateBenchUI()
    {
        for (int i = 0; i < weaponShopBenchSlots.Length; i++)
        {
            Image backgroundImage = weaponShopBenchSlots[i].transform.Find("BackgroundImage").GetComponent<Image>();
            Image boarderImage = weaponShopBenchSlots[i].transform.Find("BoarderImage").GetComponent<Image>();
            Image weaponImage = weaponShopBenchSlots[i].transform.Find("WeaponImage").GetComponent<Image>();
            
            Transform weaponPrefabSlot = weaponShopBenchSlots[i].transform.Find("WeaponPrefab");
            
            if (weaponPrefabSlot.childCount > 0)
            {
                Transform weaponPrefab = weaponPrefabSlot.GetChild(0);
                WeaponStats currentWeaponStats = weaponPrefab.GetComponent<WeaponStats>();
                
                int weaponLevel = (int)currentWeaponStats.weaponLevel;
                int weaponTier = (int)currentWeaponStats.weaponTier;
                
                backgroundImage.color = weaponLvLColors[weaponLevel];
                boarderImage.sprite = weaponBoarderSprites[weaponTier - 1]; // tier starts at 1
                weaponImage.sprite = weaponPrefab.GetComponentInChildren<SpriteRenderer>().sprite;
            }
            else
            {
                backgroundImage.color = Color.white;
                backgroundImage.sprite = null;
                weaponImage.sprite = null;
                boarderImage.sprite = null;
            }
        }
    }
    
    private void StartNextWave()
    {
        SetWeaponsToPlayerWeaponManager();
        OnShopCycleEnd?.Invoke();
        this.gameObject.SetActive(false);
    }

    private void SetWeaponsToPlayerWeaponManager()
    {
        GetWeaponAnkers();
        
        for (int i = 0; i < inventoryWeaponSlots.Length; i++)
        {
            Transform weaponPrefabSlot = inventoryWeaponSlots[i].Find("WeaponPrefab");

            if (weaponPrefabSlot != null && weaponPrefabSlot.childCount > 0 && i < playerWeaponAnkers.Count)
            {
                Transform weaponObj = weaponPrefabSlot.GetChild(0);
                weaponObj.SetParent(playerWeaponAnkers[i].transform, false);
                
                weaponObj.localPosition = Vector3.zero;
                weaponObj.localRotation = Quaternion.identity;
                weaponObj.localScale = Vector3.one;
            }
        }
    }
}
