using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject Floor;
    [SerializeField] private GameObject CooldownAbilityOverlay;
    [SerializeField] private GameObject ActiveAbilityOverlay;
    [SerializeField] private Player player;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private GameObject mainCam;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private GameObject augmentPanel;
    [SerializeField] private GameObject levelPanel;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private List<float> augmentWaves;

    private bool isAugmentShopOpen = true;
    private float currentWaveNumber = 1f;
    public Bounds mapSize;

    private void Start()
    {
        Transform FloorVisuals = Floor.transform.GetChild(0);
        Transform FloorSprite = FloorVisuals.GetChild(0);
        SpriteRenderer FloorRenderer = FloorSprite.GetComponent<SpriteRenderer>();
        mapSize = FloorRenderer.bounds;
    }

    private void OnEnable()
    {
        Timer.OnRoundOver += EndWaveProcedure;
        Timer.OnRoundOver += CycleShops;
        ShopPanel.OnShopCycleEnd += NewWaveProcedure;
    }

    private void OnDisable()
    {
        Timer.OnRoundOver -= EndWaveProcedure;
        Timer.OnRoundOver -= CycleShops;
        ShopPanel.OnShopCycleEnd += NewWaveProcedure;
    }

    public void CycleShops()
    {
        if (augmentWaves.Contains(currentWaveNumber) && isAugmentShopOpen)
        {
            //InitiateAugmentShop
            augmentPanel.gameObject.SetActive(true);
            isAugmentShopOpen = false;
        }
        else if (playerStats.playerLevelsGained > 0)
        {
            //InitiateLevelUpShop
            playerStats.playerLevelsGained--;
            levelPanel.gameObject.SetActive(true);
        }
        else
        {
            //InitiateItemShop    
            shopPanel.gameObject.SetActive(true);
        }
    }
    

    public void SetAbilityUIActive()
    {
        ActiveAbilityOverlay.SetActive(true);
    }

    public void SetAbilityUIInactive()
    {
        ActiveAbilityOverlay.SetActive(false);
    }

    public void StartAbilityCooldown()
    {
        CooldownAbilityOverlay.SetActive(true);
    }

    public void StopAbilityCooldown()
    {
        CooldownAbilityOverlay.SetActive(false);
    }

    private void EndWaveProcedure()
    {
        Time.timeScale = 0f;
        enemySpawner.gameObject.SetActive(false);
        DestroyRemainingEntities();
        ResetPlayerPosition();
        //player.gameObject.SetActive(false);
    }
    
    private void DestroyRemainingEntities()
    {
        float searchRadius = Mathf.Infinity;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, searchRadius);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Enemy") || collider.CompareTag("Drop"))
            {
                Destroy(collider.gameObject);
            }
        }
    }
    
    private void ResetPlayerPosition()
    {
        player.transform.position = new Vector3(0, 0, 0);
        mainCam.transform.position = new Vector3(0, 0, -10);
    }

    private void NewWaveProcedure()
    {
        //player.gameObject.SetActive(true);
        Time.timeScale = 1f;
        currentWaveNumber++;
        enemySpawner.gameObject.SetActive(true);
    }
}

