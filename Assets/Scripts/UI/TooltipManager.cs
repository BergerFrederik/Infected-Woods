using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance;

    [SerializeField] private RectTransform tooltipRect;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float mousePadding = 15f;
    [SerializeField] private float interactionPadding = 5f;
    
    [Header("UI References")]
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI statsText;
    [SerializeField] private TextMeshProUGUI passiveText;
    
    [Header("Interaction Window")]
    [SerializeField] private CanvasGroup interactionCanvasGroup;
    [SerializeField] private RectTransform interactionRect;
    [SerializeField] private TextMeshProUGUI sellButtonText;
    
    [SerializeField] private ShopPanel shopPanel;
    
    public bool isLocked;

    private void Awake()
    {
        Instance = this;
        
        canvasGroup.alpha = 0;
        interactionCanvasGroup.alpha = 0;
        interactionCanvasGroup.interactable = false;
        interactionCanvasGroup.blocksRaycasts = false;
    }

    private void Update()
    {
        if (canvasGroup.alpha > 0 && !isLocked)
        {
            UpdatePosition();
        }

        if (isLocked && Input.GetMouseButtonDown(0))
        {
            if (!RectTransformUtility.RectangleContainsScreenPoint(interactionRect, Input.mousePosition))
            {
                UnlockTooltip();
                HideInteractionWindow();
            }
        }
    }

    private void UpdatePosition()
    {
        Vector2 mousePos = Input.mousePosition;
        
        float tooltipWidth = tooltipRect.rect.width;
        float tooltipHeight = tooltipRect.rect.height;
        
        float pivotX = 0;
        float pivotY = 1;
        
        if (mousePos.x + tooltipWidth + mousePadding > Screen.width)
        {
            pivotX = 1;
        }

        if (mousePos.y - tooltipHeight < 0)
        {
            pivotY = 0;
        }
        
        tooltipRect.pivot = new Vector2(pivotX, pivotY);
        
        float posX = mousePos.x + (pivotX == 0 ? mousePadding : -mousePadding);
        float posY = mousePos.y;

        tooltipRect.position = new Vector2(posX, posY);
    }
    
    public void SetTooltipData(string title, string stats, string passive, Sprite icon)
    {
        titleText.text = title;
        statsText.text = stats;
        passiveText.text = passive;
        itemImage.sprite = icon;
        
        passiveText.gameObject.SetActive(!string.IsNullOrEmpty(passive));
        
        canvasGroup.alpha = 1;
    }

    public void UnlockTooltip()
    {
        isLocked = false;
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
    }

    public void HideInteractionWindow()
    {
        interactionCanvasGroup.alpha = 0;
        interactionCanvasGroup.interactable = false;
        interactionCanvasGroup.blocksRaycasts = false;
    }

    public void HandleInteractionWindow(GameObject selectedWeapon)
    {
        GetSelectedWeapon(selectedWeapon);
        ShowInteractionWindow(selectedWeapon);
    }

    private void GetSelectedWeapon(GameObject selectedWeapon)
    {
        shopPanel.selectedWeapon = selectedWeapon;
    }
    
    private void ShowInteractionWindow(GameObject selectedWeapon)
    {
        isLocked = true;
        
        Vector2 nextWindowPos = GetAnchorForNextWindow();
        interactionRect.pivot = new Vector2(tooltipRect.pivot.x, 1);
        interactionRect.position = nextWindowPos;
        
        interactionCanvasGroup.alpha = 1;
        interactionCanvasGroup.interactable = true;
        interactionCanvasGroup.blocksRaycasts = true;
        
        sellButtonText.text = $"Sell - {shopPanel.GetWeaponRefund(selectedWeapon)}";
    }
    
    
    public Vector2 GetAnchorForNextWindow()
    {
        float tooltipWidth = tooltipRect.rect.width;
        Vector2 currentPos = tooltipRect.position;
        
        if (tooltipRect.pivot.x == 0)
        {
            return new Vector2(currentPos.x + tooltipWidth + interactionPadding, currentPos.y);
        }
        else 
        {
            return new Vector2(currentPos.x - tooltipWidth - interactionPadding, currentPos.y);
        }
    }

    public void Show()
    {
        canvasGroup.alpha = 1;
        //canvasGroup.blocksRaycasts = true;
    }

    public void Hide()
    {
        canvasGroup.alpha = 0;
        //canvasGroup.blocksRaycasts = false;
    }
}