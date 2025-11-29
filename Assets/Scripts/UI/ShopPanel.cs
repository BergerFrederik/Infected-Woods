using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ShopPanel : MonoBehaviour
{
    [SerializeField] private Button startNextWaveButton;
    [SerializeField] private Button[] itemButtons;
    [SerializeField] private Sprite[] itemSprites;
    [SerializeField] private List<GameObject> itemPrefabs;
    [SerializeField] private Button[] weaponButtons;
    [SerializeField] private Sprite[] weaponSprites;
    [SerializeField] private List<GameObject> weaponPrefabs;
    [SerializeField] private GameObject[] playerWeaponAnkers;
    [SerializeField] private Image[] inventoryImages;
    [SerializeField] private GameObject playerItems;
    [SerializeField] private Button itemRerollButton;
    [SerializeField] private Button weaponRerollButton;
    [SerializeField] private TextMeshProUGUI moneyAmountText;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private GameObject itemVisualizerPrefab;
    [SerializeField] private Transform itemScrollViewContent;
    [SerializeField] private Button toggleButton;
    [SerializeField] private StatPanel statPanel;

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
            // get random item
            int randomIndex = Random.Range(0, itemPrefabs.Count);
            GameObject randomItem = itemPrefabs[randomIndex];
            arrayOfChosenRandomItems[i] = randomItem;

            // get Sprite from item
            Sprite sprite = randomItem.GetComponent<SpriteRenderer>().sprite;

            // set sprite to button
            itemButtons[i].image.sprite = sprite;

            // Add Listeners to Button
            itemButtons[i].onClick.RemoveAllListeners();
            int index = i;
            itemButtons[i].onClick.AddListener(() => BuyItem(index));
        }
    }

    private void BuyItem(int index)
    {
        itemButtons[index].onClick.RemoveAllListeners();
        itemButtons[index].image.sprite = null;
        GameObject chosenItem = arrayOfChosenRandomItems[index];
        GameObject boughtItem = Instantiate(chosenItem);
        boughtItem.transform.SetParent(playerItems.transform, false);

        Sprite itemSprite = chosenItem.GetComponent<SpriteRenderer>().sprite;
        AddItemToVisualizer(itemSprite);
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
        for (int i = 0; i < weaponButtons.Length; i++)
        {
            // get random Weapon
            int randomIndex = Random.Range(0, weaponPrefabs.Count);
            GameObject randomWeapon = weaponPrefabs[randomIndex];
            arrayOfChosenRandomWeapons[i] = randomWeapon;

            // get Sprite from Weapon
            Sprite sprite = randomWeapon.GetComponentInChildren<SpriteRenderer>().sprite;

            // set Sprite to Button
            weaponButtons[i].image.sprite = sprite;

            // Add Listeners to Button
            weaponButtons[i].onClick.RemoveAllListeners();
            int index = i;
            weaponButtons[i].onClick.AddListener(() => BuyWeapon(index));
        }        
    }

    private void BuyWeapon(int index)
    {
        weaponButtons[index].onClick.RemoveAllListeners();
        int weaponAnkerIndex = GetNextEmptyWeaponSlotIndex();
        if (weaponAnkerIndex != -1)
        {
            weaponButtons[index].image.sprite = null;
            GameObject chosenWeapon = arrayOfChosenRandomWeapons[index];
            GameObject boughtWeapon = Instantiate(chosenWeapon);
            boughtWeapon.transform.SetParent(playerWeaponAnkers[weaponAnkerIndex].transform, false);
            inventoryImages[weaponAnkerIndex].sprite = chosenWeapon.GetComponentInChildren<SpriteRenderer>().sprite;
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

    private void SetMoneyToUI()
    {
        moneyAmountText.text = playerStats.playerLightAmount.ToString(); 
    }

    private void RerollItems()
    {
        SetSpritesToItemshop();
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
