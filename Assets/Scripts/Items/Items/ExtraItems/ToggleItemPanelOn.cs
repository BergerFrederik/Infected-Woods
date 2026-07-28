using UnityEngine;

public class ToggleItemPanelOn : MonoBehaviour
{
    [SerializeField] private ItemInformation itemInformation;
    [SerializeField] private GameObject targetPanel;
    
    private void OnEnable()
    {
        if (itemInformation.IsPlayerRoot())
        {
            targetPanel.SetActive(true);
            Destroy(this.gameObject);
        }
    }
}
