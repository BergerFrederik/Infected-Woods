using UnityEngine;

public class ShopPanel : MonoBehaviour
{
    [SerializeField] private GameObject enemySpawner;
    public void StartNewWave()
    {
        enemySpawner.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
