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
    private Transform originalParent;
    private ShopPanel shopPanel;
    private int originalIndex;

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

        // 1. Ursprung merken
        originalParent = transform.parent;
        originalIndex = transform.GetSiblingIndex();
        startPosition = rectTransform.position; // Weltposition merken

        // 2. In den DragLayer schieben, damit es über allem ist
        if (shopPanel != null && shopPanel.dragLayer != null)
        {
            transform.SetParent(shopPanel.dragLayer);
        }

        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false; 
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Da es ein RectTransform ist, funktioniert diese einfache Logik wieder:
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // 3. Zurück in den ursprünglichen Parent-Container (Inventar oder Bench)
        transform.SetParent(originalParent);
        transform.SetSiblingIndex(originalIndex);
        
        // 4. Position zurücksetzen (wird durch RefreshAllUI ohnehin glattgezogen)
        rectTransform.position = startPosition; 
        
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