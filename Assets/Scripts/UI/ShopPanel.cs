using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public static event Action OnShopCycleEnd;
    private void OnEnable()
    {
        startNextWaveButton.onClick.AddListener(StartNextWave);

        SetSpritesToInventory();
        SetSpritesToItemshop();
    }

    private void OnDisable()
    {
        startNextWaveButton.onClick?.RemoveListener(StartNextWave);
    }

    private void SetSpritesToInventory()
    {
        for (int i = 0; i < playerWeaponAnkers.Length; i++)
        {
            Sprite sprite = playerWeaponAnkers[i].GetComponentInChildren<SpriteRenderer>().sprite;
            inventoryImages[i].sprite = sprite;
        }
    }

    private void SelectRandomItems()
    {
        for (int i = 0; i < itemButtons.Length; i++)
        {

        }
    }

    private void SetSpritesToItemshop()
    {

    }

    // Set Sprites to Inventory
    // Set Sprites to Itemshop
    // Set Sprites to Weaponshop
    // Choose Weapon
    // Choose Item



    public void StartNextWave()
    {
        OnShopCycleEnd?.Invoke();
        this.gameObject.SetActive(false);
    }
}
