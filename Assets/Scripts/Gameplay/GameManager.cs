using System;
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
    private bool IsWaveActive;
    private float currentWaveNumber;
    public Bounds mapSize;
    
    private GameObject CurrentWaveObject;
    private WaveStats waveStats;
    private float remainingTime;

    public static event Action OnRoundOver;
    public static event Action<float> OnTimerChanged;

    public static event Action<float> OnNewWaveRequested;

    private void Start()
    {
        Transform FloorVisuals = Floor.transform.GetChild(0);
        Transform FloorSprite = FloorVisuals.GetChild(0);
        SpriteRenderer FloorRenderer = FloorSprite.GetComponent<SpriteRenderer>();
        mapSize = FloorRenderer.bounds;
    }

    private void OnEnable()
    {
        ShopPanel.OnShopCycleEnd += RequestNewWave;
        EnemySpawner.OnWaveInitialized += NewWaveProcedure;
        StartButton.OnGameStarted += StartGame;
    }

    private void OnDisable()
    {
        ShopPanel.OnShopCycleEnd -= RequestNewWave;
        EnemySpawner.OnWaveInitialized -= NewWaveProcedure;
        StartButton.OnGameStarted -= StartGame;
    }

    private void StartGame()
    {
        RequestNewWave();
    }
    private void Update()
    {
        if (IsWaveActive)
        {
            WaveTimer();
        }
    }

    private void WaveTimer()
    {
        remainingTime -= Time.deltaTime;
        OnTimerChanged?.Invoke(remainingTime);
        if (remainingTime <= 0)
        {
            remainingTime = 0;
            OnRoundOver?.Invoke();
            EndWaveProcedure();
            CycleShops();
        }
    }
    
    private void EndWaveProcedure()
    {
        IsWaveActive = false;
        Time.timeScale = 0f;
        DestroyRemainingEntities();
        ResetPlayerPosition();
        player.gameObject.SetActive(false);
    }
    
    private void DestroyRemainingEntities()
    {
        float searchRadius = Mathf.Infinity;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, searchRadius);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Enemy") || collider.CompareTag("Drop") || collider.CompareTag("Projectile"))
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

    private void RequestNewWave()
    {
        currentWaveNumber++;
        OnNewWaveRequested?.Invoke(currentWaveNumber);
    }
    private void NewWaveProcedure()
    {
        GameObject enemySpawner = GameObject.FindGameObjectWithTag("Spawner");
        Transform enemySpawnerTransform = enemySpawner.transform;
        CurrentWaveObject = enemySpawnerTransform.GetChild(0).gameObject;
        player.gameObject.SetActive(true);
        Time.timeScale = 1f;
        waveStats = CurrentWaveObject.GetComponent<WaveStats>();
        remainingTime = waveStats.waveDuration;
        IsWaveActive = true;
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
}

