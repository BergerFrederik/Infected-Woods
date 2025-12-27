using System;
using UnityEngine;
using UnityEngine.UI;

public class StartButton : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private GameManager gameManager;
    
    public static event Action OnGameStarted;

    private void Awake()
    {
        startButton.onClick.AddListener(() => StartGame());
    }

    private void StartGame()
    {
        OnGameStarted?.Invoke();
        this.transform.parent.gameObject.SetActive(false);
    }
}
