using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Button restartButton;
    [SerializeField] private GameManager gameManager;

    private void OnEnable()
    {
        restartButton.onClick.AddListener(() => RestartGame());
    }

    private void OnDisable()
    {
        restartButton.onClick.RemoveAllListeners();
    }

    private void RestartGame()
    {
        gameManager.RestartGame();
    }
}
