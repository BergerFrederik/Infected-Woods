using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Core: fields, panel lifecycle, shelf switching, wave transition.
// Item-shop logic: ShopPanel.ItemShop.cs
// Weapon-shop (rolls/purchase) logic: ShopPanel.WeaponShop.cs
// Weapon-inventory (combine/move/sell/bench) logic: ShopPanel.WeaponInventory.cs
public partial class ShopPanel : MonoBehaviour
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
    [SerializeField] private Button handleBlossomItemButton;
    [SerializeField] private Transform blossomItemContainer;
    [SerializeField] private float blossomItemCost;
    [SerializeField] private float itemSellPercentage = 70f;
    private Transform _itemToTransact;
    private Transform _itemToTransactOriginalContainer;
    private int _currentShelfIndex;
    private string _purchaseBlossomItemText;
    private bool _isCurrentTransactionBuy;

    public event Action onItemPurchased;
    public event Action onItemSold;
    public event Action<GameObject> OnBlossomItemSold;

    [Header("Weapons")] 
    [SerializeField] private Image[] weaponShopBoarderImages;
    [SerializeField] private Sprite[] weaponShopBoarderSprites;
    [SerializeField] private Button[] weaponButtons;
    [SerializeField] private Image[] weaponImages;
    [SerializeField] private TextMeshProUGUI[] weaponTitles;
    [SerializeField] private TextMeshProUGUI[] weaponSubtitles;
    [SerializeField] private TextMeshProUGUI[] weaponButtonText;
    [SerializeField] private TextMeshProUGUI[] weaponDescriptionText;
    [SerializeField] private List<GameObject> weaponPrefabs;
    [SerializeField] private GameObject playerWeaponManager;
    [SerializeField] private Button weaponRerollButton;
    [SerializeField] private GameObject[] weaponObjects;
    [SerializeField] private TextMeshProUGUI[] weaponOddsTexts;
    [SerializeField] private TextMeshProUGUI weaponShopLvLText;
    [SerializeField] private Button weaponShopLvLUpButton;
    [SerializeField] private TextMeshProUGUI weaponShopLvlUpCostText;
    [SerializeField] private Button sellButton;
    [SerializeField] private Button combineButton;
    [SerializeField] private Button moveButton;
    [SerializeField] private RerollMechanic rerollMechanic;
    [SerializeField] private TextMeshProUGUI rerollCostText;
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
    Dictionary<WeaponTier, List<GameObject>> _weaponsByRarity = new();
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
    [SerializeField] private Transform uiCharacterObject;
    public Transform dragLayer;


    private GameObject[] _arrayOfChosenRandomWeapons;
    private List<GameObject> _playerWeaponAnkers = new();

    public static event Action onShopCycleEnd;
    public event Action onWeaponBought;

    private void OnEnable()
    {
        startNextWaveButton.onClick.AddListener(StartNextWave);
        transactionButton.onClick.AddListener(ItemTransaction);
        weaponRerollButton.onClick.AddListener(RerollWeapons);
        toggleButton.onClick.AddListener(ToggleStatsheet);
        weaponShopLvLUpButton.onClick.AddListener(IncreaseWeaponShopLvL);
        leftSwitchButton.onClick.AddListener (() => SwitchShelf(leftSwitchButton));
        rightSwitchButton.onClick.AddListener(() => SwitchShelf(rightSwitchButton));
        sellButton.onClick.AddListener(SellWeapon);
        combineButton.onClick.AddListener(CombineWeapon);
        moveButton.onClick.AddListener(MoveSelectedWeapon);
        onItemPurchased += RefreshAllUI;
        onItemSold += RefreshAllUI;

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
        sellButton.onClick.RemoveListener(SellWeapon);
        combineButton.onClick.RemoveListener(CombineWeapon);
        moveButton.onClick.RemoveListener(MoveSelectedWeapon);
        onItemPurchased -= RefreshAllUI;
        onItemSold -= RefreshAllUI;

        _itemToTransact = null;
        transactionSectionImage.sprite = null;
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

    private void ToggleStatsheet()
    {
        statPanel.gameObject.SetActive(true);
    }

    private void HandlePurchase(float purchasePrice)
    {
        playerStats.PlayerLightAmount -= Mathf.RoundToInt(purchasePrice);
        SetMoneyToUI();
    }

    private void SetMoneyToUI()
    {
        moneyAmountText.text = playerStats.PlayerLightAmount.ToString();
    }

    private void GetWeaponAnkers()
    {
        _playerWeaponAnkers.Clear();
        for (int i = 0; i < playerWeaponManager.transform.childCount; i++)
        {
            _playerWeaponAnkers.Add(playerWeaponManager.transform.GetChild(i).gameObject);
        }
    }

    public void RefreshAllUI()
    {
        SetMoneyToUI();
        SetSpritesToInventory();
        UpdateBenchUI();
        RefreshWeaponShopStatsText();
    }

    private void StartNextWave()
    {
        ResetShelvesOnNextWave();
        SetWeaponsToPlayerWeaponManager();
        onShopCycleEnd?.Invoke();
        this.gameObject.SetActive(false);
    }

    private void ResetShelvesOnNextWave()
    {
        while (_currentShelfIndex != 0)
        {
            SwitchShelf(leftSwitchButton);
        }
    }

    private void SetWeaponsToPlayerWeaponManager()
    {
        GetWeaponAnkers();

        for (int i = 0; i < inventoryWeaponSlots.Length; i++)
        {
            Transform weaponPrefabSlot = inventoryWeaponSlots[i].Find("WeaponPrefab");

            if (weaponPrefabSlot != null && weaponPrefabSlot.childCount > 0 && i < _playerWeaponAnkers.Count)
            {
                Transform weaponObj = weaponPrefabSlot.GetChild(0);
                weaponObj.SetParent(_playerWeaponAnkers[i].transform, false);

                weaponObj.localPosition = Vector3.zero;
                weaponObj.localRotation = Quaternion.identity;
                weaponObj.localScale = Vector3.one;
            }
        }
    }
}
