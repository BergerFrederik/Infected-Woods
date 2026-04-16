using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISlotHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [Header("Slot Settings")]
    public int slotIndex;
    public bool isBenchSlot;
    
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 startPosition;
    private ShopPanel shopPanel;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        
        canvas = GetComponentInParent<Canvas>();
        shopPanel = Object.FindAnyObjectByType<ShopPanel>();
    }
    

    public void OnBeginDrag(PointerEventData eventData)
    {
        Transform prefabSlot = transform.Find("WeaponPrefab");
    
        if (prefabSlot == null || prefabSlot.childCount == 0)
        {
            eventData.pointerDrag = null;
            return;
        }
        
        startPosition = rectTransform.localPosition;
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false; 
        transform.SetAsLastSibling(); 
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        rectTransform.localPosition = startPosition; 
        
        if (shopPanel != null) shopPanel.RefreshAllUI();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            UISlotHandler draggedSource = eventData.pointerDrag.GetComponent<UISlotHandler>();
            if (draggedSource != null && draggedSource != this)
            {
                shopPanel.MoveWeapon(draggedSource.isBenchSlot, draggedSource.slotIndex, this.isBenchSlot, this.slotIndex);
            }
        }
    }
}