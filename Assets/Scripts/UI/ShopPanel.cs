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
    [SerializeField] private GameObject[] playerWeaponAnkers;
    [SerializeField] private Button weaponRerollButton;
    [SerializeField] private Image[] inventoryImages;
    [SerializeField] private GameObject[] weaponObjects;
    
    
    private GameObject[] arrayOfChosenRandomItems;
    private GameObject[] arrayOfChosenRandomWeapons;

    public static event Action OnShopCycleEnd;
    private void OnEnable()
    {
        startNextWaveButton.onClick.AddListener(StartNextWave);
        itemRerollButton.onClick.AddListener(RerollItems);
        weaponRerollButton.onClick.AddListener(RerollWeapons);
        toggleButton.onClick.AddListener(ToggleStatsheet);
        
        SetSpritesToInventory();
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
        ResetItemText();
    }

    private void SetSpritesToInventory()
    {
        for (int i = 0; i < playerWeaponAnkers.Length; i++)
        {
            if (playerWeaponAnkers[i].transform.childCount != 0)
            {
                Sprite sprite = playerWeaponAnkers[i].GetComponentInChildren<SpriteRenderer>().sprite;
                inventoryImages[i].sprite = sprite;
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
        int num_weapons_in_shop = 4;
        arrayOfChosenRandomWeapons = new GameObject[num_weapons_in_shop];
        for (int i = 0; i < weaponImages.Length; i++)
        {
            // set active if not active
            if (!weaponObjects[i].activeSelf)
            {
                weaponObjects[i].SetActive(true);
            }
            
            // get random Weapon
            int randomIndex = Random.Range(0, weaponPrefabs.Count);
            GameObject randomWeapon = weaponPrefabs[randomIndex];
            arrayOfChosenRandomWeapons[i] = randomWeapon;
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
            weaponButtonText[i].text = weaponStats.weaponPrice.ToString();

            // Add Listeners to Button
            weaponButtons[i].onClick.RemoveAllListeners();
            int index = i;
            weaponButtons[i].onClick.AddListener(() => BuyWeapon(index, weaponStats));
        }        
    }

    private void BuyWeapon(int index, WeaponStats weaponStats)
    {
        float weaponPrice = weaponStats.weaponPrice;
        float playerLightAmount = playerStats.playerLightAmount;
        if (playerLightAmount - weaponPrice >= 0)
        {
            weaponButtons[index].onClick.RemoveAllListeners();
            int weaponAnkerIndex = GetNextEmptyWeaponSlotIndex();
            if (weaponAnkerIndex != -1)
            {
                GameObject chosenWeapon = arrayOfChosenRandomWeapons[index];
                GameObject boughtWeapon = Instantiate(chosenWeapon);
                boughtWeapon.transform.SetParent(playerWeaponAnkers[weaponAnkerIndex].transform, false);
                inventoryImages[weaponAnkerIndex].sprite = chosenWeapon.GetComponentInChildren<SpriteRenderer>().sprite;
                HandlePurchase(weaponPrice);
                weaponObjects[index].SetActive(false);
            }   
        }
    }

    private int GetNextEmptyWeaponSlotIndex()
    {
        int index = - 1;
        for (int i = 0; i < playerWeaponAnkers.Length; i++)
        {
            if (playerWeaponAnkers[i].transform.childCount == 0)
            {
                index = i;
                break;
            }
        }
        return index;
    }

    private void HandlePurchase(float purchasePrice)
    {
        playerStats.playerLightAmount -= purchasePrice;
        SetMoneyToUI();
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

    private void RerollWeapons()
    {
        SetSpritesToWeaponShop();
    }

    private void ToggleStatsheet()
    {
        statPanel.gameObject.SetActive(true);
    }

    public void StartNextWave()
    {
        OnShopCycleEnd?.Invoke();
        this.gameObject.SetActive(false);
    }
}
