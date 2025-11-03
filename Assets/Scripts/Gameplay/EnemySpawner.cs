using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject Timer;
    [SerializeField] private GameObject augmentPanel;
    [SerializeField] private GameObject levelPanel;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject mainCam;

    private PlayerStats playerStats;
    public GameObject[] Waves;
    public static event Action OnRoundOver;
    private GameObject player;
    private readonly List<int> augmentWaves = new List<int> { 1, 5, 10, 15 };

    float currentWaveNumber;
    float currentPlayerLevel;

    void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerStats = player.GetComponent<PlayerStats>();
        gameManager.isRoundOver = false;
        playerStats.playerLeveledUp = false;
        Time.timeScale = 1f;
        gameManager.currentWaveNumber++;
        currentWaveNumber = gameManager.currentWaveNumber;      
        currentPlayerLevel = playerStats.playerLevel;
        InstantiateCurrentWave();
    }

    private void Update()
    {
        if (this.transform.childCount == 0)
        {
            gameManager.isRoundOver = true;
            OnRoundOver?.Invoke();
            if (playerStats.playerLevel - currentPlayerLevel > 0)
            {
                playerStats.playerLeveledUp = true;
            }
            Time.timeScale = 0f;
            DestroyRemainingEnemys();
            ResetPlayerPosition();
            if (augmentWaves.Contains((int)currentWaveNumber))
            {
                augmentPanel.SetActive(true);
            }
            else if (playerStats.playerLeveledUp)
            {
                levelPanel.SetActive(true);
            }
            else
            {
                shopPanel.SetActive(true);
            }
            this.gameObject.SetActive(false);
        }
    }

    private void InstantiateCurrentWave()
    {
        GameObject newWave = Instantiate(Waves[(int)currentWaveNumber - 1], transform.position, Quaternion.identity);
        newWave.transform.SetParent(transform);
        Timer.SetActive(true);
    }

    private void DestroyRemainingEnemys()
    {
        float searchRadius = Mathf.Infinity;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, searchRadius);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
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
}
