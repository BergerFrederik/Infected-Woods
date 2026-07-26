using UnityEngine;
using UnityEngine.EventSystems;

public class OnOffToggleOnMouse : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject targetObject;
    private enum ToggleType
    {
        On,
        Off
    }
    
    [SerializeField] private ToggleType toggleType = ToggleType.On;
    [SerializeField] private bool isOnEnter;
    [SerializeField] private bool isOnExit;
    [SerializeField] private bool turnSelfOffOnEnter;
    [SerializeField] private bool turnSelfOffOnExit;

    private void Start()
    {
        if (targetObject == null)
        {
            targetObject = gameObject;
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isOnEnter) return;
        
        if (toggleType == ToggleType.On)
        {
            targetObject.SetActive(true);
        }
        else
        {
            targetObject.SetActive(false);
        }

        if (turnSelfOffOnEnter)
        {
            gameObject.SetActive(false);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isOnExit) return;
        
        if (toggleType == ToggleType.On)
        {
            targetObject.SetActive(true);
        }
        else
        {
            targetObject.SetActive(false);
        }

        if (turnSelfOffOnExit)
        {
            gameObject.SetActive(false);
        }
    }
}
