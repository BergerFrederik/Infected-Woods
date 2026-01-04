using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Button restartButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private GameManager gameManager;

    private void OnEnable()
    {
        restartButton.onClick.AddListener(() => RestartGame());
        resumeButton.onClick.AddListener(() => ResumeGame());
    }

    private void OnDisable()
    {
        restartButton.onClick.RemoveAllListeners();
        resumeButton.onClick.RemoveAllListeners();
    }

    private void RestartGame()
    {
        gameManager.RestartGame();
    }

    private void ResumeGame()
    {
        gameManager.HandlePauseRequest();
    }
}
