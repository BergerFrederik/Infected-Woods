using UnityEngine;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private GameObject characterSelectionPanel;

    private void OnEnable()
    {
        startGameButton.onClick.AddListener(() => StartNewGame());
        exitButton.onClick.AddListener(() => ExitGameFunc());
    }

    private void OnDisable()
    {
        startGameButton.onClick.RemoveAllListeners();
        exitButton.onClick.RemoveAllListeners();
    }

    private void StartNewGame()
    {
        characterSelectionPanel.SetActive(true);
        this.gameObject.SetActive(false);
    }


    private void ExitGameFunc()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                        Application.Quit();
        #endif
    }
}

