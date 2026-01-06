using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public event Action<GameObject> OnCursorHoverEnter;
    public event Action OnCursorHoverExit;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        OnCursorHoverEnter?.Invoke(this.transform.root.gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnCursorHoverExit?.Invoke();
    }
}
