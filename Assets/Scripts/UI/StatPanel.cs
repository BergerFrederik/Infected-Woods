using UnityEngine;
using UnityEngine.UI;

public class StatPanel : MonoBehaviour
{
    [SerializeField] private Button toggleButton;
    private void OnEnable()
    {
        toggleButton.onClick.AddListener(togglePanelOff);
    }

    private void OnDisable()
    {
        toggleButton.onClick.RemoveAllListeners();
    }

    private void togglePanelOff()
    {
        this.gameObject.SetActive(false);
    }
}
