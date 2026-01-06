using UnityEngine;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button returnButton;
    [SerializeField] private GameObject characterSelectionPanel;
    [SerializeField] private GameObject OptionsPanel;
    [SerializeField] private GameObject MenuSelection;

    private void OnEnable()
    {
        startGameButton.onClick.AddListener(() => StartNewGame());
        exitButton.onClick.AddListener(() => ExitGameFunc());
        optionsButton.onClick.AddListener(GoToOptions);
        returnButton.onClick.AddListener(ReturnToMainMenu);
    }

    private void ReturnToMainMenu()
    {
        OptionsPanel.SetActive(false);
        MenuSelection.SetActive(true);
    }

    private void OnDisable()
    {
        startGameButton.onClick.RemoveAllListeners();
        exitButton.onClick.RemoveAllListeners();
        optionsButton.onClick.RemoveListener(GoToOptions);
        returnButton.onClick.RemoveListener(ReturnToMainMenu);
    }

    private void StartNewGame()
    {
        characterSelectionPanel.SetActive(true);
        this.gameObject.SetActive(false);
    }

    private void GoToOptions()
    {
        OptionsPanel.SetActive(true);
        MenuSelection.SetActive(false);
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

