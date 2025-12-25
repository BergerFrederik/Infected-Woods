using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RestartButton : MonoBehaviour
{
    [SerializeField] private Button restartButton;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject StartButton;
    [SerializeField] private GameObject EnemySpawner;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private GameObject PlayerItems;
    [SerializeField] private GameObject PlayerAugments;
    [SerializeField] private GameObject PlayerCharacter;
    [SerializeField] private GameInput gameInput;
    
    private void Awake()
    {
        restartButton.onClick.AddListener(() => RestartGame());
    }

    private void OnDestroy()
    {
        restartButton.onClick.RemoveAllListeners();
    }

    private void RestartGame()
    {
        gameManager.RestartGame();
    }
}
