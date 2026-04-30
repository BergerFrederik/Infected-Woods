using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BlossomShelf : MonoBehaviour
{
    [SerializeField] private Transform itemSlot;
    [SerializeField] private Image itemImage;
    [SerializeField] private GameObject[] blossomItems;
    [SerializeField] private Button rerollButton;
    [SerializeField] private Button getItemButton;
    [SerializeField] private TextMeshProUGUI numRerollsText;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private float rerollAmount;
    
    private float _numRerolls;

    public float NumRerolls
    {
        get => _numRerolls;
        set => _numRerolls = value;
    }

    private void Start()
    {
        _numRerolls = rerollAmount;
        numRerollsText.text = rerollAmount.ToString();
    }

    private void OnEnable()
    {
        rerollButton.onClick.AddListener(RerollBlossomItem);
    }

    private void OnDisable()
    {
        rerollButton.onClick.RemoveListener(RerollBlossomItem);
    }

    public void GetRandomItem()
    {
        if (getItemButton.gameObject.activeSelf)
        {
            getItemButton.gameObject.SetActive(false);
        }
        
        if (itemSlot.childCount > 0)
        {
            for (int i = 0; i < itemSlot.childCount; i++)
            {
                Destroy(itemSlot.GetChild(i).gameObject);
            } 
        }

        if (!itemName.enabled)
        {
            itemName.enabled = true;
        }
        
        int randomIndex = Random.Range(0, blossomItems.Length);
        
        GameObject randomItem = Instantiate(blossomItems[randomIndex], itemSlot);
        ItemInformation randomItemInformation = randomItem.GetComponent<ItemInformation>();
        
        randomItem.name = randomItemInformation.itemID;
        itemImage.sprite = randomItemInformation.itemIcon;
        itemName.text = randomItemInformation.itemID;
        
        rerollButton.gameObject.SetActive(true);
    }

    private void RerollBlossomItem()
    {
        if (_numRerolls <= 0)
        {
            return;
        }
        
        GetRandomItem();
        _numRerolls--;
        numRerollsText.text = _numRerolls.ToString();
    }

    public void ResetBlossomShelf()
    {
        itemImage.sprite = null;
        rerollButton.gameObject.SetActive(false);
        itemName.enabled = false;
        getItemButton.gameObject.SetActive(true);
        _numRerolls = rerollAmount;
        numRerollsText.text = _numRerolls.ToString();

        for (int i = 0; i < itemSlot.childCount; i++)
        {
            Destroy(itemSlot.GetChild(i).gameObject);
        }
    }
}
