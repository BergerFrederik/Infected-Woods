using System.Collections.Generic; 
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BlossomItemPanel : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI[] blossomItemTitles;
    [SerializeField] private TextMeshProUGUI[] blossomItemContents;
    [SerializeField] private Button[] blossomItemButtons;
    [SerializeField] private Image[] blossomItemIcons;

    [Header("Items")] 
    [SerializeField] private List<GameObject> blossomItems;
    
    private List<GameObject> _randomChosenItems = new();

    private void OnEnable()
    {
        GetRandomItem();
        SetUIToButtons();
    }

    private void OnDisable()
    {
        foreach (Button button in blossomItemButtons)
        {
            button.onClick.RemoveAllListeners();
        }
    }

    private void GetRandomItem()
    {
        int numItemsToShow = blossomItemButtons.Length; // Should be 3
        List<GameObject> currentItemsToChoseFrom = blossomItems;
        
        for (int i = 0; i < numItemsToShow; i++)
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
            
            blossomItemButtons[i].onClick.AddListener(() => SelectItem(i));

            i++;
        }
    }

    private void SelectItem(int index)
    {
        gameObject.SetActive(false);
    }
}