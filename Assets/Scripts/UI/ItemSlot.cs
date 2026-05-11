using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    public bool isInShop;
    private ShopPanel shopPanel;
    

    private void Awake()
    {
        shopPanel = GetComponentInParent<ShopPanel>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (shopPanel == null || transform.childCount == 0) return;

        GameObject itemInSlot = transform.GetChild(0).gameObject;

        if (eventData.clickCount == 1)
        {
            shopPanel.SelectItemForTransaction(itemInSlot, isInShop);
        }
        else if (eventData.clickCount == 2)
        {
            shopPanel.InstantTransaction(itemInSlot, isInShop);
        }
    }
}
