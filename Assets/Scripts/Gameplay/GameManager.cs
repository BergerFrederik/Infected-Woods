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
    [SerializeField] private GameObject[] Characters;
    [SerializeField] private GameObject PlayerCharacter;
    [SerializeField] private GameObject[] Weapons;
    [SerializeField] private GameObject PlayerFirstWeaponAnker;
    [SerializeField] private DifficultySelection difficultySelection;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private GameObject PausePanel;
    [SerializeField] private CharacterSelection _characterSelection;

    private bool isAugmentShopOpen = true;
    private bool isWaveActive;
    private bool isPlayerInRound;
    public float currentWaveNumber;
    public Bounds mapSize;
    
    private GameObject CurrentWaveObject;
    private WaveStats waveStats;
    private float remainingTime;

    public event Action OnGameLoopStart;

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
        difficultySelection.OnDifficultySelected += StartGame;
        gameInput.OnPausePerformed += HandlePauseRequest;
    }

    private void OnDisable()
    {
        ShopPanel.OnShopCycleEnd -= RequestNewWave;
        EnemySpawner.OnWaveInitialized -= NewWaveProcedure;
        difficultySelection.OnDifficultySelected -= StartGame;
        gameInput.OnPausePerformed -= HandlePauseRequest;
    }

    private void StartGame(int difficulty)
    {
        isPlayerInRound = true;
        SetCharacterItems();
        RequestNewWave();
    }
    private void Update()
    {
        if (isWaveActive)
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
        isWaveActive = false;
        Time.timeScale = 0f;
        DestroyRemainingEntities();
        ResetPlayerPosition();
        HandlePlayerWhileShop(false);
    }
    
    private void DestroyRemainingEntities()
    {
        float searchRadius = Mathf.Infinity;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, searchRadius);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Enemy") || collider.CompareTag("Drop") || collider.CompareTag("Deletable") || collider.CompareTag("Projectile"))
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
    
    private void HandlePlayerWhileShop(bool playerStatus)
    {
        PlayerMovement playerMovement = player.GetComponentInChildren<PlayerMovement>();
        DashAbility dashAbility = player.GetComponentInChildren<DashAbility>();
        playerMovement.enabled = playerStatus;
        dashAbility.enabled = playerStatus;
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

    private void SetCharacterItems()
    {
        GameObject preselectedCharacterPrefab = _characterSelection.PreselectedCharacterPrefab;
        GameObject ChosenCharacter = Instantiate(preselectedCharacterPrefab, PlayerCharacter.transform);
        ChosenCharacter.transform.localScale = Vector3.one;

        GameObject startWeaponPrefab = _characterSelection.PreselectedCharacterWeapon;
        GameObject StartWeapon = Instantiate(startWeaponPrefab, PlayerFirstWeaponAnker.transform);
        //StartWeapon.transform.localScale = 10f;
    }

    private void RequestNewWave()
    {
        currentWaveNumber++;
        if (currentWaveNumber == 6f)
        {
            RestartGame();
            return;
        }
        OnNewWaveRequested?.Invoke(currentWaveNumber);
    }
    private void NewWaveProcedure()
    {
        playerStats.playerCurrentHP = playerStats.playerMaxHP;
        playerStats.playerCurrentMP = playerStats.playerMaxMP;
        GameObject enemySpawner = GameObject.FindGameObjectWithTag("Spawner");
        Transform enemySpawnerTransform = enemySpawner.transform;
        CurrentWaveObject = enemySpawnerTransform.GetChild(0).gameObject;
        HandlePlayerWhileShop(true);
        Time.timeScale = 1f;
        waveStats = CurrentWaveObject.GetComponent<WaveStats>();
        remainingTime = waveStats.waveDuration;
        isWaveActive = true;
        OnGameLoopStart?.Invoke();
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
    
    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }

    public void HandlePauseRequest()
    {
        if (!PausePanel.activeSelf && isPlayerInRound)
        {
            PausePanel.SetActive(true);
            gameInput.playerInput.Player.Disable();
            Time.timeScale = 0f;
        }
        else
        {
            PausePanel.SetActive(false);
            gameInput.playerInput.Player.Enable();
            Time.timeScale = 1f;
        }        
    }  
}

