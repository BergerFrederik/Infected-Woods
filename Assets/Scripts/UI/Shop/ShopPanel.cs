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
    [SerializeField] private Transform statPanelTransform;
    
    
    [Header("Items")]
    [SerializeField] private Image transactionSectionImage;
    [SerializeField] private Sprite itemSoldSprite;
    [SerializeField] private Transform playerItemsContainer;
    [SerializeField] private Transform itemInventoryContainer;
    [SerializeField] private Transform transactionItemContainer;
    [SerializeField] private Button transactionButton;
    [SerializeField] private Transform itemShelfContainer;
    [SerializeField] private Button leftSwitchButton;
    [SerializeField] private Button rightSwitchButton;
    [SerializeField] private GameObject[] shelves;
    [SerializeField] private TextMeshProUGUI shelfTextfield;
    [SerializeField] private String[] shelfNameTexts;
    [SerializeField] private BlossomShelf blossomShelf;
    [SerializeField] private Button handleBlossomItemButton;
    [SerializeField] private Transform blossomItemContainer;
    [SerializeField] private float blossomItemCost;
    [SerializeField] private float itemSellPercentage = 70f;
    private Transform _itemToTransact;
    private Transform _itemToTransactOriginalContainer;
    private int _currentShelfIndex;
    private string _purchaseBlossomItemText;
    private bool _isCurrentTransactionBuy;

    public event Action OnItemPurchased;
    public event Action OnItemSold;
    
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
    [SerializeField] private Button sellButton;
    [SerializeField] private Button combineButton;
    [SerializeField] private Button moveButton;
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
    [SerializeField] private float weaponSellFactor = 50f;
    [SerializeField] private float bonusRefundPercentagePerLevel = 10f;
    public GameObject selectedWeapon;
    
    
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
    [SerializeField] private Transform[] weaponShopBenchSlots;
    [SerializeField] private Color[] weaponLvLColors;
    [SerializeField] private Sprite[] weaponBoarderSprites;
    [SerializeField] private Transform[] inventoryWeaponSlots;
    [SerializeField] private Transform rotatingInventory;
    [SerializeField] private Transform rotatingItemPrefab;
    public Transform dragLayer;
    
    
    private GameObject[] arrayOfChosenRandomWeapons;
    private List<GameObject> playerWeaponAnkers = new List<GameObject>();

    public static event Action OnShopCycleEnd;
    public event Action OnWeaponBought;
    

    private void Start()
    {
        _purchaseBlossomItemText = $"Buy - {blossomItemCost}";
        handleBlossomItemButton.GetComponentInChildren<TextMeshProUGUI>().text = _purchaseBlossomItemText;
    }
    
    private void OnEnable()
    {
        startNextWaveButton.onClick.AddListener(StartNextWave);
        transactionButton.onClick.AddListener(ItemTransaction);
        weaponRerollButton.onClick.AddListener(RerollWeapons);
        toggleButton.onClick.AddListener(ToggleStatsheet);
        weaponShopLvLUpButton.onClick.AddListener(IncreaseWeaponShopLvL);
        leftSwitchButton.onClick.AddListener (() => SwitchShelf(leftSwitchButton));
        rightSwitchButton.onClick.AddListener(() => SwitchShelf(rightSwitchButton));
        handleBlossomItemButton.onClick.AddListener(HandleBlossomItemTransaction);
        sellButton.onClick.AddListener(SellWeapon);
        combineButton.onClick.AddListener(CombineWeapon);
        moveButton.onClick.AddListener(MoveWeapon);
        
        SetSpritesToInventoryOnActivate();
        SetSpritesToWeaponShop();
        SetMoneyToUI();
    }

    private void OnDisable()
    {
        startNextWaveButton.onClick.RemoveListener(StartNextWave);
        transactionButton.onClick.RemoveListener(ItemTransaction);
        weaponRerollButton.onClick.RemoveListener(RerollWeapons);
        toggleButton.onClick.RemoveListener(ToggleStatsheet);
        weaponShopLvLUpButton.onClick.RemoveListener(IncreaseWeaponShopLvL);
        leftSwitchButton.onClick.RemoveAllListeners();
        rightSwitchButton.onClick.RemoveAllListeners();
        handleBlossomItemButton.onClick.RemoveListener(HandleBlossomItemTransaction);
        sellButton.onClick.RemoveListener(SellWeapon);
        combineButton.onClick.RemoveListener(CombineWeapon);
        moveButton.onClick.RemoveListener(MoveWeapon);

        _itemToTransact = null;
        transactionSectionImage.sprite = null;
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
                Color c = inventoryImages[i].color;
                inventoryImages[i].color = new Color(c.r, c.g, c.b, 0f);
            }
        }
        
        if (characterObject.transform.childCount > 0)
        {
            GameObject chosenCharacterObject = characterObject.transform.GetChild(0).gameObject;
            Transform visuals = chosenCharacterObject.transform.Find("CharacterVisuals");
            if (visuals != null)
                characterInventoryImage.sprite = visuals.GetComponentInChildren<SpriteRenderer>().sprite;
        }
        RefreshAllUI();
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

                Image boarder = boarderImage.GetComponent<Image>();
                boarder.sprite = weaponBoarderSprites[(int)weaponTier - 1];
                boarder.color = new Color(boarder.color.r, boarder.color.g, boarder.color.b, 1f);

                Image weapon = weaponImage.GetComponent<Image>();
                weapon.sprite = weaponObj.GetComponentInChildren<SpriteRenderer>().sprite;
                weapon.color = new Color(weapon.color.r, weapon.color.g, weapon.color.b, 1f);
            }
            else
            {
                Color cBackground = backgroundImage.GetComponent<Image>().color;
                backgroundImage.GetComponent<Image>().color = new Color(cBackground.r, cBackground.g, cBackground.b, 0f);
                
                Color cBoarder = boarderImage.GetComponent<Image>().color;
                boarderImage.GetComponent<Image>().color = new Color(cBoarder.r, cBoarder.g, cBoarder.b, 0f);
                boarderImage.GetComponent<Image>().sprite = null;

                Color cWeapon = weaponImage.GetComponent<Image>().color;
                weaponImage.GetComponent<Image>().color = new Color(cWeapon.r, cWeapon.g, cWeapon.b, 0f);
                weaponImage.GetComponent<Image>().sprite = null;
            }
        }
    }
    
    public void SelectItemForTransaction(GameObject selectedItem, bool isInShop)
    {
        _itemToTransactOriginalContainer = selectedItem.transform.parent;
        foreach (Transform child in transactionItemContainer)
        {
            Destroy(child.gameObject);
        }
        
        GameObject displayedItem = Instantiate(selectedItem, transactionItemContainer);
        
        if (displayedItem.TryGetComponent<ItemSlot>(out var slotScript))
        {
            Destroy(slotScript);
        }
        
        transactionSectionImage.sprite = displayedItem.GetComponent<ItemInformation>().itemIcon;
        
        String buttonText = isInShop ? "Buy" : "Sell";
        transactionButton.GetComponentInChildren<TextMeshProUGUI>().text = buttonText;
        _isCurrentTransactionBuy = isInShop;
        _itemToTransact = selectedItem.transform;
    }

    private void ItemTransaction()
    {
        if (_isCurrentTransactionBuy) BuyItem(); else SellItem();
    }

    private void BuyItem()
    {
        if (transactionItemContainer.childCount == 0)
        {
            Debug.LogWarning("No item found");
            return;
        }
        
        if (itemInventoryContainer.GetChild(itemInventoryContainer.childCount - 1).childCount > 0)
        {
            Debug.LogWarning("Max item capacity reached");
            return;
        }
        
        Transform transactionItem = transactionItemContainer.GetChild(0);
        
        ItemInformation itemInformation = transactionItem.GetComponent<ItemInformation>();
        float itemPrice = itemInformation.itemPrice;
        float playerLightAmount = playerStats.PlayerLightAmount;
        
        if (playerLightAmount - itemPrice >= 0)
        {
            bool isSpecialItem = itemInformation.tier == ItemInformation.ItemTier.Special;
            if (isSpecialItem)
            {
                InstantiateItemForInventory(transactionItem);
                ResetTransactionSection();
            }
            else
            {
                PutItemIntoInventory(transactionItem);
                HandleItemToTransact();
            }
            HandlePurchase(itemPrice);
            transactionSectionImage.sprite = null;
            OnItemPurchased?.Invoke();
        }
        else
        {
            Debug.LogWarning("Not enough funds");
        }

        _itemToTransactOriginalContainer.GetComponent<Image>().sprite = itemSoldSprite;
    }

    private void PutItemIntoInventory(Transform transactionItem)
    {
        if (transactionItem.GetComponent<ItemInformation>().itemID == "StatShard") return;
        for (int i = 0; i < itemInventoryContainer.childCount; i++)
        {
            if (itemInventoryContainer.GetChild(i).childCount == 0)
            {
                Transform itemInventorySlot = itemInventoryContainer.GetChild(i);
                transactionItem.SetParent(itemInventorySlot);
                transactionItem.name = transactionItem.GetComponent<ItemInformation>().itemID;
                Image itemInventorySlotImage = itemInventorySlot.GetComponent<Image>();
                itemInventorySlotImage.enabled = true;
                itemInventorySlotImage.sprite = transactionItem.GetComponent<ItemInformation>().itemIcon;
                
                InstantiateItemForInventory(transactionItem);
                break;
            }
        }
    }

    private void InstantiateItemForInventory(Transform transactionItem)
    {
        GameObject transactionItemForPlayer = Instantiate(transactionItem.gameObject, playerItemsContainer);
        transactionItemForPlayer.name = transactionItem.GetComponent<ItemInformation>().itemID;
        Transform rotatingItem = Instantiate(rotatingItemPrefab, rotatingInventory);
        rotatingItem.GetComponent<Image>().sprite = transactionItem.GetComponent<ItemInformation>().itemIcon;
        rotatingInventory.GetComponent<EllipseRotation>().Rebuild();
    }
    
    public void InstantTransaction(GameObject itemObj, bool isBuy)
    {
        _itemToTransact = itemObj.transform;

        if (isBuy)
        {
            InstantBuy(itemObj.GetComponent<ItemInformation>());
        }
        else
        {
            SellItem();
        }
    }

    private void InstantBuy(ItemInformation itemInfo)
    {
        if (itemInventoryContainer.GetChild(itemInventoryContainer.childCount - 1).childCount > 0)
        {
            Debug.LogWarning("Max item capacity reached");
            return;
        }
        
        float playerLightAmount = playerStats.PlayerLightAmount;
        if (playerLightAmount >= itemInfo.itemPrice)
        {
            bool isSpecialItem = itemInfo.tier == ItemInformation.ItemTier.Special;
            if (isSpecialItem)
            {
                InstantiateItemForInventory(_itemToTransact);
            }
            else
            {
                PutItemIntoInventory(_itemToTransact);
            }
            
            HandlePurchase(itemInfo.itemPrice);
            OnItemPurchased?.Invoke();
        }
        else
        {
            Debug.LogWarning("Not enough funds");
        }
    }
    

    private void SellItem()
    {
        Transform itemToSell = _itemToTransact;
        if (itemToSell == null && transactionItemContainer.childCount > 0)
        {
            itemToSell = transactionItemContainer.GetChild(0);
        }
        
        if (itemToSell == null)
        {
            Debug.LogWarning("No item found");
            return;
        }
        
        ItemInformation itemInfo = itemToSell.GetComponent<ItemInformation>();
        string targetID = itemInfo.itemID;
        
        if (itemToSell.parent != null && itemToSell.parent.GetComponent<Image>() != null)
        {
            Image itemToSellImage = itemToSell.parent.GetComponent<Image>();
            itemToSellImage.sprite = null;
            itemToSellImage.enabled = false;
        }
        
        float itemRefundValue = itemInfo.itemPrice * (itemSellPercentage / 100f);
        HandlePurchase(-itemRefundValue);

        bool foundSlot = false;
        for (int i = 0; i < itemShelfContainer.childCount; i++)
        {
            Transform currentShelf = itemShelfContainer.GetChild(i);
                
            for (int y = 0; y < currentShelf.childCount; y++)
            {
                Transform currentItemSlot = currentShelf.GetChild(y);
                    
                if (currentItemSlot.name == targetID)
                {
                    currentItemSlot.GetComponent<Image>().sprite = itemInfo.itemIcon;
                    itemToSell.SetParent(currentItemSlot);
                    
                    foundSlot = true;
                    break;
                }
            }

            if (foundSlot) break;
        }
        
        for (int i = 0; i < playerItemsContainer.childCount; i++)
        {
            Transform currentPlayerItem = playerItemsContainer.GetChild(i);
            if (currentPlayerItem.name == targetID)
            {
                Destroy(currentPlayerItem.gameObject);
                break;
            }
        }
        
        OnItemSold?.Invoke();
        ResetTransactionSection();
    }
    

    private void HandleItemToTransact()
    {
        Transform transactionItemShelfContainer = _itemToTransact.parent;
        Destroy(_itemToTransact.gameObject);
        transactionItemShelfContainer.GetComponent<Image>().sprite = null;
        _itemToTransact = null;
        transactionButton.GetComponentInChildren<TextMeshProUGUI>().text = "Buy/Sell";
    }

    private void SwitchShelf(Button pressedButton)
    {
        ResetTransactionSection();
        
        shelves[_currentShelfIndex].SetActive(false);
        
        if (pressedButton == leftSwitchButton)
        {
            _currentShelfIndex--;
        }
        else
        {
            _currentShelfIndex++;
        }
        
        _currentShelfIndex = Mathf.Clamp(_currentShelfIndex, 0, shelves.Length - 1);
        
        shelves[_currentShelfIndex].SetActive(true);
        shelfTextfield.text = shelfNameTexts[_currentShelfIndex];
        
        leftSwitchButton.interactable = (_currentShelfIndex > 0);
        rightSwitchButton.interactable = (_currentShelfIndex < shelves.Length - 1);
    }

    private void ResetTransactionSection()
    {
        foreach (Transform child in transactionItemContainer)
        {
            Destroy(child.gameObject);
        }
        
        _itemToTransact = null;
        transactionSectionImage.sprite = null;
        
        transactionButton.GetComponentInChildren<TextMeshProUGUI>().text = "Buy/Sell";
    }

    private void HandleBlossomItemTransaction()
    {
        bool isRandomItemPresent = blossomItemContainer.childCount > 0;
        bool hasPlayerEnoughFunds = playerStats.PlayerLightAmount >= blossomItemCost;
        bool hasPlayerEmptyItemSlot = itemInventoryContainer.GetChild(itemInventoryContainer.childCount - 1).childCount == 0;
        
        if (!isRandomItemPresent && hasPlayerEnoughFunds && hasPlayerEmptyItemSlot)
        {
            BuyBlossomItem();
        }
        else if (isRandomItemPresent)
        {
            ChooseBlossomItem();
        }
    }
    
    private void BuyBlossomItem()
    {
        blossomShelf.GetRandomItem();
        Transform randomItem = blossomItemContainer.GetChild(0);
        ItemInformation itemInformation = randomItem.GetComponent<ItemInformation>();
        itemInformation.itemPrice = blossomItemCost;
        HandlePurchase(blossomItemCost);

        handleBlossomItemButton.GetComponentInChildren<TextMeshProUGUI>().text = "Choose";
    }

    private void ChooseBlossomItem()
    {
        Transform randomItem = blossomItemContainer.GetChild(0);
        PutItemIntoInventory(randomItem);
        blossomShelf.ResetBlossomShelf();
        
        handleBlossomItemButton.GetComponentInChildren<TextMeshProUGUI>().text = $"Buy - {blossomItemCost}";
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
            weaponStats.ApplyStats();

            // get Sprite from Weapon
            Sprite sprite = randomWeapon.GetComponentInChildren<SpriteRenderer>().sprite;

            // set Sprite to Image
            weaponImages[i].sprite = sprite;
            
            // set Title- and Subtitle-text
            weaponTitles[i].text = weaponStats.weaponName;
            weaponSubtitles[i].text = weaponStats.weaponSubtitle;
            
            // set information text
            weaponDescriptionText[i].text = weaponStats.GetStatsAsText(); 
            
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
        if (_weaponShopLvl >= weaponShopMaxLvL)
        {
            return;
        }
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
        var odds = GetCurrentOdds(_weaponShopLvl);
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
        float playerLightAmount = playerStats.PlayerLightAmount;
        
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
            Instantiate(chosenWeapon, targetParent, false);
            
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

    private void SellWeapon()
    {
        float refund = GetWeaponRefund(selectedWeapon);
        playerStats.PlayerLightAmount += refund;
        
        selectedWeapon.transform.SetParent(null);

        Destroy(selectedWeapon);
        RefreshAllUI();
    }

    public float GetWeaponRefund(GameObject weapon)
    {
        WeaponStats selectedWeaponStats = weapon.GetComponent<WeaponStats>();
        float weaponPrice = selectedWeaponStats.weaponPrice;
        float weaponLevel = selectedWeaponStats.weaponLevel;

        for (int i = 0; i < weaponLevel; i++) // i < weaponLevel to not double at lvl 0
        {
            weaponPrice *= 2f;
        }
        
        float refund = weaponPrice * (weaponSellFactor / 100) + ((bonusRefundPercentagePerLevel / 100) * weaponPrice * weaponLevel);
        return Mathf.CeilToInt(refund);
    }

    private void CombineWeapon()
    {
        int maxLevel = weaponLvLColors.Length - 1;
        WeaponStats swStats = selectedWeapon.GetComponent<WeaponStats>();

        
        if (TryCombineInList(inventoryWeaponSlots, swStats, maxLevel)) return;
        TryCombineInList(weaponShopBenchSlots, swStats, maxLevel);
    }


    private bool TryCombineInList(Transform[] slots, WeaponStats swStats, int maxLevel)
    {
        foreach (Transform slot in slots)
        {
            WeaponStats targetStats = slot.GetComponentInChildren<WeaponStats>();
            
            if (targetStats == null || 
                targetStats.gameObject == selectedWeapon || 
                targetStats.weaponName != swStats.weaponName || 
                targetStats.weaponLevel != swStats.weaponLevel || 
                targetStats.weaponLevel >= maxLevel) 
            {
                continue;
            }
            
            targetStats.weaponLevel++;
            targetStats.ApplyStats();
            
            selectedWeapon.transform.SetParent(null);
            selectedWeapon.SetActive(false);
            Destroy(selectedWeapon);
            
            CloseInteractionWindow();
            RefreshAllUI();

            return true;
        }
        return false;
    }
    

    private void MoveWeapon()
    {
        bool isBenchSlot = selectedWeapon.transform.GetComponentInParent<UISlotHandler>().isBenchSlot;
        Transform[] targetSlots = isBenchSlot ? inventoryWeaponSlots : weaponShopBenchSlots;
        
        MoveWeaponToFreeSpot(targetSlots);
        CloseInteractionWindow();
    }

    private void MoveWeaponToFreeSpot(Transform[] targetSlots)
    {
        foreach (Transform slot in targetSlots)
        {
            Transform weaponPrefabContainer = slot.Find("WeaponPrefab");
            bool isSlotOccupied = weaponPrefabContainer.childCount > 0;
            if (isSlotOccupied) continue;
            
            selectedWeapon.transform.SetParent(weaponPrefabContainer);
            RefreshAllUI();
            return;
        }
    }
    
    private void CloseInteractionWindow()
    {
        TooltipManager.Instance.UnlockTooltip();
        TooltipManager.Instance.HideInteractionWindow();
        selectedWeapon = null;
    }

    private void HandlePurchase(float purchasePrice)
    {
        playerStats.PlayerLightAmount -= Mathf.RoundToInt(purchasePrice);
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
        moneyAmountText.text = playerStats.PlayerLightAmount.ToString(); 
    }

    private void IncreaseWeaponShopLvL()
    {
        if (_weaponShopLvl == (int)weaponShopMaxLvL)
        {
            Debug.Log("Max LVL");
            return;
        }
        
        if (playerStats.PlayerLightAmount < _currentWeaponShopLvlUpCost)
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
                    statsTarget.ApplyStats();
                    
                    weaponToMove.transform.SetParent(null);
                    weaponToMove.SetActive(false);
                    Destroy(weaponToMove);
                    
                    RefreshAllUI();
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
        SetMoneyToUI();
        SetSpritesToInventory();
        UpdateBenchUI();
    }

    private void UpdateBenchUI()
    {
        for (int i = 0; i < weaponShopBenchSlots.Length; i++)
        {
            Image backgroundImage = weaponShopBenchSlots[i].Find("BackgroundImage").GetComponent<Image>();
            Image boarderImage = weaponShopBenchSlots[i].Find("BoarderImage").GetComponent<Image>();
            Image weaponImage = weaponShopBenchSlots[i].Find("WeaponImage").GetComponent<Image>();
            
            Transform weaponPrefabSlot = weaponShopBenchSlots[i].Find("WeaponPrefab");
            
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
        ResetShelvesOnNextWave();
        SetWeaponsToPlayerWeaponManager();
        OnShopCycleEnd?.Invoke();
        this.gameObject.SetActive(false);
    }

    private void ResetShelvesOnNextWave()
    {
        if (blossomItemContainer.childCount > 0)
        {
            ChooseBlossomItem();
        }

        while (_currentShelfIndex != 0)
        {
            SwitchShelf(leftSwitchButton);
        }
        
        
        blossomShelf.ResetBlossomShelf();
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
