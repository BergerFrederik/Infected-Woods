using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance;

    [SerializeField] private RectTransform tooltipRect;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Vector2 offset = new Vector2(20, 0);
    
    [Header("UI References")]
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI statsText;
    [SerializeField] private TextMeshProUGUI passiveText;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (canvasGroup.alpha > 0)
        {
            UpdatePosition();
        }
    }

    private void UpdatePosition()
    {
        Vector2 mousePos = Input.mousePosition;
        
        if (mousePos.x > Screen.width / 2)
        {
            tooltipRect.pivot = new Vector2(1, 1);
            tooltipRect.position = mousePos - offset;
        }
        else
        {
            tooltipRect.pivot = new Vector2(0, 1);
            tooltipRect.position = mousePos + offset;
        }
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

    public void Show()
    {
        canvasGroup.alpha = 1;
    }

    public void Hide()
    {
        canvasGroup.alpha = 0;
    }
}