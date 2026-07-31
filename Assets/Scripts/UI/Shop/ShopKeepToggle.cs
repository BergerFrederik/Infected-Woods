using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopKeepToggle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image shopKeepImage;
    [SerializeField] private Sprite shopKeepActiveSprite;
    [SerializeField] private Sprite shopKeepInactiveSprite;

    public void OnPointerEnter(PointerEventData eventData)
    {
        shopKeepImage.sprite = shopKeepActiveSprite;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        shopKeepImage.sprite = shopKeepInactiveSprite;
    }
}
