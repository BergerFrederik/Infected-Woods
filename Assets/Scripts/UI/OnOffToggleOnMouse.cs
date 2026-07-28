using System.Collections;
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
    [SerializeField] private float durationToRegisterMouse;

    private Coroutine _enterCoroutine;
    private Coroutine _exitCoroutine;

    private void Start()
    {
        if (targetObject == null)
        {
            targetObject = gameObject;
        }
    }

    private void OnDisable()
    { 
        StopAllCoroutines();
    }

    public void OnPointerEnter(PointerEventData eventData)
    { 
        if (_exitCoroutine != null)
        {
            StopCoroutine(_exitCoroutine);
            _exitCoroutine = null;
        }

        if (!isOnEnter) return;

        if (durationToRegisterMouse > 0f)
        {
            _enterCoroutine = StartCoroutine(ExecuteEnterWithDelay());
        }
        else
        {
            ExecuteEnter();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_enterCoroutine != null)
        {
            StopCoroutine(_enterCoroutine);
            _enterCoroutine = null;
        }

        if (!isOnExit) return;

        if (durationToRegisterMouse > 0f)
        {
            _exitCoroutine = StartCoroutine(ExecuteExitWithDelay());
        }
        else
        {
            ExecuteExit();
        }
    }

    private IEnumerator ExecuteEnterWithDelay()
    {
        yield return new WaitForSeconds(durationToRegisterMouse);
        ExecuteEnter();
        _enterCoroutine = null;
    }

    private IEnumerator ExecuteExitWithDelay()
    {
        yield return new WaitForSeconds(durationToRegisterMouse);
        ExecuteExit();
        _exitCoroutine = null;
    }

    private void ExecuteEnter()
    {
        ApplyToggle();

        if (turnSelfOffOnEnter)
        {
            gameObject.SetActive(false);
        }
    }

    private void ExecuteExit()
    {
        ApplyToggle();

        if (turnSelfOffOnExit)
        {
            gameObject.SetActive(false);
        }
    }

    private void ApplyToggle()
    {
        if (targetObject == null) return;
        targetObject.SetActive(toggleType == ToggleType.On);
    }
}