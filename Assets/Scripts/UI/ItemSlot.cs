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
        if (shopPanel != null)
        {
            if (this.transform.childCount == 0)
            {
                Debug.LogWarning("No Item in Slot");
                return;
            }
            shopPanel.SelectItemForTransaction(this.transform.GetChild(0).gameObject, isInShop);
        }
    }
}
