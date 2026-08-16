using System.Collections.Generic; 
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BlossomItemPanel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ShopPanel shopPanel;
    
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI[] blossomItemTitles;
    [SerializeField] private TextMeshProUGUI[] blossomItemContents;
    [SerializeField] private Button[] blossomItemButtons;
    [SerializeField] private Image[] blossomItemIcons;

    [Header("Items")] 
    [SerializeField] private List<GameObject> blossomItems;

    private List<GameObject> _currentAvailableBlossomItems = new();
    private List<GameObject> _randomChosenItems = new();

    private void Awake()
    {
        _currentAvailableBlossomItems = new List<GameObject>(blossomItems);
        shopPanel.OnBlossomItemSold += HandleBlossomItemSold;
    }
    
    private void OnEnable()
    {
        GetRandomItem();
        SetUIToButtons();
    }

    private void OnDisable()
    {
        _randomChosenItems.Clear();
        foreach (Button button in blossomItemButtons)
        {
            button.onClick.RemoveAllListeners();
        }
    }

    private void OnDestroy()
    {
        shopPanel.OnBlossomItemSold -= HandleBlossomItemSold;
    }

    private void GetRandomItem()
    {
        if (_currentAvailableBlossomItems.Count == 0)
        {
            Debug.LogWarning("No Blossom Item Found");
            gameObject.SetActive(false);
            return;
        }
        int numItemsToShow = blossomItemButtons.Length; // Should be 3
        List<GameObject> currentItemsToChoseFrom = new List<GameObject>(_currentAvailableBlossomItems);

        for (int i = 0; i < numItemsToShow && currentItemsToChoseFrom.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, currentItemsToChoseFrom.Count);
            _randomChosenItems.Add(currentItemsToChoseFrom[randomIndex]);
            if (currentItemsToChoseFrom.Count > 1)
            {
                currentItemsToChoseFrom.RemoveAt(randomIndex);
            }
        }
    }

    private void SetUIToButtons()
    {
        int i = 0;
        foreach (GameObject blossomItem in _randomChosenItems)
        {
            ItemInformation itemInformation = blossomItem.GetComponent<ItemInformation>();
            blossomItemTitles[i].text = itemInformation.itemName;
            blossomItemContents[i].text = itemInformation.passiveDescription;
            blossomItemIcons[i].sprite = itemInformation.itemIcon;

            int index = i;
            blossomItemButtons[i].onClick.AddListener(() => SelectItem(index));

            i++;
        }
    }

    private void SelectItem(int index)
    {
        _currentAvailableBlossomItems.Remove(_randomChosenItems[index]);
        GameObject selectedItem = Instantiate(_randomChosenItems[index], gameObject.transform);
        shopPanel.PutItemIntoInventory(selectedItem.transform);
        gameObject.SetActive(false);
    }

    private void HandleBlossomItemSold(GameObject soldItem)
    {
        ItemInformation itemInformation = soldItem.GetComponent<ItemInformation>();
        
        foreach (GameObject blossomItem in blossomItems)
        {
            ItemInformation blossomItemInformation = blossomItem.GetComponent<ItemInformation>();
            if (itemInformation.itemID == blossomItemInformation.itemID)
            {
                _currentAvailableBlossomItems.Add(blossomItem);
                break;
            }
        }
    }
}