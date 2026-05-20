using UnityEngine;

public class StatShard : MonoBehaviour
{
    [SerializeField] private ItemInformation itemInformation;
    [SerializeField] private GameObject statShardPanel;
    
    private void OnEnable()
    {
        if (itemInformation.IsPlayerRoot())
        {
            statShardPanel.SetActive(true);
            Destroy(this.gameObject);
        }
    }
}
